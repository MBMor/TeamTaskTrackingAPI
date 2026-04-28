using FluentValidation;

namespace TeamTaskTracking.Application.Users;

public sealed class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Role)
            .IsInEnum();
    }
}
