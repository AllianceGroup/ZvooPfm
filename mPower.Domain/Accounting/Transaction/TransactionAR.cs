using System;
using System.Collections.Generic;
using System.Linq;
using Paralect.Domain;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Framework;

namespace mPower.Domain.Accounting.Transaction
{
    public class TransactionAR : MpowerAggregateRoot
    {
        public bool IsTransactionDeleted = false;

        /// <summary>
        /// For object reconstraction
        /// </summary>
        public TransactionAR() { }

        public TransactionAR(string transactionId,
                            string userId,
                            string ledgerId,
                            TransactionType type,
                            List<ExpandedEntryData> entriesData,
                            string baseEntryAccountId,
                            AccountTypeEnum? baseEntryAccountType,
                            bool imported,
                            string importedTransactionId,
                            ICommandMetadata metadata,
                            string referenceNumber = "",
                            bool isMultiple = false)
        {
            SetCommandMetadata(metadata);
            GuardTransactionDataIsValid(transactionId, entriesData);
            if (!String.IsNullOrEmpty(baseEntryAccountId) && baseEntryAccountType == null)
            {
                throw new InvalidOperationException("If baseEntryAccountId was initialized then baseEntryAccountType should be initialized as well.");
            }

            var evt = new Transaction_CreatedEvent
            {
                UserId = userId,
                LedgerId = ledgerId,
                TransactionId = transactionId,
                Type = type,
                Entries = entriesData,
                Imported = imported,
                ReferenceNumber = referenceNumber,
                Memo = entriesData[0].Memo,
                BaseEntryAccountId = baseEntryAccountId,
                BaseEntryAccountType = baseEntryAccountType.GetValueOrDefault(),
                ImportedTransactionId = importedTransactionId,
                IsMultipleInsert = isMultiple,
            };

            Apply(evt);
        }

        public TransactionAR(ExpandedEntryData baseTransaction, List<ExpandedEntryData> potentialDublicates, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);

            Apply(new Transaction_DuplicateCreatedEvent
            {
                BaseTransaction = baseTransaction,
                PotentialDuplicates = potentialDublicates
            });
        }

        public void DeleteTransaction(string ledgerId, bool isMultiple = false)
        {
            GuardIsPossibleDeleteTransaction();
            Apply(new Transaction_DeletedEvent
            {
                LedgerId = ledgerId,
                TransactionId = _id,
                IsMultipleDelete = isMultiple
            });
        }

        public void DeleteDuplicate(string ledgerId)
        {
            Apply(new Transaction_DuplicateDeletedEvent {LedgerId = ledgerId, Id = _id});
        }

        public void ModifyTransaction(string ledgerId, TransactionType type, List<ExpandedEntryData> modifiedEntries, string baseEntryAccountId, AccountTypeEnum baseEntryAccountType, string referenceNumber, bool isImported)
        {
            GuardTransactionDataIsValid(_id, modifiedEntries);

            var evt = new Transaction_ModifiedEvent
            {
                LedgerId = ledgerId,
                TransactionId = _id,
                Type = type,
                Entries = modifiedEntries,
                ReferenceNumber = referenceNumber,
                Memo = modifiedEntries[0].Memo,
                BaseEntryAccountId = baseEntryAccountId,
                BaseEntryAccountType = baseEntryAccountType,
                Imported = isImported,
            };

            Apply(evt);

            //UpdateBalance(accountBalances, modifiedAccountBalances);
        }


        //public void ConfirmNotDuplicated(string ledgerId)
        //{
        //    var evt = new Transaction_ConfirmedNotDuplicatedEvent
        //    {
        //        LedgerId = ledgerId,
        //        TransactionId = _id,
        //    };

        //    Apply(evt);
        //}

        public void CreateDuplicateEntry(string duplicateId, string ledgerId, ExpandedEntryData manualEntry, List<ExpandedEntryData> potentialDuplicates)
        {
            Apply(new Transaction_Entry_DuplicateCreatedEvent
            {
                DuplicateId = duplicateId,
                LedgerId = ledgerId,
                ManualEntry = manualEntry,
                PotentialDuplicates = potentialDuplicates
            });
        }

        #region Guards

        private void GuardTransactionDataIsValid(String newTransactionId, List<ExpandedEntryData> entries)
        {
            if (entries.Count == 0)
                throw new InvalidOperationException(String.Format("Transaction [{0}] without entries cannot be created", newTransactionId));

            var isBalanced = entries.Sum(e => e.CreditAmountInCents) == entries.Sum(e => e.DebitAmountInCents);

            if (!isBalanced)
                throw new InvalidOperationException(String.Format("Transaction [{0}] isn't balanced.", newTransactionId));
        }

        private void GuardIsPossibleDeleteTransaction()
        {
            if (IsTransactionDeleted)
                throw new InvalidOperationException(String.Format("Transaction [{0}] was delete. It's can't be deleted once more", _id));
        }

        #endregion

        #region Object Reconstruction

        protected void On(Transaction_CreatedEvent created)
        {
            _id = created.TransactionId;
        }

        protected void On(Transaction_DeletedEvent deleted)
        {
            IsTransactionDeleted = true;
        }

        #endregion

    }
}
