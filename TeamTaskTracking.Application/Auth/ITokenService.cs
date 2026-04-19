using System;
using System.Collections.Generic;
using System.Text;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.Application.Auth;

public interface ITokenService
{
    LoginResultDto CreateAccessToken(User user);
}
