using NRLS_API.Core.Enums;
using NRLS_API.Models.Core;
using System;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface INrlsValidation
    {
        Response ValidJwt(Tuple<JwtScopes, string> reqScope, string jwt);
    }
}
