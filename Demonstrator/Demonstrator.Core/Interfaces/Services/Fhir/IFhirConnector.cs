using Demonstrator.Models.Core.Models;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Text;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IFhirConnector
    {
        SystemTasks.Task<Tout> RequestOneFhir<Tin, Tout>(Tin request) where Tin : Request where Tout : Resource;

        SystemTasks.Task<Tout> RequestOne<Tin, Tout>(Tin request) where Tin : Request where Tout : Response;

        SystemTasks.Task<List<Tout>> RequestMany<Tin, Tout>(Tin request) where Tin : Request where Tout : Resource;

    }
}
