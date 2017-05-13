using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Shared
{
    public class Category   
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string ParentAccountId { get; set; }
        public AccountLabelEnum AccountLabel { get; set; }
        public AccountTypeEnum AccountType { get; set; }
    }
}