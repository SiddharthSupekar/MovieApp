using App.Core.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Validators
{
    public class MovieValidator : AbstractValidator<MovieDto>
    {
        public MovieValidator()
        {
            RuleFor(m => m.movieName).NotEmpty().WithMessage("Movie name cannot be empty");

            RuleFor(m => m.releaseDate).NotEmpty().WithMessage("Release date must not be empty")
                .Matches(@"\d{4}$").LessThanOrEqualTo(DateTime.UtcNow.Year.ToString()).WithMessage("Date cannot be greater than today's date");
        }
    }
}
