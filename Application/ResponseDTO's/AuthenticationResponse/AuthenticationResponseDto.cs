﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s
{
	public class AuthenticationResponseDto
	{
		public string Id { get; set; }
		public string FullName {  get; set; }	
		public string Email {  get; set; }	
		public string UserName {  get; set; }
		public string PhoneNumber { get; set; }	
		public string[] Roles { get; set; }
		public string ProfileImageUrl { get; set; }
	}
}
