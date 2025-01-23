using App.Core.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Validators
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(u => u.email).NotEmpty().WithMessage("Email cannot be empty")
                .EmailAddress();

            RuleFor(u => u.password).NotEmpty().WithMessage("Password must not be empty")
                .MinimumLength(8).WithMessage("Password Must be aleast 8 characters long");


            RuleFor(u=> u.firstName).NotEmpty().WithMessage("First Name is required")
               .MaximumLength(25).WithMessage("First Name should not exceed 25 character")
               .MinimumLength(2).WithMessage("Last Name should contain more than 2 character");

            RuleFor(u=> u.lastName).NotEmpty().WithMessage("Last Name cannot be empty")
                .MaximumLength(25).WithMessage("Last Name should not exceed 25 character")
                .MinimumLength(2).WithMessage("Last Name should contain more than 2 character");

        }
    }
}
