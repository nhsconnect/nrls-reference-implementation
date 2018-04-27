using Hl7.Fhir.Model;

namespace NRLS_API.Core.Interfaces.Services
{
    public interface INrlsConformance
    {
        CapabilityStatement GetConformance();
    }
}
