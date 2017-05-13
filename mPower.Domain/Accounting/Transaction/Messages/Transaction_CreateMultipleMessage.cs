using System;
using System.Collections.Generic;
using mPower.Domain.Accounting.Transaction.Data;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Messages
{
    public class Transaction_CreateMultipleMessage : Event
    {
        public List<CreateMultipleTransactionDto> Transactions { get; set; }

        public DateTime Date { get; set; }
    }
}
