﻿using Hl7.Fhir.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NRLS_API.Models.Core;
using SystemTasks = System.Threading.Tasks;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface IFhirMaintain
    {
        SystemTasks.Task<T> Create<T>(FhirRequest request) where T : Resource;

        SystemTasks.Task<(T created, bool updated)> CreateWithUpdate<T>(FhirRequest request, FhirRequest updateRequest, UpdateDefinition<BsonDocument> updates) where T : Resource;

        SystemTasks.Task<bool> Update<T>(FhirRequest request, UpdateDefinition<BsonDocument> updates) where T : Resource;

        SystemTasks.Task<bool> Delete<T>(FhirRequest request) where T : Resource;

        SystemTasks.Task<bool> DeleteConditional<T>(FhirRequest request) where T : Resource;
    }
}
