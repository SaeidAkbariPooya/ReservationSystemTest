using FluentValidation;
using ReservationSystem.Core.DTOs;

namespace ReservationSystem.Core.Validators
{
    public class CreateReservationValidator : AbstractValidator<CreateReservationDto>
    {
        public CreateReservationValidator()
        {
            RuleFor(x => x.ResourceId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.StartTime).LessThan(x => x.EndTime)
                .WithMessage("زمان شروع باید از زمان پایان کوچک‌تر باشد.");
            RuleFor(x => x.StartTime).Must(BeInFuture)
                .WithMessage("زمان رزرو باید در آینده باشد.");
            RuleFor(x => x.EndTime).Must((dto, end) => (end - dto.StartTime).TotalHours <= 4)
              .WithMessage("مدت زمان رزرو حداکثر ۴ ساعت مجاز است.");
        }

        private bool BeInFuture(DateTime startTime) => startTime > DateTime.Now;
    }

}
