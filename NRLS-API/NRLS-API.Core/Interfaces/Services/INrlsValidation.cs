using NRLS_API.Core.Helpers;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface INrlsValidation
    {
        bool ValidJwt(JwtScopes scope, string jwt);
    }
}
