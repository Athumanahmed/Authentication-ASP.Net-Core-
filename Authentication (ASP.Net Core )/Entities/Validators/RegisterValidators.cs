using Authentication__ASP.Net_Core__.Entities.ModelsDTO;
using FluentValidation;

namespace Authentication__ASP.Net_Core__.Entities.Validators
{
    public class RegisterValidators : AbstractValidator<RegisterDTO>
    {
        public RegisterValidators()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.MiddleName).NotEmpty().WithMessage("Middle name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");

            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"\d").WithMessage("Password must contain at least one number")
                .Matches(@"[^\w\d]").WithMessage("Password must contain at least one special character");
        }
    }
}
