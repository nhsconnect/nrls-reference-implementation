using Demonstrator.Models.Core.Enums;
using Demonstrator.Models.ViewModels.Base;

namespace Demonstrator.Models.ViewModels.Epr
{
    public class MedicalRecordViewModel : RequestViewModel
    {
        public int Version { get; set; }

        public RecordType RecordType { get; set; }

        public bool Active { get; set; }

        public string PatientNhsNumber { get; set; }
    }
}
