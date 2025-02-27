using System.Security.Claims;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Category;

namespace TicketHub.Services.IService;

public interface ICategoryService
{
    Task<ResponseDto> GetCategories
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    );

    Task<ResponseDto> GetCategory(ClaimsPrincipal user, Guid categoryId);
    Task<ResponseDto> CreateCategory(ClaimsPrincipal user, CreateCategoryDto createCategoryDto);
    Task<ResponseDto> UpdateCategory(ClaimsPrincipal user, UpdateCategoryDto updateCategoryDto);
    Task<ResponseDto> DeleteCategory(ClaimsPrincipal user, Guid categoryId);
    Task<ResponseDto> SearchCategory(ClaimsPrincipal user, string nameCategory);
}