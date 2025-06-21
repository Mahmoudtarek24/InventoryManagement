using Application.DTO_s.ProductDto_s;
using Domain.Interface;
using EducationPlatform.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.validations.Product
{
	public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
	{
		private readonly IUnitOfWork unitOfWork;
		public CreateProductDtoValidator(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;

			RuleFor(e => e.CategoryId).MustAsync(CategoryExistsAsync).WithMessage("Category does not exist.");

			RuleFor(e => e.Name).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Name"))
				.Length(2, 150).WithMessage(string.Format(ValidationMessages.StringLent, "Product Name", 2, 150))
				.MustAsync((dto, name, ct) => {
					return IsProductNameUniqueWithinCategoryAsync(name, dto.CategoryId, ct);
				}).WithMessage(ValidationMessages.DuplicatProductName);

			RuleFor(e => e.Price).GreaterThan(0).WithMessage(ValidationMessages.PositivePrice);

		}

		private async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken cancellationToken) =>
			!await unitOfWork.CategoryRepository.IsValidCategoryIdAsync(categoryId);

		private async Task<bool> IsProductNameUniqueWithinCategoryAsync(string productName, int categoryId, CancellationToken cancellationToken) =>
			!await unitOfWork.ProductRepository.IsDuplicateProductNameInCategoryAsync(productName, categoryId);
	}
}