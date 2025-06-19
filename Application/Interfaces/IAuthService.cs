using Application.DTO_s.AuthenticationDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.AuthenticationResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<AuthenticationResponseDto>> CreateUserAsync(CreateUserDto dto);
		Task<ApiResponse<SignInResponseDto>> SignInAsync(SignInDto SignInDto);
		//Task SignOutAsync();
		//Task<AuthenticationResponse> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword);
		//Task<AuthenticationResponse> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
		//Task<TokenResponse> GeneratePasswordResetTokenAsync(string email);
		//Task<ApplicationUserDto> GetCurrentUserAsync(ClaimsPrincipal user);
		//Task<TokenResponse> GenerateEmailConfirmationAsync(ClaimsPrincipal user);
		//Task<TokenResponse> GenerateEmailChangeAsync(ClaimsPrincipal user, string newEmail);
		//Task<AuthenticationResponse> ConfirmEmailAsync(EmailConfirmationRequest emailConfirmationRequest);
		//Task RefreshSignInAsync(ClaimsPrincipal user);
	}
}
