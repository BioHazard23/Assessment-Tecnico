import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useConfirm } from '../components/ConfirmModal';
import * as coursesApi from '../api/coursesApi';
import './Courses.css';

export default function Courses() {
    const [courses, setCourses] = useState([]);
    const [totalPages, setTotalPages] = useState(1);
    const [page, setPage] = useState(1);
    const [status, setStatus] = useState('');
    const [search, setSearch] = useState('');
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingCourse, setEditingCourse] = useState(null);
    const [courseTitle, setCourseTitle] = useState('');
    const [error, setError] = useState('');

    const { logout, isAdmin, user } = useAuth();
    const { confirm } = useConfirm();
    const navigate = useNavigate();

    const fetchCourses = async () => {
        setLoading(true);
        try {
            const response = await coursesApi.searchCourses(search, status, page, 10);
            setCourses(response.data.items);
            setTotalPages(response.data.totalPages);
        } catch (err) {
            setError('Failed to load courses');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCourses();
    }, [page, status, search]);

    const handleCreateOrUpdate = async (e) => {
        e.preventDefault();
        try {
            if (editingCourse) {
                await coursesApi.updateCourse(editingCourse.id, courseTitle);
            } else {
                await coursesApi.createCourse(courseTitle);
            }
            setShowModal(false);
            setCourseTitle('');
            setEditingCourse(null);
            fetchCourses();
        } catch (err) {
            setError(err.response?.data?.error || 'Operation failed');
        }
    };

    const handleDelete = async (id) => {
        const confirmed = await confirm({
            title: 'Eliminar Curso',
            message: '¿Estás seguro de eliminar este curso? El curso puede ser recuperado después.',
            type: 'warning',
            confirmText: 'Eliminar',
            cancelText: 'Cancelar'
        });
        if (!confirmed) return;
        try {
            await coursesApi.deleteCourse(id);
            fetchCourses();
        } catch (err) {
            setError(err.response?.data?.error || 'Error al eliminar');
        }
    };

    const handleHardDelete = async (id) => {
        const confirmed = await confirm({
            title: 'Eliminación Permanente',
            message: 'Esto eliminará PERMANENTEMENTE el curso y todas sus lecciones. ¡Esta acción no se puede deshacer!',
            type: 'danger',
            confirmText: 'Eliminar para siempre',
            cancelText: 'Cancelar'
        });
        if (!confirmed) return;
        try {
            await coursesApi.hardDeleteCourse(id);
            fetchCourses();
        } catch (err) {
            setError(err.response?.data?.error || 'Error al eliminar');
        }
    };

    const handlePublish = async (id, isPublished) => {
        try {
            if (isPublished) {
                await coursesApi.unpublishCourse(id);
            } else {
                await coursesApi.publishCourse(id);
            }
            fetchCourses();
        } catch (err) {
            setError(err.response?.data?.error || 'Error en la operación');
        }
    };

    const openEditModal = (course) => {
        setEditingCourse(course);
        setCourseTitle(course.title);
        setShowModal(true);
    };

    return (
        <div className="courses-container">
            <header className="header">
                <h1>Plataforma de Cursos</h1>
                <div className="header-right">
                    {isAdmin() && <span className="admin-badge">ADMIN</span>}
                    <span className="user-email">{user?.email}</span>
                    <button className="btn-logout" onClick={logout}>Cerrar Sesión</button>
                </div>
            </header>

            <div className="controls">
                <input
                    type="text"
                    placeholder="Buscar cursos..."
                    value={search}
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }}
                    className="search-input"
                />
                <select
                    value={status}
                    onChange={(e) => { setStatus(e.target.value); setPage(1); }}
                    className="status-filter"
                >
                    <option value="">Todos</option>
                    <option value="Draft">Draft</option>
                    <option value="Published">Published</option>
                </select>
                <button
                    className="btn-create"
                    onClick={() => { setShowModal(true); setEditingCourse(null); setCourseTitle(''); }}
                >
                    + Nuevo Curso
                </button>
            </div>

            {error && <div className="error-banner">{error}</div>}

            {loading ? (
                <div className="loading">Cargando...</div>
            ) : (
                <div className="courses-grid">
                    {courses.map((course) => (
                        <div key={course.id} className="course-card">
                            <div className="course-header">
                                <h3>{course.title}</h3>
                                <span className={`status-badge ${course.status.toLowerCase()}`}>
                                    {course.status}
                                </span>
                            </div>
                            <p className="lesson-count">{course.lessonCount} lecciones</p>
                            <div className="course-actions">
                                <button onClick={() => navigate(`/courses/${course.id}/lessons`)}>
                                    Lecciones
                                </button>
                                <button onClick={() => openEditModal(course)}>Editar</button>
                                <button
                                    onClick={() => handlePublish(course.id, course.status === 'Published')}
                                    className={course.status === 'Published' ? 'unpublish' : 'publish'}
                                >
                                    {course.status === 'Published' ? 'Despublicar' : 'Publicar'}
                                </button>
                                <button onClick={() => handleDelete(course.id)} className="delete">
                                    Eliminar
                                </button>
                                {isAdmin() && (
                                    <button onClick={() => handleHardDelete(course.id)} className="hard-delete">
                                        Eliminar Permanente
                                    </button>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {!loading && courses.length === 0 && (
                <div className="empty-state">No hay cursos. ¡Crea tu primer curso!</div>
            )}

            <div className="pagination">
                <button disabled={page <= 1} onClick={() => setPage(page - 1)}>← Anterior</button>
                <span>Página {page} de {totalPages || 1}</span>
                <button disabled={page >= totalPages} onClick={() => setPage(page + 1)}>Siguiente →</button>
            </div>

            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal" onClick={(e) => e.stopPropagation()}>
                        <h2>{editingCourse ? 'Editar Curso' : 'Crear Curso'}</h2>
                        <form onSubmit={handleCreateOrUpdate}>
                            <input
                                type="text"
                                placeholder="Título del curso"
                                value={courseTitle}
                                onChange={(e) => setCourseTitle(e.target.value)}
                                required
                            />
                            <div className="modal-actions">
                                <button type="button" onClick={() => setShowModal(false)}>Cancelar</button>
                                <button type="submit" className="btn-primary">
                                    {editingCourse ? 'Actualizar' : 'Crear'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}
