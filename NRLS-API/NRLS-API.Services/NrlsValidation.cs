using NRLS_API.Core.Enums;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System;

namespace NRLS_API.Services
{
    public class NrlsValidation : INrlsValidation
    {
        private readonly IJwtHelper _jwtHelper;

        public NrlsValidation(IJwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }

        public Response ValidJwt(Tuple<JwtScopes, string> reqScope, string jwt)
        {
            return _jwtHelper.IsValid(jwt, reqScope);
        }
    }
}
