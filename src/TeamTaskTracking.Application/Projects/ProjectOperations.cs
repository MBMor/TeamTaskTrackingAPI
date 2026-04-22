using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamTaskTracking.Application.Projects;

public static class ProjectOperations
{
    public static OperationAuthorizationRequirement Read { get; } =
        new() { Name = nameof(Read) };

    public static OperationAuthorizationRequirement Update { get; } =
    new() { Name = nameof(Update) };

    public static OperationAuthorizationRequirement Delete { get; } =
    new() { Name = nameof(Delete) };
}
