using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Services;

namespace NRLS_API.Services
{
    public class NrlsValidation : INrlsValidation
    {
        public bool ValidJwt(JwtScopes scope, string jwt)
        {
            return JwtHelper.IsValid(jwt, scope);
        }
    }
}
