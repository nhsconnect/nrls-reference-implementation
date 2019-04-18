using NRLS_API.Models.Core;
using System.Net.Http;

namespace NRLS_API.Core.Interfaces.Helpers
{
    public interface IHttpRequestHelper
    {
        HttpClientHandler GetClientHandler(CommandRequest request);

        HttpRequestMessage GetRequestMessage(CommandRequest request);
    }
}
