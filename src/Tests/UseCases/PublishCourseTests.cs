using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Tests.UseCases;

public class PublishCourseTests
{
    [Fact]
    public void PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var course = new Course("Test Course");
        course.AddLesson(new Lesson(course.Id, "Lesson 1", 1));

        // Act
        course.Publish();

        // Assert
        course.Status.Should().Be(CourseStatus.Published);
    }

    [Fact]
    public void PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var course = new Course("Empty Course");

        // Act
        Action act = () => course.Publish();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot publish course without active lessons");
    }

    [Fact]
    public void PublishCourse_WithOnlyDeletedLessons_ShouldFail()
    {
        // Arrange
        var course = new Course("Course with deleted lessons");
        var lesson = new Lesson(course.Id, "Lesson 1", 1);
        lesson.SoftDelete(); // Mark as deleted
        course.AddLesson(lesson);

        // Act
        Action act = () => course.Publish();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot publish course without active lessons");
    }
}
