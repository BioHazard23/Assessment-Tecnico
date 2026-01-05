using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Lessons;

public class CreateLessonUseCase
{
    private readonly ILessonRepository _lessonRepo;
    private readonly ICourseRepository _courseRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLessonUseCase(
        ILessonRepository lessonRepo,
        ICourseRepository courseRepo,
        IUnitOfWork unitOfWork)
    {
        _lessonRepo = lessonRepo;
        _courseRepo = courseRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> ExecuteAsync(CreateLessonDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result<Guid>.Failure("Lesson title is required");
        }

        var course = await _courseRepo.GetByIdAsync(dto.CourseId);
        if (course == null)
        {
            return Result<Guid>.Failure("Course not found");
        }

        // Auto-assign order if not provided
        var order = dto.Order ?? await _lessonRepo.GetMaxOrderByCourseIdAsync(dto.CourseId) + 1;

        // Check for duplicate order
        var existingLesson = await _lessonRepo.GetByCourseAndOrderAsync(dto.CourseId, order);
        if (existingLesson != null)
        {
            return Result<Guid>.Failure($"A lesson with order {order} already exists in this course");
        }

        var lesson = new Lesson(dto.CourseId, dto.Title, order);
        await _lessonRepo.AddAsync(lesson);
        await _unitOfWork.CommitAsync();

        return Result<Guid>.Success(lesson.Id);
    }
}
