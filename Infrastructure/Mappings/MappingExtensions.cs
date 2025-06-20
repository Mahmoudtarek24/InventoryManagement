using Application.ResponseDTO_s;
using Domain.Entity;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mappings
{
	public static class MappingExtensions
	{
		public static AuthenticationResponseDto ToResponseDto(this ApplicationUser user, string[] roles=null)
		{
			if (user is null)
				return null;

			var response = new AuthenticationResponseDto()
			{
				Email = user.Email,
				FullName = user.FullName,
				Id = user.Id,
				UserName = user.UserName,
				PhoneNumber = user.PhoneNumber,
			};

			if (roles != null)
				response.Roles = roles;
			return response;
		}
	}
}

