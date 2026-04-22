using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTaskTracking.Application.Projects;

public sealed record ProjectDto
(
    Guid Id,
    Guid OwnerUserId,
    string Name,
    string? Description,
    DateTime CreateAtUtc
);
