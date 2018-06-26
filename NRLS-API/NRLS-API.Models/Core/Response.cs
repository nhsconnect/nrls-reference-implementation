namespace NRLS_API.Models.Core
{
    public class Response
    {
        public Response()
        {
            Success = false;
        }

        public Response(bool success)
        {
            Success = success;
        }

        public Response(string message)
        {
            Message = message;
        }

        public Response(bool success, string message)
        {
            Success = success;

            Message = message;
        }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
