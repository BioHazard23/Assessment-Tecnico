import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useConfirm } from '../components/ConfirmModal';
import * as lessonsApi from '../api/lessonsApi';
import { getCourseSummary } from '../api/coursesApi';
import './Lessons.css';

export default function Lessons() {
    const { courseId } = useParams();
    const navigate = useNavigate();
    const { confirm } = useConfirm();

    const [course, setCourse] = useState(null);
    const [lessons, setLessons] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [editingLesson, setEditingLesson] = useState(null);
    const [lessonTitle, setLessonTitle] = useState('');
    const [error, setError] = useState('');

    const fetchData = async () => {
        setLoading(true);
        try {
            const [courseRes, lessonsRes] = await Promise.all([
                getCourseSummary(courseId),
                lessonsApi.getLessons(courseId)
            ]);
            setCourse(courseRes.data);
            setLessons(lessonsRes.data);
        } catch (err) {
            setError('Failed to load lessons');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, [courseId]);

    const handleCreateOrUpdate = async (e) => {
        e.preventDefault();
        try {
            if (editingLesson) {
                await lessonsApi.updateLesson(courseId, editingLesson.id, lessonTitle);
            } else {
                await lessonsApi.createLesson(courseId, lessonTitle);
            }
            setShowModal(false);
            setLessonTitle('');
            setEditingLesson(null);
            fetchData();
        } catch (err) {
            setError(err.response?.data?.error || 'Operation failed');
        }
    };

    const handleDelete = async (lessonId) => {
        const confirmed = await confirm({
            title: 'Eliminar Lección',
            message: '¿Estás seguro de eliminar esta lección?',
            type: 'warning',
            confirmText: 'Eliminar',
            cancelText: 'Cancelar'
        });
        if (!confirmed) return;
        try {
            await lessonsApi.deleteLesson(courseId, lessonId);
            fetchData();
        } catch (err) {
            setError(err.response?.data?.error || 'Error al eliminar');
        }
    };

    const handleMove = async (lessonId, direction) => {
        try {
            if (direction === 'up') {
                await lessonsApi.moveUp(courseId, lessonId);
            } else {
                await lessonsApi.moveDown(courseId, lessonId);
            }
            fetchData();
        } catch (err) {
            setError(err.response?.data?.error || 'Error al mover');
        }
    };

    const openEditModal = (lesson) => {
        setEditingLesson(lesson);
        setLessonTitle(lesson.title);
        setShowModal(true);
    };

    return (
        <div className="lessons-container">
            <header className="header">
                <button className="btn-back" onClick={() => navigate('/courses')}>
                    ← Volver a Cursos
                </button>
                <h1>{course?.title || 'Cargando...'}</h1>
                <span className={`status-badge ${course?.status?.toLowerCase()}`}>
                    {course?.status}
                </span>
            </header>

            {error && <div className="error-banner">{error}</div>}

            <div className="controls">
                <span className="lesson-info">{lessons.length} lección(es)</span>
                <button
                    className="btn-create"
                    onClick={() => { setShowModal(true); setEditingLesson(null); setLessonTitle(''); }}
                >
                    + Agregar Lección
                </button>
            </div>

            {loading ? (
                <div className="loading">Cargando...</div>
            ) : (
                <div className="lessons-list">
                    {lessons.map((lesson, index) => (
                        <div key={lesson.id} className="lesson-card">
                            <div className="lesson-order">#{lesson.order}</div>
                            <div className="lesson-content">
                                <h3>{lesson.title}</h3>
                            </div>
                            <div className="lesson-actions">
                                <button
                                    onClick={() => handleMove(lesson.id, 'up')}
                                    disabled={index === 0}
                                    className="move-btn"
                                >
                                    ↑
                                </button>
                                <button
                                    onClick={() => handleMove(lesson.id, 'down')}
                                    disabled={index === lessons.length - 1}
                                    className="move-btn"
                                >
                                    ↓
                                </button>
                                <button onClick={() => openEditModal(lesson)}>Editar</button>
                                <button onClick={() => handleDelete(lesson.id)} className="delete">
                                    Eliminar
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {!loading && lessons.length === 0 && (
                <div className="empty-state">
                    No hay lecciones aún. ¡Agrega la primera lección a este curso!
                </div>
            )}

            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal" onClick={(e) => e.stopPropagation()}>
                        <h2>{editingLesson ? 'Editar Lección' : 'Agregar Lección'}</h2>
                        <form onSubmit={handleCreateOrUpdate}>
                            <input
                                type="text"
                                placeholder="Título de la lección"
                                value={lessonTitle}
                                onChange={(e) => setLessonTitle(e.target.value)}
                                required
                            />
                            <div className="modal-actions">
                                <button type="button" onClick={() => setShowModal(false)}>Cancelar</button>
                                <button type="submit" className="btn-primary">
                                    {editingLesson ? 'Actualizar' : 'Crear'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}
