using System;

namespace DoberDogBot.Application.Models
{
    internal class ValidationExceptionModel : Exception
    {
        public string CommandName { get; set; }

        public ValidationExceptionModel(string commandName, string message, Exception innerException) : base(message, innerException)
        {
            CommandName = commandName;
        }
    }
}
