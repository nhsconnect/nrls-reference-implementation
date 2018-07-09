using MongoDB.Driver;
using System;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;
using MongoDB.Bson;
using Demonstrator.Core.Interfaces.Database;
using Demonstrator.Models.DataModels.Epr;
using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Models.ViewModels.Epr;
using Demonstrator.Models.Core.Enums;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Nrls;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.Models.ViewModels.Base;
using System.Linq;
using Hl7.Fhir.Model;
using Demonstrator.Models.ViewModels.Factories;

namespace Demonstrator.Services.Service.Flows
{
    public class CrisisPlanService : ICrisisPlanService
    {
        private readonly INRLSMongoDBContext _context;
        private readonly IDocumentReferenceServices _documentReferenceServices;
        private readonly IPointerMapService _pointerMapService;

        private const string MentalHealthCrisisPlanTypeCode = "736253002";

        private const string MentalHealthCrisisPlanTypeDisplay = "Mental Health Crisis Plan";

        public CrisisPlanService(INRLSMongoDBContext context, IDocumentReferenceServices documentReferenceServices, IPointerMapService pointerMapService)
        {
            _context = context;
            _documentReferenceServices = documentReferenceServices;
            _pointerMapService = pointerMapService;
        }

        public async SystemTasks.Task<CrisisPlanViewModel> GetForPatient(string nhsNumber, bool ifActive)
        {
            try
            {
                var builder = Builders<CrisisPlan>.Filter;
                var filters = new List<FilterDefinition<CrisisPlan>>();

                if (ifActive)
                {
                    filters.Add(builder.Eq(x => x.Active, true));
                }

                filters.Add(builder.Eq(x => x.PatientNhsNumber, nhsNumber));
                filters.Add(builder.Eq(x => x.RecordType, RecordType.MentalHealthCrisisPlan.ToString()));

                var options = new FindOptions<CrisisPlan, CrisisPlan>();
                options.Sort = Builders<CrisisPlan>.Sort.Descending(x => x.Version);

                var model = await _context.CrisisPlans.FindSync(builder.And(filters), options).FirstOrDefaultAsync();

                if(model == null)
                {
                    return null;
                }

                return CrisisPlan.ToViewModel(model);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async SystemTasks.Task<CrisisPlanViewModel> GetById(string planId)
        {
            try
            {
                var builder = Builders<CrisisPlan>.Filter;
                var filters = new List<FilterDefinition<CrisisPlan>>();
                filters.Add(builder.Eq(x => x.Active, true));
                filters.Add(builder.Eq(x => x.Id, new ObjectId(planId)));
                filters.Add(builder.Eq(x => x.RecordType, RecordType.MentalHealthCrisisPlan.ToString()));

                var model = await _context.CrisisPlans.FindSync(builder.And(filters), null).FirstOrDefaultAsync();

                if (model == null)
                {
                    return null;
                }

                return CrisisPlan.ToViewModel(model);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async SystemTasks.Task<CrisisPlanViewModel> SavePlan(CrisisPlanViewModel crisisPlan)
        {
            try
            {
                crisisPlan = crisisPlan.Cleaned();

                var currentPlan = await this.GetForPatient(crisisPlan.PatientNhsNumber, true);
                int version = 1;

                if (currentPlan != null)
                {
                    version = currentPlan.Version + 1;
                    currentPlan.Asid = crisisPlan.Asid;
                    //Should really archive plans after update rather than mark as delete
                    //Delete only when an active delete request is made
                    var deleted = await DeletePlan(currentPlan);
                }

                crisisPlan.Version = version;
                crisisPlan.Active = true;

                //Create new plan
                var newCrisisPlan = CrisisPlan.ToModel(crisisPlan);

                _context.CrisisPlans.InsertOne(newCrisisPlan);

                //Hardcoded values - this is a demo with urls to ficticious documents
                var pointerRequest = NrlsPointerRequest.Create(crisisPlan.OrgCode, crisisPlan.OrgCode, crisisPlan.PatientNhsNumber, $"https://spine-proxy.national.ncrs.nhs.uk/{newCrisisPlan.Id}/mental-health-care-plan.pdf", "application/pdf", MentalHealthCrisisPlanTypeCode, MentalHealthCrisisPlanTypeDisplay, crisisPlan.Asid);

                //Create new NRLS pointer
                var newPointer = await _documentReferenceServices.GenerateAndCreatePointer(pointerRequest);

                //Create map between NRLS Pointer and Medical Record
                _pointerMapService.CreatePointerMap(newPointer.ResourceId, newCrisisPlan.Id, RecordType.MentalHealthCrisisPlan);

                return await SystemTasks.Task.Run(() => CrisisPlan.ToViewModel(newCrisisPlan));

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async SystemTasks.Task<bool> DeletePlan(RequestViewModel request)
        {
            try
            {
                //Find and delete Medical Record
                var update = new UpdateDefinitionBuilder<CrisisPlan>().Set(n => n.Active, false);
                var previous = _context.CrisisPlans.UpdateOne(item => item.Id == new ObjectId(request.Id) && item.RecordType == RecordType.MentalHealthCrisisPlan.ToString(), update);

                //Find pointer id from local map
                var pointerMap = await _pointerMapService.FindPointerMap(request.Id, RecordType.MentalHealthCrisisPlan);

                //Delete pointer from NRLS
                var deletedPointer = true;
                if(pointerMap != null && !string.IsNullOrEmpty(pointerMap.NrlsPointerId))
                {
                    var pointerRequest = NrlsPointerRequest.Delete(pointerMap.NrlsPointerId, request.Asid, request.OrgCode);
                    var outcome = await _documentReferenceServices.DeletePointer(pointerRequest);

                    deletedPointer = (outcome != null && outcome.Success);

                    if (deletedPointer)
                    {
                        var delMap = _pointerMapService.DeletePointerMap(pointerMap.Id);
                    }
 
                }
                

                return await SystemTasks.Task.Run(() => previous.IsAcknowledged && previous.ModifiedCount > 0 && deletedPointer);

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

    }
}
