using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_CreateCommand : Command
    {
        public String LedgerId { get; set; }

        public string Name { get; set; }

        public LedgerTypeEnum TypeEnum { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string TaxId { get; set; }

        public DateTime FiscalYearStart { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}