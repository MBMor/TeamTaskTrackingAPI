using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamTaskTracking.Application.Users;

public sealed class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Role)
            .IsInEnum();
    }
}
