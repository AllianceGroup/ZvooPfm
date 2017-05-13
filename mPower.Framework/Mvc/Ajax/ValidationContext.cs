using System;
using System.Collections.Generic;

namespace mPower.Framework.Mvc.Ajax
{
    public class ValidationContext
    {
        private readonly IList<ErrorMessage> errorMessages;
        private string _summaryContainerId;


        public IList<ErrorMessage> ErrorMessages
        {
            get { return errorMessages; }
        }

        public ValidationContext()
        {
            errorMessages = new List<ErrorMessage>();
        }

        public void AddError(String message, string containerName)
        {
            errorMessages.Add(new ErrorMessage(message, containerName));
        }

        public void MoveAllErrorsToSummary(string summaryContainerId)
        {
            _summaryContainerId = summaryContainerId;
        }

        /// <summary>
        /// Returns true, if there is no any error messages in current Response
        /// </summary>
        /// <returns></returns>
        public Boolean IsValid()
        {
            return errorMessages.Count == 0;
        }

        public Object ToJsonObject()
        {
            IList<Object> items = new List<object>();

            foreach (ErrorMessage message in ErrorMessages)
            {
                items.Add(new { Message = message.Message, Name =  !String.IsNullOrEmpty(_summaryContainerId) ? _summaryContainerId :  message.Container });
            }

            return items;
        }
    }
}