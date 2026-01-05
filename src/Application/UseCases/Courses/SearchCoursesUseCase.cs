using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;

namespace Application.UseCases.Courses;

public class SearchCoursesUseCase
{
    private readonly ICourseRepository _courseRepo;

    public SearchCoursesUseCase(ICourseRepository courseRepo)
    {
        _courseRepo = courseRepo;
    }

    public async Task<CourseSearchResultDto> ExecuteAsync(
        string? searchTerm,
        string? statusFilter,
        int page = 1,
        int pageSize = 10)
    {
        CourseStatus? status = null;
        if (!string.IsNullOrEmpty(statusFilter))
        {
            if (Enum.TryParse<CourseStatus>(statusFilter, true, out var parsedStatus))
            {
                status = parsedStatus;
            }
        }

        var (items, totalCount) = await _courseRepo.SearchAsync(searchTerm, status, page, pageSize);

        var courseDtos = items.Select(c => new CourseDto(
            c.Id,
            c.Title,
            c.Status,
            c.CreatedAt,
            c.UpdatedAt,
            c.Lessons.Count(l => !l.IsDeleted)
        )).ToList();

        return new CourseSearchResultDto(
            courseDtos,
            totalCount,
            page,
            pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize)
        );
    }
}
