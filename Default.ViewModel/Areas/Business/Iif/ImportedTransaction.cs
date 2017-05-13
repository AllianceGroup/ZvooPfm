using System.Collections.Generic;

namespace Default.ViewModel.Areas.Business.Iif
{
    public class ImportedTransaction
    {
        public string Id { get; set; }

        public bool Include { get; set; }

        public List<ImportedEntry> Entries { get; set; }

        public ImportedTransaction()
        {
            Include = true;
            Entries = new List<ImportedEntry>();
        }
    }
}
