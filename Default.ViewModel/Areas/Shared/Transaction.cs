using System;
using System.Collections.Generic;

namespace Default.ViewModel.Areas.Shared
{
    public class Transaction
    {
        public Transaction()
        {
            Entries = new List<Entry>();
        }
        
        public string Id { get; set; }

        public string LedgerId { get; set; }
        
        public List<Entry> Entries { get; set; }

        public DateTime BookedDate { get; set; }
    }
}