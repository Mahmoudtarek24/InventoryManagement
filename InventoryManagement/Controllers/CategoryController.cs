using Application.DTO_s;
using Application.DTO_s.CategoryDto_s;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]///deticate route to access this controller  =>[HttpGet("all")] =>api/Category/all 
	[ApiController] ///to work as api controller when put it we effict automatically 1-(model validation )
	 ///if incoming request dint validate our model constraint will  return outomatically 400 -bad Request
	 ///also response for model binding
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryServices categoryServices;
		public CategoryController(ICategoryServices categoryServices)
		{
			this.categoryServices = categoryServices;	
		}
		
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
		{
			var result = await categoryServices.CreateCategoryAsync(dto);
			return Ok(result);
		}

		[HttpGet("{Id}")]
		public async Task<IActionResult> GetCategoryById([FromRoute] int Id)
		{
			var result=await categoryServices.GetCategoryByIdAsync(Id);		
			return Ok(result);	
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCategory([FromRoute] int id, UpdateCategoryDto dto)
		{
			if (id != dto.CategoryId)
				return BadRequest();

			var result = await categoryServices.UpdateCategoryAsync(id,dto);
			return Ok(result);
		}

		[HttpGet("all")]
		public async Task<IActionResult> GetAllCategories()
		{
			var result = await categoryServices.GetAllCategoryAsync();
			return Ok(result);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> SoftDeleteCategory(int id)
		{
			var result = await categoryServices.SoftDeleteCategoryAsync(id);
			return Ok(result);
		}

		[HttpGet("paged")]
		public async Task<IActionResult> GetCategoriesPaged([FromQuery] BaseQueryParameters query)
		{
			var route = $"{Request.Path}";
			var result = await categoryServices.GetCategoriesWithPaginationAsync(query, route);
			return Ok(result);
		}

	}
}
