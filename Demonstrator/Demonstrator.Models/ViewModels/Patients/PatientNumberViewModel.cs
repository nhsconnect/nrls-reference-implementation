using System;
using System.Linq;

namespace Demonstrator.Models.ViewModels.Patients
{
    public class PatientNumberViewModel
    {
        public string Id { get; set; }

        public string NhsNumber { get; set; }

        public string UniqueId => GenerateUniqueId();

        private string GenerateUniqueId()
        {
            return string.Join("", Guid.NewGuid().ToString("n").Take(9).Select(o => o));
        }
    }
}
