using Application.DTO_s;
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
	public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
	{
		public UpdateCategoryDtoValidator(IUnitOfWork unitOfWork)
		{
			Include(new CategoryBaseDtoValidator<UpdateCategoryDto>(unitOfWork));

			RuleFor(p => p.CategoryId)
					.GreaterThan(0).WithMessage(string.Format(ValidationMessages.IdValue, "Category Id"));
		}
	}
}