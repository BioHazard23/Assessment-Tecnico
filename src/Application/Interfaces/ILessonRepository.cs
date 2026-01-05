using Domain.Entities;

namespace Application.Interfaces;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(Guid id);
    Task<List<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<Lesson?> GetByCourseAndOrderAsync(Guid courseId, int order);
    Task<int> GetMaxOrderByCourseIdAsync(Guid courseId);
    Task AddAsync(Lesson lesson);
    void Update(Lesson lesson);
    void Delete(Lesson lesson);
    Task HardDeleteAsync(Guid id);
}
