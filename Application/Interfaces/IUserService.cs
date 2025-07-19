using Application.DTO_s;
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
	public interface IUserService //application user  
	{
		Task<ApiResponse<AuthenticationResponseDto>> FindByIdAsync(string userId);
		Task<ApiResponse<AuthenticationResponseDto>> FindByEmailAsync(string email);
		Task<PagedResponse<List<AuthenticationResponseDto>>> GetUsersWithPaginationAsync(ApplicationUserQueryParameters query, string route);
		Task<ApiResponse<ConfirmationResponseDto>> UnLOckedUsers(string userId);
		Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteUserAsync(string userId);
		Task<ApiResponse<UpdateUserRespondDto>> UpdateProfileAsync(UpdateUserProfileDto dto);
	}
}
