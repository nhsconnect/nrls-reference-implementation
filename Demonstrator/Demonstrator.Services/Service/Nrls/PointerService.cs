﻿using Demonstrator.Core.Exceptions;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Core.Resources;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.Nrls;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.Models.ViewModels.Factories;
using Demonstrator.Models.ViewModels.Nrls;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.Services.Service.Base;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Services.Service.Nrls
{
    public class PointerService : BaseFhirService, IPointerService
    {
        private readonly ApiSetting _apiSetting;
        private readonly IDocumentReferenceServices _docRefService;
        private readonly IDocumentsServices _docService;
        private readonly IPatientServices _patientService;
        private readonly IOrganisationServices _organisationServices;
        private readonly IMemoryCache _cache;

        public PointerService(IOptions<ApiSetting> apiSetting, IDocumentReferenceServices docRefService, IPatientServices patientService, IOrganisationServices organisationServices, IMemoryCache cache, IDocumentsServices docService)
        {
            _apiSetting = apiSetting.Value;
            _docRefService = docRefService;
            _patientService = patientService;
            _organisationServices = organisationServices;
            _cache = cache;
            _docService = docService;
        }

        public async SystemTasks.Task<IEnumerable<PointerViewModel>> GetPointers(RequestViewModel request)
        {
            var pointerViewModels = new List<PointerViewModel>();

            var pointerRequest = NrlsPointerRequest.Search(request.OrgCode, request.Id, null, request.Asid, null);

            var pointerResponse = await _docRefService.GetPointersBundle(pointerRequest);

            if (pointerResponse.ResourceType.Equals(ResourceType.OperationOutcome))
            {
                throw new HttpFhirException("Invalid Fhir Request", (OperationOutcome)pointerResponse, null);
            }

            var pointerBundle = pointerResponse as Bundle;

            //need a more slick solution for getting related references
            //we are connecting to NRLS so will only get Pointers back - a complete Fhir server would allow for includes
            var patients = await _patientService.GetPatients(); //In live this could be lots
            var organisations = await _organisationServices.GetOrganisations(); //In live this could be lots

            var pointers = ListEntries<DocumentReference>(pointerBundle.Entry, ResourceType.DocumentReference);
            //var patients = ListEntries<Patient>(entries, ResourceType.Patient); // If we could do includes take from bundle
            //var organisations = ListEntries<Organization>(entries, ResourceType.Organization); // If we could do includes take from bundle

            foreach (var pointer in pointers)
            {
                var pointerViewModel = pointer.ToViewModel(DefaultUrlBase, PointerUrlBase);
                var patientNhsNumber = pointerViewModel.Subject?.Reference?.Replace(FhirConstants.SystemPDS, "");
                var authorOrgCode = pointerViewModel.Author?.Reference?.Replace(FhirConstants.SystemODS, "");
                var custodianOrgCode = pointerViewModel.Custodian?.Reference?.Replace(FhirConstants.SystemODS, "");

                //This assumes the resource is relative
                //In reality it does not make sense to attach a patient because a GET to NRLS should be in the patient context anyway!
                var subject = patients.FirstOrDefault(s => s.Identifier.FirstOrDefault(t => !string.IsNullOrEmpty(patientNhsNumber) && !string.IsNullOrEmpty(t.System) && t.System.Equals(FhirConstants.SystemNhsNumber) && !string.IsNullOrEmpty(t.Value) && t.Value.Equals(patientNhsNumber)) != null);
                pointerViewModel.SubjectViewModel = subject?.ToViewModel(null);

                //This assumes the resource is relative
                var custodian = organisations.FirstOrDefault(s => s.Identifier.FirstOrDefault(t => !string.IsNullOrEmpty(custodianOrgCode) && !string.IsNullOrEmpty(t.System) && t.System.Equals(FhirConstants.SystemOrgCode) && !string.IsNullOrEmpty(t.Value) && t.Value.Equals(custodianOrgCode)) != null);
                pointerViewModel.CustodianViewModel = custodian?.ToViewModel(FhirConstants.SystemOrgCode);

                var author = organisations.FirstOrDefault(s => s.Identifier.FirstOrDefault(t => !string.IsNullOrEmpty(authorOrgCode) && !string.IsNullOrEmpty(t.System) && t.System.Equals(FhirConstants.SystemOrgCode) && !string.IsNullOrEmpty(t.Value) && t.Value.Equals(authorOrgCode)) != null);
                pointerViewModel.AuthorViewModel = author?.ToViewModel(FhirConstants.SystemOrgCode);

                pointerViewModels.Add(pointerViewModel);
            }

            if (pointers.Any())
            {
                CachePointers(request.Id, pointerViewModels);
            }

            return pointerViewModels;
        }

        public async SystemTasks.Task<Binary> GetPointerDocument(RequestViewModel request, PointerViewModel pointer)
        {
            var pointerUrl = pointer.Content.FirstOrDefault().Attachment.Url;
            var providerOrgCode = pointer.AuthorViewModel.OrgCode;

            var pointerResponse = await _docService.GetPointerDocument(request.Asid, request.OrgCode, providerOrgCode, pointerUrl);

            return pointerResponse as Binary;
        }

        public PointerViewModel GetCachedPointer(string nhsNumber, string pointerId)
        {
            var pointers = CachedPointers(nhsNumber);

            return pointers.FirstOrDefault(x => !string.IsNullOrEmpty(pointerId) && x.Id == pointerId);
        }


        //DO NOT DO IN LIVE
        private IEnumerable<PointerViewModel> CachedPointers(string nhsNumber)
        {
            var map = _cache.Get<PatientPointers>($"Pointers:{nhsNumber}");

            return map != null ? map.Pointers : new List<PointerViewModel>();
        }

        private void CachePointers(string nhsNumber, List<PointerViewModel> pointers)
        {
            if (!_cache.TryGetValue<PatientPointers>($"Pointers:{nhsNumber}", out PatientPointers patientPointers))
            {
                // Save data in cache.
                _cache.Set($"Pointers:{nhsNumber}", new PatientPointers { Pointers = pointers }, new TimeSpan(0,30,0));
            }
        }

        private string PointerUrlBase
        {
            get
            {
                return $"{(_apiSetting.Secure ? "https" : "http")}{_apiSetting.BaseUrl}:{(_apiSetting.Secure ? _apiSetting.SecurePort : _apiSetting.DefaultPort)}/";
            }
        }

        //DocRefs created in //repo/data/defaultdata/DocumentReference.json 
        //have attachment.url's starting with this url base
        private string DefaultUrlBase
        {
            get
            {
                return $"http{_apiSetting.BaseUrl}:{_apiSetting.DefaultPort}/";
            }
        }

    }
}
