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
	public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
	{
		public CreateCategoryDtoValidator(IUnitOfWork unitOfWork)
		{
			Include(new CategoryBaseDtoValidator<CreateCategoryDto>(unitOfWork));
		}
	}
}
