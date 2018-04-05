using System.Collections.Generic;

namespace Demonstrator.Models.Core.Models
{
    public class ContentView
    {
        public string Title { get; set; }

        public string CssClass { get; set; }

        public int Order { get; set; }

        public IList<string> Content { get; set; }
    }
}
