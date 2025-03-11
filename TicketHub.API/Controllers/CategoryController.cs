using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Category;
using TicketHub.Services.IService;
using TicketHub.Utility.Constants;

namespace TicketHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Authorize(Roles = StaticUserRoles.Admin)]
    public async Task<ActionResult<ResponseDto>> GetCategories
    (
        [FromQuery] string? filterOn,
        [FromQuery] string? filterQuery,
        [FromQuery] string? sortBy,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var responseDto =
            await _categoryService.GetCategories(User, filterOn, filterQuery, sortBy, pageNumber, pageSize);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<ResponseDto>> GetCategory
    (
        [FromRoute] Guid categoryId
    )
    {
        var responseDto = await _categoryService.GetCategory(User, categoryId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPost]
    [Authorize(Roles = StaticUserRoles.Admin)]
    public async Task<ActionResult<ResponseDto>> CreateCategory
    (
        [FromBody] CreateCategoryDto createCategoryDto
    )
    {
        var responseDto = await _categoryService.CreateCategory(User, createCategoryDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpPut]
    [Authorize(Roles = StaticUserRoles.Admin)]
    public async Task<ActionResult<ResponseDto>> UpdateCategory
    (
        [FromBody] UpdateCategoryDto updateCategoryDto
    )
    {
        var responseDto = await _categoryService.UpdateCategory(User, updateCategoryDto);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpDelete("{categoryId}")]
    [Authorize(Roles = StaticUserRoles.Admin)]
    public async Task<ActionResult<ResponseDto>> DeleteCategory
    (
        [FromRoute] Guid categoryId
    )
    {
        var responseDto = await _categoryService.DeleteCategory(User, categoryId);
        return StatusCode(responseDto.StatusCode, responseDto);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ResponseDto>> SearchCategories
    (
        [FromQuery] string categoryName
    )
    {
        var responseDto = await _categoryService.SearchCategory(User, categoryName);
        return StatusCode(responseDto.StatusCode, responseDto);
    }
}