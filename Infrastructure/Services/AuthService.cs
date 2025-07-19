using Application.DTO_s.AuthenticationDto_s;
using Application.DTO_s.SupplierDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.AuthenticationResponse;
using Application.ResponseDTO_s.ProductResponse;
using Azure.Core;
using Infrastructure.Enum;
using Infrastructure.InternalInterfaces;
using Infrastructure.Mappings;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Services
{
	public class AuthService : IAuthService
	{
		private readonly ITokenService tokenService;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		public AuthService(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> roleManager
						  , ITokenService tokenService)
		{
			this.userManager = UserManager;
			this.roleManager = roleManager;
			this.tokenService = tokenService;	
		}
		public async Task<ApiResponse<AuthenticationResponseDto>> CreateUserAsync(CreateUserDto dto)
		{
			var existingUser = await userManager.FindByNameAsync(dto.UserName);
			if (existingUser != null)
				return ApiResponse<AuthenticationResponseDto>.Failuer(404, $"Username '{dto.UserName}' already exists.");

			var existingEmail = await userManager.FindByEmailAsync(dto.Email);
			if (existingEmail != null)
				return ApiResponse<AuthenticationResponseDto>.Failuer(404, $"Email '{dto.Email}' already exists.");


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
					return ApiResponse<AuthenticationResponseDto>.ValidationError($"The following role IDs do not exist: {string.Join(", ", invalidRoles)}");
			}

			var newUser = new ApplicationUser
			{
				UserName = dto.UserName,
				Email = dto.Email,
				EmailConfirmed = true,
				PhoneNumber = dto.PhoneNumber,
				FullName = dto.FullName,
				CreateOn = DateTime.Now,
				ProfileImage=""
			};

			var createResult = await userManager.CreateAsync(newUser, dto.Password);
			if (!createResult.Succeeded)
			{
				var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
				throw new Exception($"Failed to create user: {errors}");
			}

			if (validRoles.Any())
			{
				var addRolesResult = await userManager.AddToRolesAsync(newUser, validRoles);
				if (!addRolesResult.Succeeded)
				{
					var roleErrors = string.Join(", ", addRolesResult.Errors.Select(e => e.Description));
					throw new Exception($"User created but failed to assign roles: {roleErrors}");
				}
			}
			var Message = $"User '{dto.UserName}' created successfully. Assigned roles: {(validRoles.Any() ? string.Join(", ", validRoles.ToArray()) : "None")}";
			var responseDto = newUser.ToResponseDto(validRoles.ToArray());
			return ApiResponse<AuthenticationResponseDto>.Success(responseDto, 201, Message);
		}

		public async Task<ApiResponse<SignInResponseDto>> SignInAsync(SignInDto SignInDto)
		{
			var responseDto = new SignInResponseDto();
			var user = await userManager.FindByEmailAsync(SignInDto.Email);
			if (user is null)
				return ApiResponse<SignInResponseDto>.Unauthorized("Invalid email or password.");

			var passwordValid = await userManager.CheckPasswordAsync(user, SignInDto.Password);
			if (!passwordValid)
				return ApiResponse<SignInResponseDto>.Unauthorized("Invalid email or password.");

			var isLockedOut = await userManager.IsLockedOutAsync(user);
			if (isLockedOut)
				return ApiResponse<SignInResponseDto>.Unauthorized("User account is locked out.");

			var userRoles = await userManager.GetRolesAsync(user);

			var token = await tokenService.GenerateJwtToken(user, userRoles.ToArray());


			// i want to check if this user have active token  
			if (user.RefreshTokens.Any(e => e.IsActive))
			{
				var activeRefreshToken = user.RefreshTokens.FirstOrDefault(e => e.IsActive); /////ده معناه ان المستخدم هيبا عنده ريفريش توكن واحد بس اكتف مش اكتر من واحد
				responseDto.RefreshToken = activeRefreshToken.Token;
				responseDto.RefreshTokenExpiration=activeRefreshToken.Expires;
			}
			else
			{
				var tokens = tokenService.GenerateRefreshToken();
				var refreshToken = await tokenService.SaveRefreshTokenAsync(user, tokens, "123"); ////edit ip address
				responseDto.RefreshToken = refreshToken.Token;
				responseDto.RefreshTokenExpiration = refreshToken.Expires;
			}
			responseDto.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

			return ApiResponse<SignInResponseDto>.Success(responseDto, 200, "Sign in successful");
		}

		public async Task RevokeTokenAsync(string refreshToken) 
		{
			await tokenService.RevokeRefreshTokenAsync(refreshToken);
		}

		public async Task<ApiResponse<SignInResponseDto>> RefreshTokenAsync(string token, HttpContext httpContext)
		{
			var dto = await tokenService.RefreshTokenAsync(token, httpContext);

			return ApiResponse<SignInResponseDto>.Success(dto, 200);
		}

		public async Task<bool> IsUserNameUniqueAsync(string userName) =>
			       await userManager.Users.AsNoTracking().AnyAsync(e=>e.UserName.ToLower() == userName.ToLower());

		public async Task<bool> IsEmailUniqueAsync(string email) =>
			      await userManager.Users.AsNoTracking().AnyAsync(e => e.Email.ToLower() == email.ToLower());

		public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber) =>
			      await userManager.Users.AsNoTracking().AnyAsync(e => e.PhoneNumber == phoneNumber);
		public async Task<bool> IsValidRolesIdAsync(string[] RolesId)
		{
			int validCount = await roleManager.Roles.AsNoTracking().CountAsync(r => RolesId.Contains(r.Id));
			return validCount == RolesId.Length;
		}

		public async Task<bool> VerifySignInCredentialsAsync(string Email, string password)
		{
			try
			{
				var user = await userManager.FindByEmailAsync(Email);

				if (user is null)
					return false;

				return await userManager.CheckPasswordAsync(user, password);
			}
			catch
			{   //any excption return false only "on calme" 
				return false;
			}
		}
		public async Task<string> CreateSupplierAsync(CreateSupplierDto dto)
		{
			var supplierUser = new ApplicationUser
			{
				UserName = dto.UserName,
				Email = dto.Email,
				EmailConfirmed = true,
				PhoneNumber = dto.PhoneNumber,
				FullName = dto.CompanyName,
				CreateOn = DateTime.Now,
				ProfileImage=""
			};
			var createResult = await userManager.CreateAsync(supplierUser, dto.Password);
			if (!createResult.Succeeded)
			{
				var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
				throw new Exception($"Failed to create user: {errors}");
			}

			var addRolesResult = await userManager.AddToRoleAsync(supplierUser, $"{AppRoles.Supplier}");
			if (!addRolesResult.Succeeded)
			{
				var roleErrors = string.Join(", ", addRolesResult.Errors.Select(e => e.Description));
				throw new Exception($"User created but failed to assign roles: {roleErrors}");
			}
			return supplierUser.Id;
		}
	}
}
