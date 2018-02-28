using System.Collections.Generic;

namespace Demonstrator.NRLSAdapter.Models
{
    public class CommandResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public static CommandResponse Set(bool success, string message)
        {
            return new CommandResponse
            {
                Success = success,
                Message = message
            };

        }
    }
}
