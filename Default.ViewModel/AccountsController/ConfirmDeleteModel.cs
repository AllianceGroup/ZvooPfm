namespace Default.ViewModel.AccountsController
{
    public class ConfirmDeleteModel
    {
        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public bool DisplayHasTransactionsMessage { get; set; }

        public bool IsLinked { get; set; }
    }
}