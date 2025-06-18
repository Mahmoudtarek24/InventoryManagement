using Application.DTO_s.CategoryDto_s;
using Domain.Interface;
using EducationPlatform.Constants;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.validations.Category
{
	public class CategoryBaseDtoValidator<T> :AbstractValidator<T> where T : CategoryBaseDto
	{
		private readonly IUnitOfWork unitOfWork;
		public CategoryBaseDtoValidator(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;

			RuleFor(e => e.Name).NotEmpty().WithMessage(string.Format(ValidationMessages.RequiredField, "Name"))
							  .Length(2, 50)
							  .WithMessage(string.Format(ValidationMessages.StringLent, "Category Name", 2, 50))
							  .MustAsync(BeUniqueNameAsync)
							  .WithMessage(string.Format(ValidationMessages.Duplicated, "Category Name"));

			RuleFor(e => e.Description).MaximumLength(150)
							  .WithMessage(string.Format(ValidationMessages.MaxLength, "Description", 150));

			//RuleFor(e=>e.DisplayOrder).MustAsync(IsTakenDisplayOrderNumber)
			//	              .WithMessage( async (model, displayOrder, ct) => //(createDto,Properity(display order), cansolation )
			//				  {
			//					  var taken = await unitOfWork.CategoryRepository.IsDisplayOrderTakenAsync();
			//					  return $"Display order '{displayOrder}' is already taken.  Number Taken {string.Join(", ", taken)}";
			//				  });
			//[1,3,5] , =>[2,4,5]
			RuleFor(e => e.DisplayOrder)
		   .MustAsync(BeUniqueDisplayOrderAsync).WithMessage("Display order already exists.");
		}
		private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
		{
			return !await unitOfWork.CategoryRepository.IsCategoryNameUniqueAsync(name);
		}
		private async Task<bool> BeUniqueDisplayOrderAsync(int displayOrder, CancellationToken cancellationToken)
		{
			return !await unitOfWork.CategoryRepository.IsDisplayOrderTakenAsync(displayOrder);
		}
	}
}
