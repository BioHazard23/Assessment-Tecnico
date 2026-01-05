using Application.DTOs;
using Application.UseCases.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/courses/{courseId}/[controller]")]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly CreateLessonUseCase _createLesson;
    private readonly UpdateLessonUseCase _updateLesson;
    private readonly DeleteLessonUseCase _deleteLesson;
    private readonly ReorderLessonsUseCase _reorderLessons;
    private readonly GetLessonsByCourseUseCase _getLessonsByCourse;

    public LessonsController(
        CreateLessonUseCase createLesson,
        UpdateLessonUseCase updateLesson,
        DeleteLessonUseCase deleteLesson,
        ReorderLessonsUseCase reorderLessons,
        GetLessonsByCourseUseCase getLessonsByCourse)
    {
        _createLesson = createLesson;
        _updateLesson = updateLesson;
        _deleteLesson = deleteLesson;
        _reorderLessons = reorderLessons;
        _getLessonsByCourse = getLessonsByCourse;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid courseId)
    {
        var lessons = await _getLessonsByCourse.ExecuteAsync(courseId);
        return Ok(lessons);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateLessonRequest request)
    {
        var dto = new CreateLessonDto(courseId, request.Title, request.Order);
        var result = await _createLesson.ExecuteAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetAll), new { courseId }, new { id = result.Value });
    }

    [HttpPut("{lessonId}")]
    public async Task<IActionResult> Update(Guid courseId, Guid lessonId, [FromBody] UpdateLessonDto dto)
    {
        var result = await _updateLesson.ExecuteAsync(lessonId, dto.Title);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("{lessonId}")]
    public async Task<IActionResult> Delete(Guid courseId, Guid lessonId)
    {
        var result = await _deleteLesson.ExecuteAsync(lessonId, hardDelete: false);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("{lessonId}/hard")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HardDelete(Guid courseId, Guid lessonId)
    {
        var result = await _deleteLesson.ExecuteAsync(lessonId, hardDelete: true);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPatch("{lessonId}/move-up")]
    public async Task<IActionResult> MoveUp(Guid courseId, Guid lessonId)
    {
        var result = await _reorderLessons.MoveUpAsync(lessonId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPatch("{lessonId}/move-down")]
    public async Task<IActionResult> MoveDown(Guid courseId, Guid lessonId)
    {
        var result = await _reorderLessons.MoveDownAsync(lessonId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(Guid courseId, [FromBody] List<ReorderLessonDto> newOrders)
    {
        var result = await _reorderLessons.ExecuteAsync(courseId, newOrders);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}

public record CreateLessonRequest(string Title, int? Order = null);
