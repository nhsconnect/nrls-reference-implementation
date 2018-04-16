using Demonstrator.Core.Helpers;
using Demonstrator.Core.Interfaces.Services;
using Demonstrator.Models.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;

namespace Demonstrator.NRLSAdapter.Helpers
{
    public class ClientAsidHelper : IClientAsidHelper
    {
        private readonly ExternalApiSetting _apiSettings;
        private IMemoryCache _cache;

        public ClientAsidHelper(IOptions<ExternalApiSetting> apiSettings, IMemoryCache memoryCache)
        {
            _apiSettings = apiSettings.Value;
            _cache = memoryCache;
        }

        public void Load()
        {
            //Fake SSP Interaction/ASID datastore

            if (!_cache.TryGetValue<ClientAsidMap>(ClientAsidMap.Key, out ClientAsidMap clientAsidMap))
            {
                var basePath = DirectoryHelper.GetBaseDirectory();

                using (StreamReader interactionFile = File.OpenText(Path.Combine(basePath, _apiSettings.ClientAsidMapFile)))
                {
                    var serializer = new JsonSerializer();
                    clientAsidMap = (ClientAsidMap)serializer.Deserialize(interactionFile, typeof(ClientAsidMap));

                    // Save data in cache.
                    _cache.Set(ClientAsidMap.Key, clientAsidMap);
                }
            }
        }
    }
}
