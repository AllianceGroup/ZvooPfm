using System;
using System.Collections.Generic;
using Paralect.Domain;

namespace mPower.Domain.Accounting.Transaction.Messages
{
    public class Aggregation_TransactionSavedMessage : Event
    {
        public string UserId { get; set; }

        public long IntuitAccountId { get; set; }

        public string LedgerId { get; set; }

        public DateTime? LatestPostedDate { get; set; }
        public List<long> SavedTransactionFinicityIds { get; set; }
    }
}