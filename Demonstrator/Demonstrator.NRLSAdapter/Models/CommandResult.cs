using System.Collections.Generic;

namespace Demonstrator.NRLSAdapter.Models
{
    public class CommandResult<T> : CommandResponse
    {
        public T Result { get; set; }

        public static CommandResult<T> Set(bool success, string message, T result)
        {
            return new CommandResult<T>
            {
                Success = success,
                Message = message,
                Result = result
            };

        }
    }
}
