import axiosInstance from './axiosInstance';

export const searchCourses = (q = '', status = '', page = 1, pageSize = 10) =>
    axiosInstance.get('/courses/search', { params: { q, status, page, pageSize } });

export const getCourseSummary = (id) =>
    axiosInstance.get(`/courses/${id}/summary`);

export const createCourse = (title) =>
    axiosInstance.post('/courses', { title });

export const updateCourse = (id, title) =>
    axiosInstance.put(`/courses/${id}`, { title });

export const deleteCourse = (id) =>
    axiosInstance.delete(`/courses/${id}`);

export const hardDeleteCourse = (id) =>
    axiosInstance.delete(`/courses/${id}/hard`);

export const publishCourse = (id) =>
    axiosInstance.patch(`/courses/${id}/publish`);

export const unpublishCourse = (id) =>
    axiosInstance.patch(`/courses/${id}/unpublish`);
