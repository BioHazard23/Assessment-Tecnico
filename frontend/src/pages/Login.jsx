import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { register } from '../api/authApi';

export default function Login() {
    const [isLogin, setIsLogin] = useState(true);
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const { login } = useAuth();
    const navigate = useNavigate();

    // Translate common backend errors to Spanish
    const translateError = (error) => {
        const translations = {
            'Passwords must be at least 6 characters': 'Mínimo 6 caracteres',
            'Passwords must have at least one non alphanumeric character': 'Requiere un carácter especial (!@#$)',
            'Passwords must have at least one uppercase': 'Requiere una mayúscula (A-Z)',
            "Passwords must have at least one uppercase ('A'-'Z')": 'Requiere una mayúscula',
            'Cannot publish course without active lessons': 'El curso necesita lecciones para publicar',
            'Invalid credentials': 'Credenciales inválidas',
            'User already exists': 'El usuario ya existe'
        };

        for (const [eng, esp] of Object.entries(translations)) {
            if (error.includes(eng)) {
                error = error.replace(eng, esp);
            }
        }
        return error;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            if (isLogin) {
                await login(email, password);
                navigate('/courses');
            } else {
                if (password !== confirmPassword) {
                    setError('Las contraseñas no coinciden');
                    setLoading(false);
                    return;
                }
                await register(email, password, confirmPassword);
                await login(email, password);
                navigate('/courses');
            }
        } catch (err) {
            const rawError = err.response?.data?.error || err.response?.data?.errors?.join(', ') || 'Ha ocurrido un error';
            setError(translateError(rawError));
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={styles.container}>
            {/* Left side - Branding */}
            <div style={styles.leftPanel}>
                <div style={styles.brandContent}>
                    <h1 style={styles.brandTitle}>Plataforma de Cursos</h1>
                    <p style={styles.brandSubtitle}>
                        Gestiona tus cursos y lecciones de manera simple y eficiente.
                    </p>
                    <div style={styles.features}>
                        <div style={styles.feature}>
                            <span style={styles.featureIcon}>●</span>
                            <span>Crea y organiza cursos</span>
                        </div>
                        <div style={styles.feature}>
                            <span style={styles.featureIcon}>●</span>
                            <span>Gestiona lecciones</span>
                        </div>
                        <div style={styles.feature}>
                            <span style={styles.featureIcon}>●</span>
                            <span>Publica cuando estés listo</span>
                        </div>
                    </div>
                </div>
            </div>

            {/* Right side - Form */}
            <div style={styles.rightPanel}>
                <div style={styles.formContainer}>
                    <div style={styles.formHeader}>
                        <h2 style={styles.formTitle}>
                            {isLogin ? 'Bienvenido' : 'Crear cuenta'}
                        </h2>
                        <p style={styles.formSubtitle}>
                            {isLogin
                                ? 'Ingresa tus datos para continuar'
                                : 'Completa el formulario para registrarte'}
                        </p>
                    </div>

                    {error && <div style={styles.error}>{error}</div>}

                    <form onSubmit={handleSubmit}>
                        <div style={styles.inputGroup}>
                            <label style={styles.label}>Correo electrónico</label>
                            <input
                                type="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                                placeholder="tu@correo.com"
                                style={styles.input}
                            />
                        </div>

                        <div style={styles.inputGroup}>
                            <label style={styles.label}>Contraseña</label>
                            <input
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                                placeholder="••••••••"
                                style={styles.input}
                            />
                        </div>

                        {!isLogin && (
                            <div style={styles.inputGroup}>
                                <label style={styles.label}>Confirmar contraseña</label>
                                <input
                                    type="password"
                                    value={confirmPassword}
                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                    required
                                    placeholder="••••••••"
                                    style={styles.input}
                                />
                            </div>
                        )}

                        <button
                            type="submit"
                            style={{
                                ...styles.button,
                                opacity: loading ? 0.6 : 1,
                                cursor: loading ? 'not-allowed' : 'pointer'
                            }}
                            disabled={loading}
                        >
                            {loading ? 'Procesando...' : (isLogin ? 'Ingresar' : 'Crear cuenta')}
                        </button>
                    </form>

                    <div style={styles.divider}>
                        <span style={styles.dividerLine}></span>
                        <span style={styles.dividerText}>o</span>
                        <span style={styles.dividerLine}></span>
                    </div>

                    <button
                        style={styles.switchButton}
                        onClick={() => setIsLogin(!isLogin)}
                    >
                        {isLogin ? 'Crear una cuenta nueva' : 'Ya tengo una cuenta'}
                    </button>


                </div>
            </div>
        </div>
    );
}

