namespace Demonstrator.Models.ViewModels.Base
{
    public class RequestViewModel
    {
        public string Id { get; set; }

        public string OrgCode { get; set; }

        public string Asid { get; set; }

        public static RequestViewModel Create(string id)
        {
            return new RequestViewModel
            {
                Id = id
            };
        }
    }
}
