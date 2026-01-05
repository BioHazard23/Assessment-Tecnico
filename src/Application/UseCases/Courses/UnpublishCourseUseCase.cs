using Application.Common;
using Application.Interfaces;

namespace Application.UseCases.Courses;

public class UnpublishCourseUseCase
{
    private readonly ICourseRepository _courseRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UnpublishCourseUseCase(ICourseRepository courseRepo, IUnitOfWork unitOfWork)
    {
        _courseRepo = courseRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid courseId)
    {
        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure("Course not found");
        }

        course.Unpublish();
        _courseRepo.Update(course);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
