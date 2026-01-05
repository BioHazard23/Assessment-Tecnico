using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Lessons;

public class GetLessonsByCourseUseCase
{
    private readonly ILessonRepository _lessonRepo;
    private readonly ICourseRepository _courseRepo;

    public GetLessonsByCourseUseCase(ILessonRepository lessonRepo, ICourseRepository courseRepo)
    {
        _lessonRepo = lessonRepo;
        _courseRepo = courseRepo;
    }

    public async Task<List<LessonDto>> ExecuteAsync(Guid courseId)
    {
        var lessons = await _lessonRepo.GetByCourseIdAsync(courseId);

        return lessons
            .OrderBy(l => l.Order)
            .Select(l => new LessonDto(
                l.Id,
                l.CourseId,
                l.Title,
                l.Order,
                l.CreatedAt,
                l.UpdatedAt
            ))
            .ToList();
    }
}
