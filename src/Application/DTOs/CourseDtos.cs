using Domain.Enums;

namespace Application.DTOs;

public record CourseDto(
    Guid Id,
    string Title,
    CourseStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int LessonCount
);

public record CreateCourseDto(string Title);

public record UpdateCourseDto(string Title);

public record CourseSummaryDto(
    Guid Id,
    string Title,
    CourseStatus Status,
    int TotalLessons,
    DateTime CreatedAt,
    DateTime LastModified
);

public record CourseSearchResultDto(
    List<CourseDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
