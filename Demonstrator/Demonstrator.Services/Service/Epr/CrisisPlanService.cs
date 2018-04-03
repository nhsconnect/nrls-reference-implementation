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
using Hl7.Fhir.Model;
using Demonstrator.NRLSAdapter.Helpers;

namespace Demonstrator.Services.Service.Flows
{
    public class CrisisPlanService : ICrisisPlanService
    {
        private readonly INRLSMongoDBContext _context;

        private readonly IDocumentReferenceServices _documentReferenceServices;

        public CrisisPlanService(INRLSMongoDBContext context, IDocumentReferenceServices documentReferenceServices)
        {
            _context = context;
            _documentReferenceServices = documentReferenceServices;
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

        public async SystemTasks.Task<CrisisPlanViewModel> Save(CrisisPlanViewModel crisisPlan)
        {
            try
            {
                var currentPlan = await this.GetForPatient(crisisPlan.PatientNhsNumber, false);
                var version = 1;

                if (currentPlan != null)
                {
                    version = currentPlan.Version + 1;
                    var update = new UpdateDefinitionBuilder<CrisisPlan>().Set(n => n.Active, false);
                    var previous =_context.CrisisPlans.UpdateMany(item => item.RecordType == RecordType.MentalHealthCrisisPlan.ToString(), update);
                }

                crisisPlan.Version = version;
                crisisPlan.Active = true;

                var newCrisisPlan = CrisisPlan.ToModel(crisisPlan);

                _context.CrisisPlans.InsertOne(newCrisisPlan);

                var pointer = new DocumentReference
                {
                    VersionId = $"{version}",
                    Status = DocumentReferenceStatus.Current,
                    Author = new List<ResourceReference>()
                    {
                        new ResourceReference
                        {
                            Reference = $"{FhirConstants.SystemODS}{crisisPlan.OrgCode}"
                        }
                    },
                    Custodian = new ResourceReference
                    {
                        Reference = $"{FhirConstants.SystemODS}{crisisPlan.OrgCode}"
                    },
                    Subject = new ResourceReference
                    {
                        Reference = $"{FhirConstants.SystemPDS}{crisisPlan.PatientNhsNumber}"
                    },
                    Created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssz"),
                    Indexed = DateTime.UtcNow,
                    Type = new CodeableConcept(FhirConstants.CodingSystemPointerType, "718347000", "Mental health care plan"),
                    Content = new List<DocumentReference.ContentComponent>()
                    {
                        new DocumentReference.ContentComponent
                        {
                            Attachment = new Attachment
                            {
                                ContentType = "application/pdf",
                                Url = $"https://spine-proxy.national.ncrs.nhs.uk/{newCrisisPlan.Id}/mental-health-care-plan.pdf",
                                Title = "Mental Health Care Plan",
                                Creation = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssz")
                            }
                        }
                    }

                };

                var newPointer = _documentReferenceServices.CreatePointer(pointer);


                return await SystemTasks.Task.Run(() => CrisisPlan.ToViewModel(newCrisisPlan));

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async SystemTasks.Task<bool> Delete(string id)
        {
            try
            {

                //delete pointer

                var update = new UpdateDefinitionBuilder<CrisisPlan>().Set(n => n.Active, false);
                var previous = _context.CrisisPlans.UpdateOne(item => item.Id == new ObjectId(id) && item.RecordType == RecordType.MentalHealthCrisisPlan.ToString(), update);

                return await SystemTasks.Task.Run(() => previous.IsAcknowledged && previous.ModifiedCount > 0);

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

    }
}
