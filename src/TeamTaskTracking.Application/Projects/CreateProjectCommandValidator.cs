using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamTaskTracking.Application.Projects;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand> 
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.OwnerUserId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(1500);
    }
}
