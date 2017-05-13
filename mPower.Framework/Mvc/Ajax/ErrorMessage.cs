using System;

namespace mPower.Framework.Mvc.Ajax
{
    public class ErrorMessage
    {
        public string Container { get; set; }
        public string Message { get; set; }

        public ErrorMessage(string message, string containerName)
        {
            Container = containerName;
            Message = message;
        }
    }
}