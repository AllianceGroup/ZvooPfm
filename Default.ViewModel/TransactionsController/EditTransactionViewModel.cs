using System.Collections.Generic;
using Default.ViewModel.Areas.Shared;

namespace Default.ViewModel.TransactionsController
{
    public class EditTransactionViewModel
    {
        public List<Category> Categories;
        public Transaction Transaction { get; set; }
    }
}