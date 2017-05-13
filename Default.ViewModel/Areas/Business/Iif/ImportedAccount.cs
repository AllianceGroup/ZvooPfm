using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Business.Iif
{
    public class ImportedAccount
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AccountLabelEnum Label { get; set; }
    }
}
