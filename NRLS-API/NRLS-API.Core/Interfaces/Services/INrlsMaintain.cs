using Hl7.Fhir.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Models.Core;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface INrlsMaintain
    {
        SystemTasks.Task<OperationOutcome> ValidateCreate(FhirRequest request);

        SystemTasks.Task<Resource> ValidateConditionalUpdate(FhirRequest request);

        SystemTasks.Task<Resource> CreateWithoutValidation(FhirRequest request);

        SystemTasks.Task<Resource> SupersedeWithoutValidation(FhirRequest request, string oldDocumentId, string oldVersionId);

        SystemTasks.Task<OperationOutcome> Delete(FhirRequest request);

        FhirRequest SetMetaValues(FhirRequest request);

        void BuildSupersede(string oldDocumentId, string oldVersion, out UpdateDefinition<BsonDocument> updates, out FhirRequest updateRequest);
    }
}
