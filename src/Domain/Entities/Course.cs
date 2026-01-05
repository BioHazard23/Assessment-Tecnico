using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Course
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public CourseStatus Status { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<Lesson> _lessons = new();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    private Course() { } // EF Core

    public Course(string title)
    {
        Id = Guid.NewGuid();
        Title = title;
        Status = CourseStatus.Draft;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (!_lessons.Any(l => !l.IsDeleted))
        {
            throw new DomainException("Cannot publish course without active lessons");
        }

        Status = CourseStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        Status = CourseStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLesson(Lesson lesson)
    {
        _lessons.Add(lesson);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveLesson(Lesson lesson)
    {
        _lessons.Remove(lesson);
        UpdatedAt = DateTime.UtcNow;
    }
}
