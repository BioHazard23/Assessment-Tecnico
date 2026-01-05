using Application.Common;
using Application.Interfaces;

namespace Application.UseCases.Lessons;

public class UpdateLessonUseCase
{
    private readonly ILessonRepository _lessonRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLessonUseCase(ILessonRepository lessonRepo, IUnitOfWork unitOfWork)
    {
        _lessonRepo = lessonRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid lessonId, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure("Lesson title is required");
        }

        var lesson = await _lessonRepo.GetByIdAsync(lessonId);
        if (lesson == null)
        {
            return Result.Failure("Lesson not found");
        }

        lesson.Update(title);
        _lessonRepo.Update(lesson);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
