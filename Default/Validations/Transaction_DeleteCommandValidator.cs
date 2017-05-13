using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Framework.Mvc.Validation;

namespace Default.Validations
{
    public class Transaction_DeleteCommandValidator : IValidator<Transaction_DeleteCommand>
    {
        private readonly TransactionDocumentService _transactionDocumentService;

        public Transaction_DeleteCommandValidator(TransactionDocumentService transactionDocumentService)
        {
            _transactionDocumentService = transactionDocumentService;
        }

        public bool IsValid(Transaction_DeleteCommand command)
        {
            var transactionToDelete = _transactionDocumentService.GetById(command.TransactionId);

            return transactionToDelete != null && transactionToDelete.LedgerId == command.LedgerId;
        }
    }
}