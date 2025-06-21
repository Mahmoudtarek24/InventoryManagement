using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using Domain.Interface;
using EducationPlatform.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.validations.Authentication
{
	public class CreateUserDtoValidator :AbstractValidator<CreateUserDto>
	{
		private readonly IAuthService authService;	
		public CreateUserDtoValidator(IAuthService authService)
		{
			this.authService = authService;	

			RuleFor(e=>e.UserName).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "UserName"))
				   .Length(6,50).WithMessage(string.Format(ValidationMessages.StringLent, "UserName", 2, 50))
				   .MustAsync(BeUniqueUserNameAsync)
							  .WithMessage(string.Format(ValidationMessages.Duplicated, "userName"));

			RuleFor(e => e.Email).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Email"))
				   .MaximumLength(50).WithMessage(string.Format(ValidationMessages.MaxLength, "Email", 50))
				   .Must(IsValidEmailFormat).WithMessage(string.Format(ValidationMessages.EmailAddress))
				   .MustAsync(BeUniqueEmailAsync)
							  .WithMessage(string.Format(ValidationMessages.Duplicated, "Email"));

			RuleFor(e => e.FullName).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Full Name"))
				   .Length(3, 60).WithMessage(string.Format(ValidationMessages.StringLent, "Full Name", 3, 60));

			RuleFor(e=>e.RoleId).MustAsync(IsValidRoleIdAsync).WithMessage(ValidationMessages.InvalidRole);

			RuleFor(e => e.Password)
				  .NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Password"))
				  .MinimumLength(8).WithMessage(string.Format(ValidationMessages.MinLength, "Password", 8))
				  .MaximumLength(25).WithMessage(string.Format(ValidationMessages.MaxLength, "Password", 25))
				  .Must(IsValidPasswordFormat).WithMessage(ValidationMessages.WeakPassword);

	
			RuleFor(e => e.PhoneNumber)
				 .NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Phone Number"))
				 .MustAsync(BeUniquePhoneNumberAsync)
				 .WithMessage(string.Format(ValidationMessages.Duplicated, "Phone Number"));


		}

		private async Task<bool> BeUniqueUserNameAsync(string userName,CancellationToken cancellationToken) =>
			           !await authService.IsUserNameUniqueAsync(userName);
		private async Task<bool> BeUniqueEmailAsync(string userName, CancellationToken cancellationToken) =>
				       !await authService.IsEmailUniqueAsync(userName);

		private async Task<bool> IsValidRoleIdAsync(string[] RolesId, CancellationToken cancellationToken) =>
					   !await authService.IsValidRolesIdAsync(RolesId);
		private async Task<bool> BeUniquePhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken) =>
			          !await authService.IsPhoneNumberUniqueAsync(phoneNumber);


		private bool IsValidEmailFormat(string email)
		{
			if(string.IsNullOrEmpty(email)) return false;	

			if(!email.Contains('@')) return false;

			var knownDomains = new[] { "gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "mail.com" };
			var correctFormat = knownDomains.Any(e => email.EndsWith(e));
			return correctFormat;
		}
		private bool IsValidPasswordFormat(string password)
		{
			if (string.IsNullOrEmpty(password)) return false;

			bool hasUpperCase = password.Any(char.IsUpper);
			bool hasLowerCase = password.Any(char.IsLower);
			bool hasDigit = password.Any(char.IsDigit);
			bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

			return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
		}
	}
}
