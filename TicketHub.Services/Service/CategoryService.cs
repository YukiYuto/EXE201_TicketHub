using System.Security.Claims;
using AutoMapper;
using TicketHub.DataAccess.IRepository;
using TicketHub.Models.Domain;
using TicketHub.Models.DTO;
using TicketHub.Models.DTO.Category;
using TicketHub.Services.IService;

namespace TicketHub.Services.Service;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto> GetCategories
    (
        ClaimsPrincipal user,
        string? filterOn,
        string? filterQuery,
        string? sortBy,
        int pageNumber = 0,
        int pageSize = 0
    )
    {
        IEnumerable<Category> allCateogories = null!;

        // Lấy tất cả các sự kiện có trong database
        allCateogories = await _unitOfWork.CategoryRepository.GetAllAsync();

        // Kiểm tra nếu danh sách events là null hoặc rỗng
        if (!allCateogories.Any())
        {
            return new ResponseDto()
            {
                Message = "There are no Category",
                IsSuccess = true,
                StatusCode = 404,
                Result = null
            };
        }

        var listCateogories = allCateogories.ToList();

        // Filter Query
        if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
        {
            switch (filterOn.Trim().ToLower())
            {
                case "cateogryname":
                    listCateogories = listCateogories.Where(x =>
                        x.CategoryName.Contains(filterQuery, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    break;
                default:
                    break;
            }
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            var sortParams = sortBy.Trim().ToLower().Split('_'); // Chia chuỗi sortBy theo ký tự '_'
            var sortField = sortParams[0]; // Tên cột cần sắp xếp
            var sortDirection = sortParams.Length > 1 ? sortParams[1] : "asc"; // Lấy hướng sắp xếp

            switch (sortField)
            {
                case "cateogryname":
                    listCateogories = sortDirection == "desc"
                        ? listCateogories.OrderByDescending(x => x.CategoryName).ToList()
                        : listCateogories.OrderBy(x => x.CategoryName).ToList();
                    break;
                default:
                    // Sắp xếp mặc định theo ngày gần nhất nếu không có cột phù hợp
                    listCateogories = listCateogories.OrderBy(x => x.CategoryName).ToList();
                    break;
            }
        }
        else
        {
            // Sắp xếp mặc định theo ngày gần nhất nếu không có `sortBy`
            listCateogories = listCateogories.OrderBy(x => x.CategoryName).ToList();
        }

        // Phân trang
        if (pageNumber > 0 && pageSize > 0)
        {
            var skipResult = (pageNumber - 1) * pageSize;
            listCateogories = listCateogories.Skip(skipResult).Take(pageSize).ToList();
        }

        // Chuyển đổi danh sách sự kiện thành DTO
        var categoryDto = listCateogories.Select(categoryItem => new GetCategoryDto()
        {
            CategoryId = categoryItem.CategoryId,
            CategoryName = categoryItem.CategoryName,
            ParentCategoryId = categoryItem.ParentCategoryId
        }).ToList();

        return new ResponseDto()
        {
            Message = "Get Categories successfully",
            IsSuccess = true,
            StatusCode = 200,
            Result = categoryDto
        };
    }

    public async Task<ResponseDto> GetCategory(ClaimsPrincipal user, Guid categoryId)
    {
        // Lấy danh mục theo ID  
        var category = await _unitOfWork.CategoryRepository.GetById(categoryId);
        if (category == null)
        {
            return new ResponseDto
            {
                Message = "Category not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        // Khởi tạo DTO cho danh mục  
        var categoryDto = _mapper.Map<GetCategoryByIdDto>(category);

        // Lấy tên danh mục cha (nếu có)  
        if (category.ParentCategoryId.HasValue)
        {
            var parentCategory = await _unitOfWork.CategoryRepository.GetById(category.ParentCategoryId.Value);
            if (parentCategory != null)
            {
                categoryDto.ParentCategoryName = parentCategory.CategoryName;
            }
        }

        // Lấy danh sách các danh mục con  
        var subcategories = await _unitOfWork.CategoryRepository.GetSubcategories(categoryId);
        if (subcategories != null && subcategories.Any())
        {
            categoryDto.SubcategoryNames = subcategories.Select(sub => sub.CategoryName).ToList();
        }

        return new ResponseDto
        {
            Message = "Category found successfully",
            Result = categoryDto,
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> CreateCategory(ClaimsPrincipal user, CreateCategoryDto createCategoryDto)
    {
        // Kiểm tra và parse ParentCategoryId nếu có
        Guid? parentCategoryId = Guid.TryParse(createCategoryDto.ParentCategoryId, out var guidOutput)
            ? guidOutput
            : (Guid?)null;

        // Tạo Category mới
        var category = new Category
        {
            CategoryId = Guid.NewGuid(),
            CategoryName = createCategoryDto.CategoryName,
            ParentCategoryId = parentCategoryId,
            CreatedTime = DateTime.UtcNow,
            CreatedBy = user.Identity.Name,
            Status = 0,
        };

        // Thêm category vào cơ sở dữ liệu
        await _unitOfWork.CategoryRepository.AddAsync(category);
        var result = await _unitOfWork.SaveAsync();

        // Kiểm tra kết quả lưu vào cơ sở dữ liệu
        if (result <= 0)
        {
            return new ResponseDto
            {
                Message = "Failed to create category",
                IsSuccess = false,
                StatusCode = 500
            };
        }

        return new ResponseDto
        {
            Message = "Category created successfully",
            Result = category,
            IsSuccess = true,
            StatusCode = 200
        };
    }

    public async Task<ResponseDto> UpdateCategory(ClaimsPrincipal user, UpdateCategoryDto updateCategoryDto)
    {
        var categoryId =
            await _unitOfWork.CategoryRepository.GetAsync(x => x.CategoryId == updateCategoryDto.CategoryId);
        if (categoryId == null)
        {
            return new ResponseDto
            {
                Message = "Category not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        //update Category
        categoryId.CategoryName = updateCategoryDto.CategoryName;
        categoryId.ParentCategoryId = updateCategoryDto.ParentCategoryId;
        categoryId.UpdatedBy = user.Identity.Name;
        categoryId.UpdatedTime = DateTime.Now;


        //save changes
        _unitOfWork.CategoryRepository.Update(categoryId);
        var save = await _unitOfWork.SaveAsync();

        return new ResponseDto
        {
            Message = "Category updated successfully",
            Result = categoryId,
            IsSuccess = true,
            StatusCode = 201
        };
    }

    public async Task<ResponseDto> DeleteCategory(ClaimsPrincipal user, Guid categoryId)
    {
        var category = await _unitOfWork.CategoryRepository.GetAsync(x => x.CategoryId == categoryId);
        if (category == null)
        {
            return new ResponseDto
            {
                Message = "Category not found",
                Result = null,
                IsSuccess = false,
                StatusCode = 404
            };
        }

        category.Status = 0;
        category.UpdatedBy = user.Identity.Name;
        category.UpdatedTime = DateTime.UtcNow;

        //save changes
        _unitOfWork.CategoryRepository.Update(category);
        var save = await _unitOfWork.SaveAsync();

        return new ResponseDto()
        {
            Message = "Category delete successfully",
            Result = category,
            IsSuccess = true,
            StatusCode = 201
        };
    }
}