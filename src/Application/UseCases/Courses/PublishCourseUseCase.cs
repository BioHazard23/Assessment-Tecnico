using Application.Common;
using Application.Interfaces;
using Domain.Exceptions;

namespace Application.UseCases.Courses;

public class PublishCourseUseCase
{
    private readonly ICourseRepository _courseRepo;
    private readonly IUnitOfWork _unitOfWork;

    public PublishCourseUseCase(ICourseRepository courseRepo, IUnitOfWork unitOfWork)
    {
        _courseRepo = courseRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(Guid courseId)
    {
        var course = await _courseRepo.GetByIdWithLessonsAsync(courseId);
        if (course == null)
        {
            return Result.Failure("Course not found");
        }

        try
        {
            course.Publish();
            _courseRepo.Update(course);
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
