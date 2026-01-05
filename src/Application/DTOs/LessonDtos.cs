namespace Application.DTOs;

public record LessonDto(
    Guid Id,
    Guid CourseId,
    string Title,
    int Order,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateLessonDto(
    Guid CourseId,
    string Title,
    int? Order = null  // If null, auto-assign next order
);

public record UpdateLessonDto(string Title);

public record ReorderLessonDto(Guid LessonId, int NewOrder);
