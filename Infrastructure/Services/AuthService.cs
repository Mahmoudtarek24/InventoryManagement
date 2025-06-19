using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.AuthenticationResponse;
using Infrastructure.Mappings;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		public AuthService(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> roleManager)
		{
			this.userManager = UserManager;
			this.roleManager = roleManager;
		}
		public async Task<ApiResponse<AuthenticationResponseDto>> CreateUserAsync(CreateUserDto dto)
		{
			var existingUser = await userManager.FindByNameAsync(dto.UserName);
			if (existingUser != null)
				throw new Exception();
			//throw new ConflictException($"Username '{dto.UserName}' already exists.");

			var existingEmail = await userManager.FindByEmailAsync(dto.Email);
			if (existingEmail != null)
				throw new Exception();
			//throw new ConflictException($"Email '{dto.Email}' already exists.");

			var invalidRoles = new List<string>();
			var validRoles = new List<string>();
			if (dto.RoleId != null && dto.RoleId.Any())
			{
				foreach (var roleId in dto.RoleId)
				{
					var role = await roleManager.FindByIdAsync(roleId);
					if (role == null)
						invalidRoles.Add(roleId);
					else
						validRoles.Add(role.Name);
				}

				if (invalidRoles.Any())
					throw new Exception();
				//throw new NotFoundException($"The following role IDs do not exist: {string.Join(", ", invalidRoles)}");
			}

			var newUser = new ApplicationUser
			{
				UserName = dto.UserName,
				Email = dto.Email,
				EmailConfirmed = true,
				PhoneNumber = dto.PhoneNumber,
				FullName = dto.FullName 
			};

			var createResult = await userManager.CreateAsync(newUser, dto.Password);
			if (!createResult.Succeeded)
			{
				var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
				throw new Exception();
				//throw new InternalServerErrorException($"Failed to create user: {errors}");
			}

			if (validRoles.Any())
			{
				var addRolesResult = await userManager.AddToRolesAsync(newUser, validRoles);
				if (!addRolesResult.Succeeded)
				{
					var roleErrors = string.Join(", ", addRolesResult.Errors.Select(e => e.Description));
					throw new Exception();
					//throw new InternalServerErrorException($"User created but failed to assign roles: {roleErrors}");
				}
			}
			var Message = $"User '{dto.UserName}' created successfully. Assigned roles: {(validRoles.Any() ? string.Join(", ", validRoles.ToArray()) : "None")}";
			var responseDto = newUser.ToResponseDto();
			responseDto.Roles = validRoles.ToArray();

			return ApiResponse<AuthenticationResponseDto>.Success(responseDto, 201, Message);
		}

		public async Task<ApiResponse<SignInResponseDto>> SignInAsync(SignInDto SignInDto)
		{
			var user = await userManager.FindByEmailAsync(SignInDto.Email);
			if (user == null)
				throw new Exception();
			//throw new UnauthorizedException("Invalid email or password.");

			var passwordValid = await userManager.CheckPasswordAsync(user, SignInDto.Password);
			if (!passwordValid)
				throw new Exception();
			//throw new UnauthorizedException("Invalid email or password.");

			var isLockedOut = await userManager.IsLockedOutAsync(user);
			if (isLockedOut)
				throw new Exception();
			//throw new UnauthorizedException("User account is locked out.");

			var userRoles = await userManager.GetRolesAsync(user);

			// Generate JWT token (you'll need to implement this based on your JWT configuration)
			//var token = GenerateJwtToken(user, userRoles);////////////////////////////////////////////////////////

			var responseDto = new SignInResponseDto()
			{
				//Token = token,
				Email = user.Email,
				UserId = user.Id,
			};

			return ApiResponse<SignInResponseDto>.Success(responseDto, 200, "Sign in successful");
		}
	}
}
