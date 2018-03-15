namespace Demonstrator.Models.Core.Models
{
    public class Issue
    { 
        public Issue()
        {
            SeverityCode = IssueSeverity.Information;
        }

        public string Severity => SeverityCode.ToString();

        public IssueSeverity SeverityCode { get; set; }

        public string Details { get; set; }

        public string Diagnostics { get; set; }
    }

    public enum IssueSeverity
    {
        Fatal = 0,
        Error,
        Warning,
        Information
    }
}
