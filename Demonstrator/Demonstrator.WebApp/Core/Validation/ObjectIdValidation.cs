using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Demonstrator.WebApp.Core.Validation
{
    public class EnsureAreObjectIdsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var isValid = true;
            Regex rgx = new Regex("[^A-Fa-f0-9]{1,1024}");

            var list = value as IList;
            if (list != null)
            {
                foreach (string id in list) { 
                    if (!rgx.IsMatch(id))
                    {
                        isValid = false;
                        break;
                    }
                }
 
            }
            return isValid;
        }
    }
}
