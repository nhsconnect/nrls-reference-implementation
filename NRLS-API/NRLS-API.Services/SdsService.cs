using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.Models.ViewModels.Core;
using NRLS_API.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class SdsService : ISdsService
    {
        private readonly INRLSMongoDBContext _context;
        private readonly IMemoryCache _cache;

        public SdsService(INRLSMongoDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public SdsViewModel GetFor(string asid)
        {
            var cache = GetAll().Result;

            return cache.FirstOrDefault(x => !string.IsNullOrEmpty(asid) && x.Asid == asid);
        }

        public SdsViewModel GetFor(string odsCode, string interactionId)
        {
            //TODO: LDAP support in .net core is limited right now
            //ldapsearch - x - H ldaps://ldap.vn03.national.ncrs.nhs.uk –b "ou=services, o=nhs" 
            //"(&(nhsIDCode={odsCode}) (objectClass=nhsAS)(nhsAsSvcIA={interactionId}))"
            //uniqueIdentifier nhsMhsPartyKey

            var cache = GetAll().Result;

            return cache.FirstOrDefault(x => !string.IsNullOrEmpty(odsCode) && x.OdsCode == odsCode
                                                    && (string.IsNullOrEmpty(interactionId) || x.Interactions.Contains(interactionId)));
        }


        public async Task<IEnumerable<SdsViewModel>> GetAll()
        {
            var cache = _cache.Get<IEnumerable<SdsViewModel>>(SdsViewModel.CacheKey);

            return cache ?? await GetAllFromSource();
        }

        public async Task<IEnumerable<SdsViewModel>> GetAllFromSource()
        {
            try
            {
                var builder = Builders<Sds>.Filter;
                var filters = new List<FilterDefinition<Sds>>();
                filters.Add(builder.Eq(x => x.Active, true));

                var entries = await _context.Sds.FindSync(builder.And(filters)).ToViewModelListAsync();

                CachePointers(entries);

                return entries;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        private void CachePointers(IEnumerable<SdsViewModel> entries)
        {
            if (!_cache.TryGetValue<IEnumerable<SdsViewModel>>(SdsViewModel.CacheKey, out IEnumerable<SdsViewModel> sdsEntries))
            {
                // Save data in cache.
                _cache.Set(SdsViewModel.CacheKey, entries, new TimeSpan(12, 0, 0));
            }
        }

    }
}
