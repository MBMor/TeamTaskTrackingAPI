using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamTaskTracking.Application.Projects;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>  
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(150);

        RuleFor(x => x.Description)
         .MaximumLength(1500);
    }
}
