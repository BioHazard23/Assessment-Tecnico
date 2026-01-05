using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Lessons;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Tests.UseCases;

public class CreateLessonTests
{
    [Fact]
    public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        
        var mockCourseRepo = new Mock<ICourseRepository>();
        mockCourseRepo.Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(new Course("Test Course"));

        var mockLessonRepo = new Mock<ILessonRepository>();
        mockLessonRepo.Setup(r => r.GetByCourseAndOrderAsync(courseId, 1))
            .ReturnsAsync((Lesson?)null); // No existing lesson with order 1
        mockLessonRepo.Setup(r => r.GetMaxOrderByCourseIdAsync(courseId))
            .ReturnsAsync(0);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var useCase = new CreateLessonUseCase(
            mockLessonRepo.Object,
            mockCourseRepo.Object,
            mockUnitOfWork.Object
        );

        var dto = new CreateLessonDto(courseId, "New Lesson", 1);

        // Act
        var result = await useCase.ExecuteAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingLesson = new Lesson(courseId, "Existing Lesson", 1);

        var mockCourseRepo = new Mock<ICourseRepository>();
        mockCourseRepo.Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(new Course("Test Course"));

        var mockLessonRepo = new Mock<ILessonRepository>();
        mockLessonRepo.Setup(r => r.GetByCourseAndOrderAsync(courseId, 1))
            .ReturnsAsync(existingLesson); // Existing lesson with order 1

        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var useCase = new CreateLessonUseCase(
            mockLessonRepo.Object,
            mockCourseRepo.Object,
            mockUnitOfWork.Object
        );

        var dto = new CreateLessonDto(courseId, "New Lesson", 1);

        // Act
        var result = await useCase.ExecuteAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateLesson_WithAutoOrder_ShouldAssignNextOrder()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        var mockCourseRepo = new Mock<ICourseRepository>();
        mockCourseRepo.Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(new Course("Test Course"));

        var mockLessonRepo = new Mock<ILessonRepository>();
        mockLessonRepo.Setup(r => r.GetMaxOrderByCourseIdAsync(courseId))
            .ReturnsAsync(3); // Current max is 3
        mockLessonRepo.Setup(r => r.GetByCourseAndOrderAsync(courseId, 4))
            .ReturnsAsync((Lesson?)null); // Order 4 is available

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var useCase = new CreateLessonUseCase(
            mockLessonRepo.Object,
            mockCourseRepo.Object,
            mockUnitOfWork.Object
        );

        var dto = new CreateLessonDto(courseId, "Auto-ordered Lesson", null); // No order specified

        // Act
        var result = await useCase.ExecuteAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
