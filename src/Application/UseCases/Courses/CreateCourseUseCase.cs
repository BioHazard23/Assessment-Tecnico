using Application.Common;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Courses;

public class CreateCourseUseCase
{
    private readonly ICourseRepository _courseRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseUseCase(ICourseRepository courseRepo, IUnitOfWork unitOfWork)
    {
        _courseRepo = courseRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> ExecuteAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Guid>.Failure("Course title is required");
        }

        var course = new Course(title);
        await _courseRepo.AddAsync(course);
        await _unitOfWork.CommitAsync();

        return Result<Guid>.Success(course.Id);
    }
}
