using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<Course?> GetByIdWithLessonsAsync(Guid id);
    Task<List<Course>> GetAllAsync();
    Task<(List<Course> Items, int TotalCount)> SearchAsync(
        string? searchTerm,
        CourseStatus? status,
        int page,
        int pageSize
    );
    Task AddAsync(Course course);
    void Update(Course course);
    void Delete(Course course);
    Task HardDeleteAsync(Guid id);
}
