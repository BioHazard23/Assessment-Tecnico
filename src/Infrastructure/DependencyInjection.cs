using Application.Interfaces;
using Application.UseCases.Courses;
using Application.UseCases.Lessons;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<JwtService>();

        // Repositories
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Use Cases - Courses
        services.AddScoped<CreateCourseUseCase>();
        services.AddScoped<UpdateCourseUseCase>();
        services.AddScoped<DeleteCourseUseCase>();
        services.AddScoped<PublishCourseUseCase>();
        services.AddScoped<UnpublishCourseUseCase>();
        services.AddScoped<SearchCoursesUseCase>();
        services.AddScoped<GetCourseSummaryUseCase>();

        // Use Cases - Lessons
        services.AddScoped<CreateLessonUseCase>();
        services.AddScoped<UpdateLessonUseCase>();
        services.AddScoped<DeleteLessonUseCase>();
        services.AddScoped<ReorderLessonsUseCase>();
        services.AddScoped<GetLessonsByCourseUseCase>();

        return services;
    }
}
