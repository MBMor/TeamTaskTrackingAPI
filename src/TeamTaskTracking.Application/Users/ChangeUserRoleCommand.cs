using System;
using System.Collections.Generic;
using System.Text;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Application.Users;

public sealed record ChangeUserRoleCommand(UserRole Role);
