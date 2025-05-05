using FluentValidation;
using Fly.Flight.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Application.Features.Validators
{
    public class SearchFlightsQueryValidator:AbstractValidator<SearchFlightsQuery>
    {
        public SearchFlightsQueryValidator()
        {
            RuleFor(x => x.Departure)
            .Matches("^[a-zA-Z ]+$").When(x=>!string.IsNullOrEmpty(x.Departure))
            .WithMessage("Departure must be alphabetic.");

            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("Destination is required.")
                .Matches("^[a-zA-Z ]+$").
                When(x => !string.IsNullOrEmpty(x.Destination))
                .WithMessage("Destination must be alphabetic.");

            RuleFor(x => x.StartDate)
                .Must(BeAValidDate).When(x => x.StartDate.HasValue)
                .WithMessage("StartDate must be a valid date.");

            RuleFor(x => x.EndDate)
                .Must(BeAValidDate).When(x => x.EndDate.HasValue)
                .WithMessage("EndDate must be a valid date.");

            RuleFor(x => x)
                .Must(x => x.StartDate <= x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("StartDate must be before or equal to EndDate.");

            RuleFor(x => x.MinPrice)
                .GreaterThan(0).When(x => x.MinPrice.HasValue)
                .WithMessage("PriceMin must be greater than 0.");

            RuleFor(x => x.MaxPrice)
                .GreaterThan(0).When(x => x.MaxPrice.HasValue)
                .WithMessage("PriceMax must be greater than 0.");

            RuleFor(x => x)
                .Must(x => x.MinPrice <= x.MaxPrice)
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
                .WithMessage("PriceMin must be less than or equal to PriceMax.");
        }

        private bool BeAValidDate(DateTime? date) =>
            date.HasValue && date.Value != default;
    }
}
