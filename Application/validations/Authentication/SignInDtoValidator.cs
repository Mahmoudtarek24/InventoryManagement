using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using EducationPlatform.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.validations.Authentication
{
	public class SignInDtoValidator : AbstractValidator<SignInDto>
	{
		private readonly IAuthService authService;
		public SignInDtoValidator(IAuthService authService)
		{ 
			this.authService = authService;
			RuleFor(e => e.Email)
			   .NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Email"))
			   .MaximumLength(50).WithMessage(string.Format(ValidationMessages.MaxLength, "Email", 50))
			   .Matches(RegexPatterns.Email).WithMessage(string.Format(ValidationMessages.EmailAddress))
			   .MustAsync(EmailExistsAsync).WithMessage(ValidationMessages.NotFoundEmail);

			RuleFor(e => e.Password)
				.NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Password"))
				.MinimumLength(8).WithMessage(string.Format(ValidationMessages.MinLength, "Password", 8))
				.MaximumLength(25).WithMessage(string.Format(ValidationMessages.MaxLength, "Password", 25));

			RuleFor(e => e)  ///her rule for object not specific properity that need to give it name """Credentials"""
				.MustAsync(ValidateCredentialsAsync) 
				.WithMessage(ValidationMessages.InvalidEmailOrPassword)
				.WithName("Credentials");
		}
		private async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken) =>
			         await authService.IsEmailUniqueAsync(email);

		private async Task<bool> ValidateCredentialsAsync(SignInDto signInDto, CancellationToken cancellationToken) =>
			   await authService.VerifySignInCredentialsAsync(signInDto.Email, signInDto.Password);
	}
}
