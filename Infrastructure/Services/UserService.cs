using Application.Constants.Enum;
using Application.DTO_s;
using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.AuthenticationResponse;
using Application.Services;
using Domain.Parameters;
using Infrastructure.Enum;
using Infrastructure.InternalInterfaces;
using Infrastructure.Mappings;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IImageStorageService imageStorageService;
		private readonly ITokenService tokenService;
		private readonly IUriService uriService;	
		public UserService(UserManager<ApplicationUser> UserManager , IImageStorageService imageStorageService
			              ,ITokenService tokenService,IUriService uriService)
		{
			this.userManager = UserManager;
			this.imageStorageService = imageStorageService;
			this.tokenService = tokenService;	
			this.uriService = uriService;	
		}
		public async Task<ApiResponse<AuthenticationResponseDto>> FindByIdAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user is null)
			return ApiResponse<AuthenticationResponseDto>.Failuer(404, $"User with ID '{userId}' not found.");

			var userRoles = await userManager.GetRolesAsync(user);

			var responseDto = user.ToResponseDto(userRoles.ToArray());
			responseDto.ProfileImageUrl = $@"{uriService.GetBaseUri()}Images/{user.ProfileImage}";

			return ApiResponse<AuthenticationResponseDto>.Success(responseDto, 200);
		}
		public async Task<ApiResponse<AuthenticationResponseDto>> FindByEmailAsync(string email)
		{
			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
				return ApiResponse<AuthenticationResponseDto>.Failuer(404, $"User with email '{email}' not found.");

			// Get user roles
			var userRoles = await userManager.GetRolesAsync(user);

			var responseDto = user.ToResponseDto(userRoles.ToArray());
			return ApiResponse<AuthenticationResponseDto>.Success(responseDto, 200);
		}

		public async Task<PagedResponse<List<AuthenticationResponseDto>>> GetUsersWithPaginationAsync(ApplicationUserQueryParameters query, string route)
		{
			var parameter = new BaseFilter()
			{
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
				searchTearm = query.searchTearm,
				SortAscending = query.SortAscending,
				SortBy = query.UserOrdering.ToString(),	
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
				  e.UserName.ToLower().Contains(userFilter.searchTearm.ToLower()) ||
				  e.Email.ToLower().Contains(userFilter.searchTearm.ToLower()) ||
				  (e.PhoneNumber != null && e.PhoneNumber.ToString().Contains(userFilter.searchTearm)));

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

		public async Task<ApiResponse<ConfirmationResponseDto>> UnLOckedUsers(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"User with ID '{userId}' not found.");

			var isLockedOut = await userManager.IsLockedOutAsync(user);
			if (!isLockedOut)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"User '{user.UserName}' is not currently locked out.");

			var unlockResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(-1));
			if (!unlockResult.Succeeded)
			{
				var errors = string.Join(", ", unlockResult.Errors.Select(e => e.Description));
				//throw new InternalServerErrorException($"Failed to unlock user: {errors}");////////////////////////////
			}

			var resetResult = await userManager.ResetAccessFailedCountAsync(user);
			if (!resetResult.Succeeded)
			{
				var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
				//throw new InternalServerErrorException($"User unlocked but failed to reset access failed count: {errors}");///////////////////////
			}
			user.LastUpdateOn = DateTime.Now;
			var updateResult = await userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"User unlocked but failed to reset access failed count: {errors}");/////////////////
			}

			var responseDto = new ConfirmationResponseDto()
			{
				Message = $"User '{user.UserName}' has been successfully unlocked.",
				status = ConfirmationStatus.Updated
			};

			return ApiResponse<ConfirmationResponseDto>.Success(responseDto,200);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteUserAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"User with ID '{userId}' not found.");

			user.IsDeleted =!user.IsDeleted;
			user.LastUpdateOn = DateTime.Now;
			var updateResult = await userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"Failed to delete user: {errors}");/////////////
			}

			var responseDto = new ConfirmationResponseDto()
			{
				Message = $"User '{user.UserName}' has been successfully deleted.",
				status = ConfirmationStatus.SoftDeleted
			};

			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}

		public async Task<ApiResponse<UpdateUserRespondDto>> UpdateProfileAsync(UpdateUserProfileDto dto)
		{
			var existingUser = await userManager.FindByIdAsync(dto.UserId);
			if (existingUser == null)
				return ApiResponse<UpdateUserRespondDto>.Failuer(404, $"User with ID '{dto.UserId}' not found.");

			if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != existingUser.UserName)
			{
				var userNameExists = await userManager.FindByNameAsync(dto.UserName);
				if (userNameExists != null)
					return ApiResponse<UpdateUserRespondDto>.ValidationError($"Username '{dto.UserName}' already exists.");
			}

			if (!string.IsNullOrEmpty(dto.Email) && dto.Email != existingUser.Email)
			{
				var emailExists = await userManager.FindByEmailAsync(dto.Email);
				if (emailExists != null)
					return ApiResponse<UpdateUserRespondDto>.ValidationError($"Username '{dto.UserName}' already exists.");
			}

			string imageFileName = null;
			if (dto.ImageFile != null)
			{
				var (uploadSuccess, fileName) = await imageStorageService.UploadImage(dto.ImageFile, ImageFolderName.User.ToString());
				if (!uploadSuccess)
					throw new Exception();
				//throw new InternalServerErrorException("Failed to upload profile image.");/////////////////

				if (!string.IsNullOrEmpty(existingUser.ProfileImage))
					imageStorageService.DeleteImage(existingUser.ProfileImage);

				imageFileName = $"{ImageFolderName.User.ToString()}/{fileName}";  //  products/product123.jpg will store on database
			}
			bool hasChange = default;

			if (!string.IsNullOrEmpty(dto.UserName))
			{
				existingUser.UserName = dto.UserName;
				hasChange = true;
			}

			if (!string.IsNullOrEmpty(dto.Email))
			{
				existingUser.Email = dto.Email;
				hasChange = true;
			}
			if (!string.IsNullOrEmpty(dto.FullName))
				existingUser.FullName = dto.FullName;

			if (imageFileName != null)
				existingUser.ProfileImage = imageFileName;

			var updateResult = await userManager.UpdateAsync(existingUser);
			if (!updateResult.Succeeded)
			{
				var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"Failed to update user profile: {errors}");///////////////
			}

			var BaseUri = uriService.GetBaseUri();
			var responseDto = new UpdateUserRespondDto
			{
				Email=existingUser.Email,
				FullName=existingUser.FullName,
				UserId=existingUser.Id,
				UserName=existingUser.UserName,
				ProfileImageUrl = $@"{BaseUri}Images/{existingUser.ProfileImage}" //////////////////////////
			};

			var userRoles = await userManager.GetRolesAsync(existingUser);
			if (hasChange) 
			{ 
				var Token=await tokenService.GenerateJwtToken(existingUser, userRoles.ToArray());	
			    responseDto.AccessToken = new JwtSecurityTokenHandler().WriteToken(Token);
			}
			var message = $"Profile for user '{existingUser.UserName}' updated successfully.";
			return ApiResponse<UpdateUserRespondDto>.Success(responseDto, 200, message);
		}
	}
}





