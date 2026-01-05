using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByCourseAndOrderAsync(Guid courseId, int order)
    {
        return await _context.Lessons
            .FirstOrDefaultAsync(l => l.CourseId == courseId && l.Order == order);
    }

    public async Task<int> GetMaxOrderByCourseIdAsync(Guid courseId)
    {
        var lessons = await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .ToListAsync();

        return lessons.Any() ? lessons.Max(l => l.Order) : 0;
    }

    public async Task AddAsync(Lesson lesson)
    {
        await _context.Lessons.AddAsync(lesson);
    }

    public void Update(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
    }

    public void Delete(Lesson lesson)
    {
        lesson.SoftDelete();
        _context.Lessons.Update(lesson);
    }

    public async Task HardDeleteAsync(Guid id)
    {
        var lesson = await _context.Lessons
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
        }
    }
}
