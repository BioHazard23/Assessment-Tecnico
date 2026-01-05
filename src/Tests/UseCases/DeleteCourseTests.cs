using Domain.Entities;
using FluentAssertions;

namespace Tests.UseCases;

public class DeleteCourseTests
{
    [Fact]
    public void DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var course = new Course("Test Course");
        var originalId = course.Id;

        // Act
        course.SoftDelete();

        // Assert
        course.IsDeleted.Should().BeTrue();
        course.Id.Should().Be(originalId); // ID should remain unchanged
        course.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void SoftDeletedCourse_CanBeRestored()
    {
        // Arrange
        var course = new Course("Test Course");
        course.SoftDelete();

        // Act
        course.Restore();

        // Assert
        course.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void DeleteLesson_ShouldBeSoftDelete()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lesson = new Lesson(courseId, "Test Lesson", 1);
        var originalId = lesson.Id;

        // Act
        lesson.SoftDelete();

        // Assert
        lesson.IsDeleted.Should().BeTrue();
        lesson.Id.Should().Be(originalId);
        lesson.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void SoftDelete_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var course = new Course("Test Course");
        var originalUpdatedAt = course.UpdatedAt;
        
        // Small delay to ensure different timestamp
        Thread.Sleep(10);

        // Act
        course.SoftDelete();

        // Assert
        course.UpdatedAt.Should().BeOnOrAfter(originalUpdatedAt);
    }
}
