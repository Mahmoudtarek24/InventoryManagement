using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
	public class LocalImageStorageService : IImageStorageService
	{
		private readonly IWebHostEnvironment webHostEnvironment;
		public LocalImageStorageService(IWebHostEnvironment webHostEnvironment)
		{
			this.webHostEnvironment = webHostEnvironment;
		}

		public async Task<(bool, string)> UploadImage(IFormFile image, string folderName)
		{
			try
			{
				var imageRoot = Path.Combine(webHostEnvironment.WebRootPath, "Images", folderName);
				if (!Directory.Exists(imageRoot))
					Directory.CreateDirectory(imageRoot);

				var ImageFileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
				var filePath = Path.Combine(imageRoot, ImageFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				{
					await image.CopyToAsync(fileStream);
				}
				return (true , ImageFileName);
			}
			catch
			{
				return (false,null);
			}
		}

		public bool DeleteImage(string ImagePath)/////               products/product123.jpg
		{
			try
			{
				var relativePath = Path.Combine("Images", ImagePath);
				var fullPath = Path.Combine(webHostEnvironment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
				// to get appropriate character for operating system 
				if (File.Exists(fullPath))
				{
					File.Delete(fullPath);
					return true;
				}
				return false;
			}
			catch
			{
				return false;	
			}
		}
	}
}
