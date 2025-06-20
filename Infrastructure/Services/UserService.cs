using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.Services;
using Domain.Parameters;
using Infrastructure.Mappings;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		public UserService(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> roleManager)
		{
			this.userManager = UserManager;
			this.roleManager = roleManager;
		}
		public async Task<ApiResponse<AuthenticationResponseDto>> FindByIdAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new Exception();
			//throw new NotFoundException($"User with ID '{userId}' not found.");

			var userRoles = await userManager.GetRolesAsync(user);

			var responseDto = user.ToResponseDto(userRoles.ToArray());
			return ApiResponse<AuthenticationResponseDto>.Success(responseDto, 200);
		}
		public async Task<ApiResponse<AuthenticationResponseDto>> FindByEmailAsync(string email)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
				throw new Exception();
			//throw new NotFoundException($"User with email '{email}' not found.");

			// Get user roles
			var userRoles = await userManager.GetRolesAsync(user);

			var responseDto = user.ToResponseDto(userRoles.ToArray());
			return ApiResponse<AuthenticationResponseDto>.Success(responseDto, 200);
		}

		public async Task<PagedResponse<List<AuthenticationResponseDto>>> GetUsersWithPaginationAsync(UserQueryParameters query, string route)
		{
			var parameter = new BaseFilter()
			{
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
				searchTearm = query.searchTearm,
				SortAscending = query.SortAscending,
				SortBy = query.SortBy,
			};
			var (users, totalCount) = await GetUsersWithFiltersAsync(parameter);

			var userDtos = users.Select(e=>e.ToResponseDto()).ToList();

			var message = !userDtos.Any() ? "No users found matching your criteria." : null;
			var pagedResult = PagedResponse<List<AuthenticationResponseDto>>.SimpleResponse(userDtos, parameter.PageNumber, parameter.PageSize, totalCount, message);
			return pagedResult;

		}
		private async Task<(List<ApplicationUser>, int)> GetUsersWithFiltersAsync(BaseFilter userFilter)
		{
			var query = userManager.Users.AsQueryable();

			if (!string.IsNullOrEmpty(userFilter.searchTearm))
				query = query.Where(e =>
					  e.UserName.Contains(userFilter.searchTearm, StringComparison.OrdinalIgnoreCase) ||
					  e.Email.Contains(userFilter.searchTearm, StringComparison.OrdinalIgnoreCase) ||
					  (e.PhoneNumber != null && e.PhoneNumber.Contains(userFilter.searchTearm, StringComparison.OrdinalIgnoreCase)));

			int totalCount = await query.CountAsync();

			if (!string.IsNullOrEmpty(userFilter.SortBy))
			{
				switch (userFilter.SortBy.ToLower())
				{
					case "username":
						query = userFilter.SortAscending ? query.OrderBy(e => e.UserName) : query.OrderByDescending(e => e.UserName);
						break;
					case "email":
						query = userFilter.SortAscending ? query.OrderBy(e => e.Email) : query.OrderByDescending(e => e.Email);
						break;
					case "phonenumber":
						query = userFilter.SortAscending ? query.OrderBy(e => e.PhoneNumber) : query.OrderByDescending(e => e.PhoneNumber);
						break;
					case "emailconfirmed":
						query = userFilter.SortAscending ? query.OrderBy(e => e.EmailConfirmed) : query.OrderByDescending(e => e.EmailConfirmed);
						break;
					case "lockoutenabled":
						query = userFilter.SortAscending ? query.OrderBy(e => e.LockoutEnabled) : query.OrderByDescending(e => e.LockoutEnabled);
						break;
					default:
						query = userFilter.SortAscending ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id);
						break;
				}
			}

			query = query.Skip((userFilter.PageNumber - 1) * userFilter.PageSize).Take(userFilter.PageSize);
			var result = await query.ToListAsync();
			return (result, totalCount);
		}

		public async Task<ConfirmationResponseDto> UnLOckedUsers(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new Exception();
			//throw new NotFoundException($"User with ID '{userId}' not found.");

			var isLockedOut = await userManager.IsLockedOutAsync(user);
			if (!isLockedOut)
				throw new Exception();
			//throw new BadRequestException($"User '{user.UserName}' is not currently locked out.");

			var unlockResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(-1));
			if (!unlockResult.Succeeded)
			{
				var errors = string.Join(", ", unlockResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"Failed to unlock user: {errors}");
			}

			var resetResult = await userManager.ResetAccessFailedCountAsync(user);
			if (!resetResult.Succeeded)
			{
				var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"User unlocked but failed to reset access failed count: {errors}");
			}
			user.LastUpdateOn = DateTime.Now;
			var updateResult = await userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"User unlocked but failed to reset access failed count: {errors}");
			}

			var responseDto = new ConfirmationResponseDto()
			{
				Message = $"User '{user.UserName}' has been successfully unlocked.",
				status = ConfirmationStatus.Updated
			};

			return responseDto;
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteUserAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new Exception();
			//throw new NotFoundException($"User with ID '{userId}' not found.");

			user.IsDeleted =!user.IsDeleted;
			user.LastUpdateOn = DateTime.Now;
			var updateResult = await userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"Failed to delete user: {errors}");
			}

			var responseDto = new ConfirmationResponseDto()
			{
				Message = $"User '{user.UserName}' has been successfully deleted.",
				status = ConfirmationStatus.SoftDeleted
			};

			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}
	}
}





