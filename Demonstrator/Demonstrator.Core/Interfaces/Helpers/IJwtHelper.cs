using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.Core.Models;
using System;

namespace Demonstrator.Core.Interfaces.Helpers
{
    public interface IJwtHelper
    {
        Response IsValid(string jwt, JwtScopes reqScope, DateTime? tokenIssued = null);

        Response IsValidUser(string jwt);
    }
}
