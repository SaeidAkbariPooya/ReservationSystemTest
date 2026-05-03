using FluentValidation;
using ReservationSystem.Core.DTOs;

namespace ReservationSystem.Core.Validators
{
    public class ReportRequestValidator : AbstractValidator<ReportRequest>
    {
        public ReportRequestValidator()
        {
            RuleFor(x => x.ResourceId).GreaterThan(0);
            RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate);
        }
    }
}
