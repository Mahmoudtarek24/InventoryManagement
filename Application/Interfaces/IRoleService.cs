using Application.DTO_s.RolesDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.RoleResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IRoleService
	{
	    Task<ApiResponse<List<RoleResponseDto>>> GetRoleListAsync();
		Task<ApiResponse<ConfirmationResponseDto>> UpdateUserRolesAsync(UpdateUserRolesDto dto);
	}
}
