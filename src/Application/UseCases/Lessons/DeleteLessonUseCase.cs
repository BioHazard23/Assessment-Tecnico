using Application.Common;
using Application.Interfaces;

namespace Application.UseCases.Lessons;

public class DeleteLessonUseCase
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLessonUseCase(ILessonRepository lessonRepo, IUnitOfWork unitOfWork)
    {
        _lessonRepo = lessonRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid lessonId, bool hardDelete = false)
    {
        var lesson = await _lessonRepo.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return Result.Failure("Lesson not found");
        }

        if (hardDelete)
        {
            await _lessonRepo.HardDeleteAsync(lessonId);
        }
        else
        {
            lesson.SoftDelete();
            _lessonRepo.Update(lesson);
        }

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
