using Demonstrator.WebApp.Core.Validation;
using System.Collections.Generic;

namespace Demonstrator.WebApp.Core.Models
{
    public class ObjectIdList
    {
        [EnsureAreObjectIds(ErrorMessage = "Not all provided ids are valid.")]
        public List<string> ObjectIds { get; set; }
    }
}
