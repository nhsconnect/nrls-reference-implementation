namespace Demonstrator.Models.Core.Models
{
    public class Issue
    { 
        public Issue()
        {
            _severity = IssueSeverity.Information;
        }

        public string Severity => _severity.ToString();

        private IssueSeverity _severity { get; set; }

        public string Details { get; set; }
    }

    public enum IssueSeverity
    {
        Fatal = 0,
        Error,
        Warning,
        Information
    }
}
