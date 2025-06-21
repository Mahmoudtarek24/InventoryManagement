using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IImageStorageService
	{
		Task<(bool,string)> UploadImage(IFormFile image, string folderPath);
		bool DeleteImage(string ImagePath);
	}
}
