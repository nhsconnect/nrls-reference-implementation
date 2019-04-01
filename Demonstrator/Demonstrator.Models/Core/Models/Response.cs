namespace Demonstrator.Models.Core.Models
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

        public void SetError(string message)
        {
            Message = message;
            Success = false;
        }
    }
}
