using Application.Common;
using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Courses;

public class GetCourseSummaryUseCase
{
    private readonly ICourseRepository _courseRepo;

    public GetCourseSummaryUseCase(ICourseRepository courseRepo)
    {
        _courseRepo = courseRepo;
    }

    public async Task<Result<CourseSummaryDto>> ExecuteAsync(Guid courseId)
    {
        var course = await _courseRepo.GetByIdWithLessonsAsync(courseId);
        if (course == null)
        {
            return Result<CourseSummaryDto>.Failure("Course not found");
        }

        var activeLessons = course.Lessons.Where(l => !l.IsDeleted).ToList();
        var lastModified = activeLessons.Any()
            ? new[] { course.UpdatedAt }.Concat(activeLessons.Select(l => l.UpdatedAt)).Max()
            : course.UpdatedAt;

        var summary = new CourseSummaryDto(
            course.Id,
            course.Title,
            course.Status,
            activeLessons.Count,
            course.CreatedAt,
            lastModified
        );

        return Result<CourseSummaryDto>.Success(summary);
    }
}
