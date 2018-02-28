using System;

namespace Demonstrator.Models.ViewModels.Fhir
{
    public partial class AttachmentViewModel
    {
        public string ContentType { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public DateTimeOffset? Creation { get; set; }
    }
}
