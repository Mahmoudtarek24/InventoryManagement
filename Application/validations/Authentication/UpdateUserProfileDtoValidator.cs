using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using EducationPlatform.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.validations.Authentication
{
	public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
	{
		private readonly IAuthService authService;
		public UpdateUserProfileDtoValidator(IAuthService authService)
		{
			this.authService = authService;

			//// UserId is required for update operations
			//RuleFor(e => e.UserId).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "User ID"))
			//	   .MustAsync(UserExistsAsync).WithMessage("User not found");

			//// UserName validation - check uniqueness excluding current user
			//RuleFor(e => e.UserName).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "UserName"))
			//	   .Length(6, 50).WithMessage(string.Format(ValidationMessages.StringLent, "UserName", 6, 50))
			//	   .MustAsync(BeUniqueUserNameForUpdateAsync)
			//				  .WithMessage(string.Format(ValidationMessages.Duplicated, "UserName"));

			//// Email validation - check uniqueness excluding current user
			//RuleFor(e => e.Email).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Email"))
			//	   .MaximumLength(50).WithMessage(string.Format(ValidationMessages.MaxLength, "Email", 50))
			//	   .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage(string.Format(ValidationMessages.EmailAddress))
			//	   .MustAsync(BeUniqueEmailForUpdateAsync)
			//				  .WithMessage(string.Format(ValidationMessages.Duplicated, "Email"));

			//// FullName validation
			//RuleFor(e => e.FullName).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Full Name"))
			//	   .Length(3, 60).WithMessage(string.Format(ValidationMessages.StringLent, "Full Name", 3, 60));

			//// ImageFile validation (optional)
			//RuleFor(e => e.ImageFile).Must(BeValidImageFile).WithMessage("Invalid image file format or size")
			//	   .When(e => e.ImageFile != null);
		}
		//private async Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken) =>
		//	   await authService.UserExistsAsync(userId);

		//// Check username uniqueness excluding current user
		//private async Task<bool> BeUniqueUserNameForUpdateAsync(UpdateUserProfileDto dto, string userName, CancellationToken cancellationToken) =>
		//		   await authService.IsUserNameUniqueForUpdateAsync(userName, dto.UserId);

		//// Check email uniqueness excluding current user
		//private async Task<bool> BeUniqueEmailForUpdateAsync(UpdateUserProfileDto dto, string email, CancellationToken cancellationToken) =>
		//		   await authService.IsEmailUniqueForUpdateAsync(email, dto.UserId);

		private bool BeValidImageFile(IFormFile imageFile)
		{
			if (imageFile == null) return true; // Optional field

			// Check file size (e.g., max 5MB)
			const int maxSizeInBytes = 5 * 1024 * 1024;
			if (imageFile.Length > maxSizeInBytes) return false;

			// Check file extension
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
			var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
			if (!allowedExtensions.Contains(fileExtension)) return false;

			// Check content type
			var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp" };
			if (!allowedContentTypes.Contains(imageFile.ContentType.ToLowerInvariant())) return false;

			return true;
		}
	}
}
