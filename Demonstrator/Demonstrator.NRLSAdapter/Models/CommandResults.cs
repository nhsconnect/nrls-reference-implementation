using System.Collections.Generic;

namespace Demonstrator.NRLSAdapter.Models
{
    public class CommandResults<T> : CommandResponse
    {
        public IEnumerable<T> Results { get; set; }

        public static CommandResults<T> Set(bool success, string message, IEnumerable<T> results = null)
        {
            return new CommandResults<T>
            {
                Success = success,
                Message = message,
                Results = results
            };

        }
    }
}
