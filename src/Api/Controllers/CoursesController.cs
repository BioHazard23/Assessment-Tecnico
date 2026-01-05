using Application.DTOs;
using Application.UseCases.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly CreateCourseUseCase _createCourse;
    private readonly UpdateCourseUseCase _updateCourse;
    private readonly DeleteCourseUseCase _deleteCourse;
    private readonly PublishCourseUseCase _publishCourse;
    private readonly UnpublishCourseUseCase _unpublishCourse;
    private readonly SearchCoursesUseCase _searchCourses;
    private readonly GetCourseSummaryUseCase _getCourseSummary;

    public CoursesController(
        CreateCourseUseCase createCourse,
        UpdateCourseUseCase updateCourse,
        DeleteCourseUseCase deleteCourse,
        PublishCourseUseCase publishCourse,
        UnpublishCourseUseCase unpublishCourse,
        SearchCoursesUseCase searchCourses,
        GetCourseSummaryUseCase getCourseSummary)
    {
        _createCourse = createCourse;
        _updateCourse = updateCourse;
        _deleteCourse = deleteCourse;
        _publishCourse = publishCourse;
        _unpublishCourse = unpublishCourse;
        _searchCourses = searchCourses;
        _getCourseSummary = getCourseSummary;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _searchCourses.ExecuteAsync(q, status, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        var result = await _getCourseSummary.ExecuteAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
    {
        var result = await _createCourse.ExecuteAsync(dto.Title);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetSummary), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto dto)
    {
        var result = await _updateCourse.ExecuteAsync(id, dto.Title);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _deleteCourse.ExecuteAsync(id, hardDelete: false);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpDelete("{id}/hard")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        var result = await _deleteCourse.ExecuteAsync(id, hardDelete: true);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result = await _publishCourse.ExecuteAsync(id);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPatch("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        var result = await _unpublishCourse.ExecuteAsync(id);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}
