using Application.Constants.Enum;
using Application.DTO_s.RolesDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.RoleResponse;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
	public class RoleService : IRoleService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		public RoleService(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> roleManager)
		{
			this.userManager = UserManager;
			this.roleManager = roleManager;
		}
		public async Task<ApiResponse<List<RoleResponseDto>>> GetRoleListAsync()
		{
			var roles =await roleManager.Roles.Select(e=>new RoleResponseDto
			{
				RoleId=e.Id,
				RoleName=e.Name
			}).ToListAsync();

			return ApiResponse<List<RoleResponseDto>>.Success(roles, 200);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> UpdateUserRolesAsync(UpdateUserRolesDto dto)
		{
			var user = await userManager.FindByIdAsync(dto.userId);
			if (user == null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"User with ID '{dto.userId}' not found.");


			var invalidRoles = new List<string>();
			foreach (var roleName in dto.roles)
			{
				var roleExists = await roleManager.RoleExistsAsync(roleName);
				if (!roleExists)
					invalidRoles.Add(roleName);
			}

			if (invalidRoles.Any())
				 return ApiResponse<ConfirmationResponseDto>.ValidationError($"The following roles do not exist: {string.Join(", ", invalidRoles)}");


			var currentRoles = await userManager.GetRolesAsync(user);

			if (currentRoles.Any())
			{
				var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
				if (!removeResult.Succeeded)
				{
					var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
					throw new Exception();
					//throw new InternalServerException($"Failed to remove current roles: {errors}");/////////////
				}
			}

			if (dto.roles.Any())
			{
				var addResult = await userManager.AddToRolesAsync(user, dto.roles);
				if (!addResult.Succeeded)
				{
					var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
					throw new Exception();
					//throw new InternalServerException($"Failed to add new roles: {errors}");///////////////////
				}
			}

			var responseDto = new ConfirmationResponseDto()
			{
				Message = $"User '{user.UserName}' roles updated successfully. New roles: {(dto.roles.Any() ? string.Join(", ", dto.roles) : "None")}",
				status = ConfirmationStatus.Updated
			};
			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}
	}
	
}
