using Hl7.Fhir.Model;

namespace Demonstrator.Models.Core.Models
{
    public class Issue
    { 
        public Issue()
        {
            Severity = IssueSeverity.Information;
        }

        public IssueSeverity Severity { get; set; }

        public string SeverityCode => Severity.ToString();

        public string Message { get; set; }

        public string Diagnostics { get; set; }

        public CodeableConcept Details { get; set; }
    }

    public enum IssueSeverity
    {
        Fatal = 0,
        Error,
        Warning,
        Information
    }
}
