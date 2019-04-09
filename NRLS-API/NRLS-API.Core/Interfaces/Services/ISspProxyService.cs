using NRLS_API.Models.Core;
using System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface ISspProxyService
    {
        Task<CommandResponse> ForwardRequest(CommandRequest request);
    }
}
