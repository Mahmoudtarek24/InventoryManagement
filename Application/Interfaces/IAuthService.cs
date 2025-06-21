using Application.DTO_s.AuthenticationDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.AuthenticationResponse;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IAuthService //authentication 
	{
		Task<ApiResponse<AuthenticationResponseDto>> CreateUserAsync(CreateUserDto dto);
		Task<ApiResponse<SignInResponseDto>> SignInAsync(SignInDto SignInDto);
		Task<ApiResponse<SignInResponseDto>> RefreshTokenAsync(string token, HttpContext httpContext);
		Task RevokeTokenAsync(string token);
		Task<bool> IsUserNameUniqueAsync(string userName);
		Task<bool> IsEmailUniqueAsync(string email);
		Task<bool> IsValidRolesIdAsync(string[] RolesId);
		Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
		Task<bool> VerifySignInCredentialsAsync(string Email, string Password);
	}
}
