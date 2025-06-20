using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.AuthenticationResponse;
using Infrastructure.Identity_Models;
using Infrastructure.Mappings;
using Infrastructure.Models;
using Infrastructure.settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly JWTSetting jWTSetting;
		public AuthService(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> roleManager
						  , IOptions<JWTSetting> options)
		{
			this.userManager = UserManager;
			this.roleManager = roleManager;
			this.jWTSetting = options.Value;
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
				FullName = dto.FullName,
				CreateOn = DateTime.Now,
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
			var responseDto = newUser.ToResponseDto(validRoles.ToArray());
			return ApiResponse<AuthenticationResponseDto>.Success(responseDto, 201, Message);
		}

		public async Task<ApiResponse<SignInResponseDto>> SignInAsync(SignInDto SignInDto)
		{
			var responseDto = new SignInResponseDto();
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

			var token = await GenerateJwtToken(user, userRoles.ToArray());


			// i want to check if this user have active token  
			if (user.RefreshTokens.Any(e => e.IsActive))
			{
				var activeRefreshToken = user.RefreshTokens.FirstOrDefault(e => e.IsActive); /////ده معناه ان المستخدم هيبا عنده ريفريش توكن واحد بس اكتف مش اكتر من واحد
				responseDto.RefreshToken = activeRefreshToken.Token;
				responseDto.RefreshTokenExpiration=activeRefreshToken.Expires;
			}
			else
			{
				var tokens = GenerateRefreshToken();
				var refreshToken = await SaveRefreshTokenAsync(user, tokens, "123"); ////edit ip address
				responseDto.RefreshToken = refreshToken.Token;
				responseDto.RefreshTokenExpiration = refreshToken.Expires;
			}
			responseDto.Token = new JwtSecurityTokenHandler().WriteToken(token);
			responseDto.Email = user.Email;
			responseDto.UserId = user.Id;

			return ApiResponse<SignInResponseDto>.Success(responseDto, 200, "Sign in successful");
		}
		public async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user, string[] userRoles)
		{
			List<Claim> claims = new List<Claim>();

			if (userRoles.Length > 0)
			{
				foreach (var role in userRoles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}
			}
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
			claims.Add(new Claim(ClaimTypes.Email, user.Email));
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

			SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTSetting.Key));
			SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
			return new JwtSecurityToken(
				claims: claims,
				issuer: jWTSetting.Issuer,
				audience: jWTSetting.Audience,
				expires: DateTime.Now.AddDays(jWTSetting.DurationInDays),
				signingCredentials: signingCredentials);
		}
		public string GenerateRefreshToken()
		{
			var randomBytes = new byte[64];
			using var generator = new RNGCryptoServiceProvider();
			generator.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}
		public async Task<RefreshToken> SaveRefreshTokenAsync(ApplicationUser user, string token, string ipAddress)
		{
			var refreshToken = new RefreshToken
			{
				CreatedByIp = ipAddress,
				CreateOn = DateTime.Now,
				Expires = DateTime.Now.AddDays(jWTSetting.RefreshTokenExpirationInDays),
				Token = GenerateRefreshToken(),
			};
			user.RefreshTokens.Add(refreshToken);	
			await userManager.UpdateAsync(user);
			return refreshToken;
		}
		public void SetRefreshTokenCookie(HttpResponse response, string refreshToken)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				IsEssential = true,
				Secure = true,
				Expires = DateTime.Now.AddDays(jWTSetting.RefreshTokenExpirationInDays).ToLocalTime(),
			};
			response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
		}
	}
}
