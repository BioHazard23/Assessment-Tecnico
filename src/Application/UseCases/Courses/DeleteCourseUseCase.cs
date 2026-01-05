using Application.Common;
using Application.Interfaces;

namespace Application.UseCases.Courses;

public class DeleteCourseUseCase
{
    private readonly ICourseRepository _courseRepo;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseUseCase(ICourseRepository courseRepo, IUnitOfWork unitOfWork)
    {
        _courseRepo = courseRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid courseId, bool hardDelete = false)
    {
        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null)
        {
            return Result.Failure("Course not found");
        }

        if (hardDelete)
        {
            await _courseRepo.HardDeleteAsync(courseId);
        }
        else
        {
            course.SoftDelete();
            _courseRepo.Update(course);
        }

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
