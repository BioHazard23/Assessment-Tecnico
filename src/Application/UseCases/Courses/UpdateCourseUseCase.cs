using Application.Common;
using Application.Interfaces;

namespace Application.UseCases.Courses;

public class UpdateCourseUseCase
{
    private readonly ICourseRepository _courseRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseUseCase(ICourseRepository courseRepo, IUnitOfWork unitOfWork)
    {
        _courseRepo = courseRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid courseId, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure("Course title is required");
        }

        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure("Course not found");
        }

        course.Update(title);
        _courseRepo.Update(course);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
