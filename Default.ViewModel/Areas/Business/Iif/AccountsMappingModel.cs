using System.Collections.Generic;
using Default.ViewModel.Areas.Shared;

namespace Default.ViewModel.Areas.Business.Iif
{
    public class AccountsMappingModel
    {
        public List<ImportedAccount> ImportedAccounts { get; set; }
        public List<Category> Categories { get; set; }

        public AccountsMappingModel()
        {
            ImportedAccounts = new List<ImportedAccount>();
            Categories = new List<Category>();
        }
    }
}
