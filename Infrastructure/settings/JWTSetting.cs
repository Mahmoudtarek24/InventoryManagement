﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.settings
{
	public class JWTSetting
	{
		public string Key { get; set; }	
		public string Issuer { get; set; }	
		public string Audience { get; set; }	
		public int DurationInDays { get; set; }	
		public int RefreshTokenExpirationInDays { get; set; }	
	}
}
