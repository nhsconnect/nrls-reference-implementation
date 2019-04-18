using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    //rudementry proxy
    public class SspProxyService : ISspProxyService
    {
        IHttpRequestHelper _httpRequestHelper;

        public SspProxyService(IHttpRequestHelper httpRequestHelper)
        {
            _httpRequestHelper = httpRequestHelper;
        }

        public async Task<CommandResponse> ForwardRequest(CommandRequest request)
        {
            //var internalTraceId = Guid.NewGuid();

            var handler = _httpRequestHelper.GetClientHandler(request);

            var response = new CommandResponse();

            using (var client = new HttpClient(handler))
            {
                var httpRequest = _httpRequestHelper.GetRequestMessage(request);
                //string responseMessage = null;

                //LogRequest(httpRequest, request.Resource, internalTraceId);

                using (HttpResponseMessage res = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead))
                using (HttpContent content = res.Content)
                {
                    try
                    {
                        //TODO: upgrade to core 2.2 and use IHttpClientFactory for delegate handling
                        var data = content.ReadAsByteArrayAsync().Result;

                        //responseMessage = Encoding.UTF8.GetString(data, 0, data.Length);

                        if (res.Headers.TransferEncodingChunked == true &&
                            res.Headers.TransferEncoding.Count == 1)
                        {
                            res.Headers.TransferEncoding.Clear();
                        }

                        foreach (var resHeader in res.Headers)
                        {
                            response.Headers.Add(resHeader.Key, resHeader.Value);
                        }


                        var contentLength = content.Headers.ContentLength;

                        foreach (var resHeader in content.Headers)
                        {
                            response.Headers.Add(resHeader.Key, resHeader.Value);
                        }

                        response.Content = data;
                        response.StatusCode = (int)res.StatusCode;

                    }
                    catch(Exception e)
                    {
                        throw new HttpRequestException($"Parsing response from {request.ForwardUrl} failed.");
                    }
                    finally
                    {
                        //LogResponse(res.Headers, (int)res.StatusCode, responseMessage, internalTraceId);
                    }

                }
            }


            return await Task.Run(() => response);
        }
                
    }
}
