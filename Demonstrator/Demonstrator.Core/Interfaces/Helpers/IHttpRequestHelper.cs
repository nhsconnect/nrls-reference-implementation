using Demonstrator.Models.Core.Models;
using System.Net.Http;

namespace Demonstrator.Core.Interfaces.Helpers
{
    public interface IHttpRequestHelper
    {
        HttpClientHandler GetClientHandler<T>(T request) where T : Request;

        HttpRequestMessage GetRequestMessage<T>(T request) where T : Request;
    }
}
