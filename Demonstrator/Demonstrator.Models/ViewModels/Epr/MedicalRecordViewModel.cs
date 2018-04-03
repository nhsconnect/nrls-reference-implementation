using Demonstrator.Models.Core.Enums;

namespace Demonstrator.Models.ViewModels.Epr
{
    public class MedicalRecordViewModel
    {
        public string Id { get; set; }

        public int Version { get; set; }

        public RecordType RecordType { get; set; }

        public string OrgCode { get; set; }

        public string Asid { get; set; }

        public bool Active { get; set; }

        public string PatientNhsNumber { get; set; }
    }
}