const styles = {
    container: {
        minHeight: '100vh',
        display: 'flex',
        fontFamily: "'Segoe UI', 'Inter', system-ui, sans-serif"
    },
    leftPanel: {
        flex: 1,
        background: '#3d3229',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '3rem'
    },
    brandContent: {
        maxWidth: '400px',
        color: '#f5ebe0'
    },
    brandTitle: {
        fontSize: '2.5rem',
        fontWeight: '700',
        margin: '0 0 1rem 0',
        letterSpacing: '-0.5px'
    },
    brandSubtitle: {
        fontSize: '1.125rem',
        opacity: 0.8,
        margin: '0 0 2.5rem 0',
        lineHeight: 1.6
    },
    features: {
        display: 'flex',
        flexDirection: 'column',
        gap: '1rem'
    },
    feature: {
        display: 'flex',
        alignItems: 'center',
        gap: '0.75rem',
        fontSize: '0.9375rem',
        opacity: 0.9
    },
    featureIcon: {
        color: '#d5bdaf',
        fontSize: '0.625rem'
    },
    rightPanel: {
        flex: 1,
        background: '#edede9',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '2rem'
    },
    formContainer: {
        width: '100%',
        maxWidth: '360px'
    },
    formHeader: {
        marginBottom: '2rem'
    },
    formTitle: {
        fontSize: '1.5rem',
        fontWeight: '600',
        color: '#2d2a26',
        margin: '0 0 0.5rem 0'
    },
    formSubtitle: {
        fontSize: '0.9375rem',
        color: '#5c5650',
        margin: 0
    },
    error: {
        background: '#f8d7da',
        color: '#a04040',
        padding: '0.75rem 1rem',
        borderRadius: '8px',
        marginBottom: '1.5rem',
        fontSize: '0.875rem'
    },
    inputGroup: {
        marginBottom: '1.25rem'
    },
    label: {
        display: 'block',
        marginBottom: '0.5rem',
        color: '#3d3229',
        fontWeight: '500',
        fontSize: '0.8125rem'
    },
    input: {
        width: '100%',
        padding: '0.75rem 1rem',
        border: '1px solid #d6ccc2',
        borderRadius: '8px',
        fontSize: '0.9375rem',
        boxSizing: 'border-box',
        outline: 'none',
        backgroundColor: 'white',
        color: '#2d2a26',
        transition: 'border-color 0.15s'
    },
    button: {
        width: '100%',
        padding: '0.875rem',
        background: '#3d3229',
        color: 'white',
        border: 'none',
        borderRadius: '8px',
        fontSize: '0.9375rem',
        fontWeight: '600',
        transition: 'transform 0.15s, box-shadow 0.15s'
    },
    divider: {
        display: 'flex',
        alignItems: 'center',
        gap: '1rem',
        margin: '1.5rem 0'
    },
    dividerLine: {
        flex: 1,
        height: '1px',
        background: '#d6ccc2'
    },
    dividerText: {
        color: '#5c5650',
        fontSize: '0.8125rem'
    },
    switchButton: {
        width: '100%',
        padding: '0.75rem',
        background: 'transparent',
        color: '#5c4f43',
        border: '1px solid #d6ccc2',
        borderRadius: '8px',
        fontSize: '0.875rem',
        fontWeight: '500',
        cursor: 'pointer',
        transition: 'all 0.15s'
    },

};
