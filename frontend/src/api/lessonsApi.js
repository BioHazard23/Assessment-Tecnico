import axiosInstance from './axiosInstance';

export const getLessons = (courseId) =>
    axiosInstance.get(`/courses/${courseId}/lessons`);

export const createLesson = (courseId, title, order = null) =>
    axiosInstance.post(`/courses/${courseId}/lessons`, { title, order });

export const updateLesson = (courseId, lessonId, title) =>
    axiosInstance.put(`/courses/${courseId}/lessons/${lessonId}`, { title });

export const deleteLesson = (courseId, lessonId) =>
    axiosInstance.delete(`/courses/${courseId}/lessons/${lessonId}`);

export const moveUp = (courseId, lessonId) =>
    axiosInstance.patch(`/courses/${courseId}/lessons/${lessonId}/move-up`);

export const moveDown = (courseId, lessonId) =>
    axiosInstance.patch(`/courses/${courseId}/lessons/${lessonId}/move-down`);
