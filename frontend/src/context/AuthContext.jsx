import { createContext, useContext, useState, useEffect } from 'react';
import { login as loginApi } from '../api/authApi';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const email = localStorage.getItem('email');
        const roles = JSON.parse(localStorage.getItem('roles') || '[]');

        if (token && email) {
            setUser({ email, roles, token });
        }
        setLoading(false);
    }, []);

    const login = async (email, password) => {
        const response = await loginApi(email, password);
        const { token, roles } = response.data;

        localStorage.setItem('token', token);
        localStorage.setItem('email', email);
        localStorage.setItem('roles', JSON.stringify(roles));

        setUser({ email, roles, token });
        return response;
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('email');
        localStorage.removeItem('roles');
        setUser(null);
    };

    const isAdmin = () => user?.roles?.includes('Admin');

    return (
        <AuthContext.Provider value={{ user, login, logout, loading, isAdmin }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
