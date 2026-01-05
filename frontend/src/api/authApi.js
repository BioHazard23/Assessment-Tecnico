import axiosInstance from './axiosInstance';

export const register = (email, password, confirmPassword) =>
    axiosInstance.post('/auth/register', { email, password, confirmPassword });

export const login = (email, password) =>
    axiosInstance.post('/auth/login', { email, password });
