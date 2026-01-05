using Application.Common;
using Application.DTOs;
using Application.Interfaces;

namespace Application.UseCases.Lessons;

public class ReorderLessonsUseCase
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderLessonsUseCase(ILessonRepository lessonRepo, IUnitOfWork unitOfWork)
    {
        _lessonRepo = lessonRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid courseId, List<ReorderLessonDto> newOrders)
    {
        // Validate no duplicate orders in the new arrangement
        var duplicateOrders = newOrders
            .GroupBy(x => x.NewOrder)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateOrders.Any())
        {
            return Result.Failure($"Duplicate orders found: {string.Join(", ", duplicateOrders)}");
        }

        var lessons = await _lessonRepo.GetByCourseIdAsync(courseId);

        foreach (var orderDto in newOrders)
        {
            var lesson = lessons.FirstOrDefault(l => l.Id == orderDto.LessonId);
            if (lesson == null)
            {
                return Result.Failure($"Lesson {orderDto.LessonId} not found in course");
            }

            lesson.UpdateOrder(orderDto.NewOrder);
            _lessonRepo.Update(lesson);
        }

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }

    public async Task<Result> MoveUpAsync(Guid lessonId)
    {
        var lesson = await _lessonRepo.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return Result.Failure("Lesson not found");
        }

        var lessons = await _lessonRepo.GetByCourseIdAsync(lesson.CourseId);
        var sortedLessons = lessons.OrderBy(l => l.Order).ToList();

        var currentIndex = sortedLessons.FindIndex(l => l.Id == lessonId);
        if (currentIndex <= 0)
        {
            return Result.Failure("Lesson is already at the top");
        }

        var previousLesson = sortedLessons[currentIndex - 1];
        var currentOrder = lesson.Order;
        var previousOrder = previousLesson.Order;

        // Safe swap strategy: 
        // 1. Move current lesson to temp value to free up the slot
        // 2. Move previous lesson to current lesson's old slot
        // 3. Move current lesson to previous lesson's old slot
        
        // Step 1
        lesson.UpdateOrder(-1 * currentOrder - 1000); // Temp negative value
        _lessonRepo.Update(lesson);
        await _unitOfWork.CommitAsync();

        // Step 2
        previousLesson.UpdateOrder(currentOrder);
        _lessonRepo.Update(previousLesson);
        await _unitOfWork.CommitAsync();

        // Step 3
        lesson.UpdateOrder(previousOrder);
        _lessonRepo.Update(lesson);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<Result> MoveDownAsync(Guid lessonId)
    {
        var lesson = await _lessonRepo.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return Result.Failure("Lesson not found");
        }

        var lessons = await _lessonRepo.GetByCourseIdAsync(lesson.CourseId);
        var sortedLessons = lessons.OrderBy(l => l.Order).ToList();

        var currentIndex = sortedLessons.FindIndex(l => l.Id == lessonId);
        if (currentIndex >= sortedLessons.Count - 1)
        {
            return Result.Failure("Lesson is already at the bottom");
        }

        var nextLesson = sortedLessons[currentIndex + 1];
        var currentOrder = lesson.Order;
        var nextOrder = nextLesson.Order;

        // Safe swap strategy
        
        // Step 1
        lesson.UpdateOrder(-1 * currentOrder - 1000); // Temp negative value
        _lessonRepo.Update(lesson);
        await _unitOfWork.CommitAsync();

        // Step 2
        nextLesson.UpdateOrder(currentOrder);
        _lessonRepo.Update(nextLesson);
        await _unitOfWork.CommitAsync();

        // Step 3
        lesson.UpdateOrder(nextOrder);
        _lessonRepo.Update(lesson);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
