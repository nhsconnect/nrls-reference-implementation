using NRLS_API.Core.Enums;
using NRLS_API.Models.Core;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface INrlsValidation
    {
        Response ValidJwt(JwtScopes scope, string jwt);
    }
}
