using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.CategoryDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.Services;
using EducationPlatform.Constants;
using FluentValidation;
using InventoryManagement.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]///deticate route to access this controller  =>[HttpGet("all")] =>api/Category/all 
	[ApiController] ///to work as api controller when put it we effict automatically 1-(model validation )
					///if incoming request dint validate our model constraint will  return outomatically 400 -bad Request
					///also response for model binding
	[ValidateModel]
	[ServiceFilter(typeof(RequireVerifiedSupplierAttribute))]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryServices categoryServices;
		public CategoryController(ICategoryServices categoryServices)
		{
			this.categoryServices = categoryServices;	
		}
		
		[HttpPost]
		[SwaggerOperation(Summary = "Create a new category",
	     Description = "Allows an authorized admin , Supplier and Inventory Manager to create a new product category " +
			"   by providing a name, description (nullable) , and display order.")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.RoleGroup)]
		public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
		{
			var result = await categoryServices.CreateCategoryAsync(dto);
			return StatusCode(result.StatusCode, result);
		}

		//[HttpGet("{Id}")]
		//public async Task<IActionResult> GetCategoryById([FromRoute] int Id)
		//{
		//	var result=await categoryServices.GetCategoryByIdAsync(Id);		
		//	return Ok(result);	
		//}
		[SwaggerOperation(
		Summary = "Update an existing category",
		Description = "Allows an Admin Manager to update an existing product category. " +
				"The request must include a valid Category ID and updated values for name," +
				" description, and display order. Name and display order must be unique. ")]

		[HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		public async Task<IActionResult> UpdateCategory([FromRoute] int id, UpdateCategoryDto dto)
		{
			if (id != dto.CategoryId)
				return BadRequest(ValidationMessages.RouteError);

			var result = await categoryServices.UpdateCategoryAsync(id,dto);
			return StatusCode(result.StatusCode,result);
		}

		[SwaggerOperation(
		Summary = "Get all categories",
		Description = "Returns a list of all available categories." +
			" This endpoint is accessible to authorized users and can be used to populate dropdown lists." +
			" The result includes only basic category information without any related data.")]
		[HttpGet("all")]
		public async Task<IActionResult> GetAllCategories()
		{
			var result = await categoryServices.GetAllCategoryAsync();
			return StatusCode(result.StatusCode, result);
		}

		[HttpDelete("{id}")]
		[SwaggerOperation(
			Summary = "Soft delete or restore a category",
			Description = @"Allows an Admin to soft delete or restore a category by toggling its 'IsDeleted' status. 
		Returns 404 if the category is not found.
		Returns 401 if the category has related products and cannot be deleted.
		If the category is already deleted, this will restore it.
		If it's active, this will mark it as deleted.")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer,Roles =AppRoles.Admin)]
		public async Task<IActionResult> SoftDeleteCategory(int id)
		{
			var result = await categoryServices.SoftDeleteCategoryAsync(id);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("paged")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer)]
		[SwaggerOperation(
		Summary = "Get paginated list of categories",
		Description = @"Returns a paginated list of categories for authorized users. 
		Clients can pass query parameters for page number and page size. 
		The response includes pagination metadata (total count, total pages, next/previous page URIs)
        and a list of categories.")]
		public async Task<IActionResult> GetCategoriesPaged([FromQuery] PaginationQueryParameters query)
		{
			var result = await categoryServices.GetCategoriesWithPaginationAsync(query);
			return StatusCode(result.StatusCode, result);
		}

	}
}
