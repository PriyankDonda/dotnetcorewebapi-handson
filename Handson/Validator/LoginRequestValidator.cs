using FluentValidation;
using Handson.RequestResponseModel;

namespace Handson;

public class LoginRequestValidator : AbstractValidator<LoginBORequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
    
}