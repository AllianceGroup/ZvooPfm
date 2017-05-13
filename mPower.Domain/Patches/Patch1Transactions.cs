using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Paralect.Domain;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Framework;

namespace mPower.Domain.Patches
{
    public class Patch1Transactions : IPatch
    {
        private readonly MongoWrite _write;

        public Patch1Transactions(MongoWrite write)
        {
            _write = write;
        }

        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            //Patch for mongodb version < 2.0
            _write.Transitions.Update(Query.EQ("Timestamp", DateTime.MinValue),
                                     Update<Transition>.Set(x => x.Timestamp, new DateTime(2010, 1, 1)), UpdateFlags.Multi);

            foreach (var transition in transitions)
            {
                var removeEventsIndexes = new List<TransitionEvent>();
                var transactionId = String.Empty;
                for (int i = 0; i < transition.Events.Count; i++)
                {
                    var item = transition.Events[i].Data as Event;
                    if (item != null)
                    {
                        #region Ledger_Transaction_CreatedEvent

                        var createdEvent = item as Ledger_Transaction_CreatedEvent;
                        if (createdEvent != null)
                        {
                            var createNewEvent = new Transaction_CreatedEvent();
                            createNewEvent.BaseEntryAccountId = createdEvent.BaseEntryAccountId;
                            createNewEvent.BaseEntryAccountType = createdEvent.BaseEntryAccountType;
                            createNewEvent.Entries = new List<ExpandedEntryData>();
                            foreach (var entry in createdEvent.Entries)
                            {
                                createNewEvent.Entries.Add(new ExpandedEntryData()
                                {
                                    AccountId = entry.AccountId,
                                    AccountLabel = entry.AccountLabel,
                                    AccountName = entry.AccountName,
                                    AccountType = entry.AccountType,
                                    BookedDate = entry.BookedDate,
                                    CreditAmountInCents = entry.CreditAmountInCents,
                                    DebitAmountInCents = entry.DebitAmountInCents,
                                    LedgerId = entry.LedgerId,
                                    TransactionId = entry.TransactionId,
                                    Memo = entry.Memo,
                                    Payee = entry.Payee,
                                    OffsetAccountId = entry.OffsetAccountId,
                                    OffsetAccountName = entry.OffsetAccountName,
                                    TransactionImported = entry.TransactionImported
                                });
                            }

                            createNewEvent.Imported = createdEvent.Imported;
                            createNewEvent.LedgerId = createdEvent.LedgerId;
                            createNewEvent.Memo = createdEvent.Memo;
                            createNewEvent.Metadata = new EventMetadata()
                            {
                                TypeName = createNewEvent.GetType().Name,
                                CommandId = createdEvent.Metadata.CommandId,
                                EventId = createdEvent.Metadata.EventId,
                                StoredDate = createdEvent.Metadata.StoredDate,
                                UserId = createdEvent.Metadata.UserId,
                                TransferedEvent = createdEvent.Metadata.TransferedEvent
                            };

                            createNewEvent.ReferenceNumber = createdEvent.ReferenceNumber;
                            createNewEvent.TransactionId = createdEvent.TransactionId;
                            createNewEvent.Type = createdEvent.Type;


                            transition.Events[i] = new TransitionEvent(createNewEvent.GetType().AssemblyQualifiedName,
                                                                       createNewEvent,
                                                                       new Dictionary<string, object>());

                            transactionId = createdEvent.TransactionId;
                        }

                        #endregion

                        #region Ledger_Transaction_ModifiedEvent

                        var modifiedEvent = item as Ledger_Transaction_ModifiedEvent;
                        if (modifiedEvent != null)
                        {
                            var newEvent = new Transaction_ModifiedEvent();
                            newEvent.BaseEntryAccountId = modifiedEvent.BaseEntryAccountId;
                            newEvent.BaseEntryAccountType = modifiedEvent.BaseEntryAccountType;
                            newEvent.Entries = new List<ExpandedEntryData>();
                            foreach (var entry in modifiedEvent.Entries)
                            {
                                newEvent.Entries.Add(new ExpandedEntryData()
                                {
                                    AccountId = entry.AccountId,
                                    AccountLabel = entry.AccountLabel,
                                    AccountName = entry.AccountName,
                                    AccountType = entry.AccountType,
                                    BookedDate = entry.BookedDate,
                                    CreditAmountInCents = entry.CreditAmountInCents,
                                    DebitAmountInCents = entry.DebitAmountInCents,
                                    LedgerId = entry.LedgerId,
                                    TransactionId = entry.TransactionId,
                                    Memo = entry.Memo,
                                    Payee = entry.Payee,
                                    OffsetAccountId = entry.OffsetAccountId,
                                    OffsetAccountName = entry.OffsetAccountName,
                                    TransactionImported = entry.TransactionImported
                                });
                            }

                            newEvent.Imported = modifiedEvent.Imported;
                            newEvent.LedgerId = modifiedEvent.LedgerId;
                            newEvent.Memo = modifiedEvent.Memo;
                            newEvent.Metadata = new EventMetadata()
                            {
                                TypeName = newEvent.GetType().Name,
                                CommandId = modifiedEvent.Metadata.CommandId,
                                EventId = modifiedEvent.Metadata.EventId,
                                StoredDate = modifiedEvent.Metadata.StoredDate,
                                UserId = modifiedEvent.Metadata.UserId,
                                TransferedEvent = modifiedEvent.Metadata.TransferedEvent
                            };

                            newEvent.ReferenceNumber = modifiedEvent.ReferenceNumber;
                            newEvent.TransactionId = modifiedEvent.TransactionId;
                            newEvent.Type = modifiedEvent.Type;

                            transition.Events[i] = new TransitionEvent(newEvent.GetType().AssemblyQualifiedName,
                                                                       newEvent,
                                                                       new Dictionary<string, object>());
                            transactionId = modifiedEvent.TransactionId;
                        }

                        #endregion

                        #region Ledger_Transaction_DeletedEvent

                        var deletedEvent = item as Ledger_Transaction_DeletedEvent;
                        if (deletedEvent != null)
                        {
                            var newEvent = new Transaction_DeletedEvent();

                            newEvent.LedgerId = deletedEvent.LedgerId;
                            newEvent.TransactionId = deletedEvent.TransactionId;
                            newEvent.Metadata = MapMetadata(deletedEvent.Metadata, newEvent.GetType().Name);

                            transition.Events[i] = new TransitionEvent(newEvent.GetType().AssemblyQualifiedName,
                                                                       newEvent,
                                                                       new Dictionary<string, object>());

                            transactionId = newEvent.TransactionId;
                        }

                        #endregion

                        #region Ledger_Account_BalanceChangedEvent

                        var balanceChanged = item as Ledger_Account_BalanceChangedEvent;

                        if (balanceChanged != null)
                        {
                            //we are don't have this  event in our domain anymore
                            removeEventsIndexes.Add(transition.Events[i]);
                        }

                        #endregion

                        #region Ledger_Transaction_DuplicateCreatedEvent

                        var dublicateEvent = item as Ledger_Transaction_DuplicateCreatedEvent;
                        if (dublicateEvent != null)
                        {
                            var newEvent = new Transaction_DuplicateCreatedEvent();
                            newEvent.BaseTransaction = MapEntryData(dublicateEvent.BaseTransaction);
                            newEvent.PotentialDuplicates = dublicateEvent.PotentialDuplicates.Select(MapEntryData).ToList();
                            newEvent.Metadata = MapMetadata(dublicateEvent.Metadata, newEvent.GetType().Name);

                            transition.Events[i] = new TransitionEvent(newEvent.GetType().AssemblyQualifiedName,
                                                                     newEvent,
                                                                     new Dictionary<string, object>());
                            transactionId = newEvent.BaseTransaction.TransactionId;
                        }

                        #endregion

                        #region Ledger_Transaction_DuplicateDeletedEvent

                        var deleteDublicateEvent = item as Ledger_Transaction_DuplicateDeletedEvent;

                        if (deleteDublicateEvent != null)
                        {
                            var newEvent = new Transaction_DuplicateDeletedEvent();
                            newEvent.Id = deleteDublicateEvent.Id;
                            newEvent.LedgerId = deleteDublicateEvent.LedgerId;
                            newEvent.Metadata = MapMetadata(deleteDublicateEvent.Metadata, newEvent.GetType().Name);

                            transition.Events[i] = new TransitionEvent(newEvent.GetType().AssemblyQualifiedName,
                                                                  newEvent,
                                                                  new Dictionary<string, object>());

                            transactionId = deleteDublicateEvent.Id;
                        }

                        #endregion

                        #region Ledger_Entry_DuplicateCreatedEvent

                        var dublicateEntryEvent = item as Ledger_Entry_DuplicateCreatedEvent;

                        if (dublicateEntryEvent != null)
                        {
                            var newEvent = new Transaction_Entry_DuplicateCreatedEvent();
                            newEvent.DuplicateId = dublicateEntryEvent.DuplicateId;
                            newEvent.LedgerId = dublicateEntryEvent.LedgerId;
                            newEvent.ManualEntry = MapEntryData(dublicateEntryEvent.ManualEntry);
                            newEvent.Metadata = MapMetadata(dublicateEntryEvent.Metadata, newEvent.GetType().Name);
                            newEvent.PotentialDuplicates = dublicateEntryEvent.PotentialDuplicates.Select(MapEntryData).ToList();

                            transition.Events[i] = new TransitionEvent(newEvent.GetType().AssemblyQualifiedName,
                                                                  newEvent,
                                                                  new Dictionary<string, object>());

                            transactionId = dublicateEntryEvent.DuplicateId;
                        }

                        #endregion
                    }
                }

                foreach (var i in removeEventsIndexes)
                {
                    transition.Events.Remove(i);
                }

                if (transition.Events.Count == 0)
                {
                    transitionRepository.RemoveTransition(transition.Id.StreamId, transition.Id.Version);
                }
                else if (!String.IsNullOrEmpty(transactionId))
                {
                    UpdateTransition(transitionRepository, transition, transactionId);
                }
            }
        }

        private EventMetadata MapMetadata(EventMetadata oldMetadata, string newTypeName)
        {
            return new EventMetadata()
            {
                TypeName = newTypeName,
                CommandId = oldMetadata.CommandId,
                EventId = oldMetadata.EventId,
                StoredDate = oldMetadata.StoredDate,
                UserId = oldMetadata.UserId,
                TransferedEvent = oldMetadata.TransferedEvent
            };
        }

        private ExpandedEntryData MapEntryData(Domain.Accounting.Ledger.Data.ExpandedEntryData data)
        {
            return new ExpandedEntryData()
                       {
                           LedgerId = data.LedgerId,
                           TransactionId = data.TransactionId,
                           AccountId = data.AccountId,
                           AccountType = data.AccountType,
                           AccountName = data.AccountName,
                           CreditAmountInCents = data.CreditAmountInCents,
                           DebitAmountInCents = data.CreditAmountInCents,
                           Memo = data.Memo,
                           Payee = data.Payee,
                           BookedDate = data.BookedDate,
                           AccountLabel = data.AccountLabel,
                           OffsetAccountId = data.OffsetAccountId,
                           OffsetAccountName = data.OffsetAccountName,
                           TransactionImported = data.TransactionImported
                       };
        }

        private static void UpdateTransition(ITransitionRepository transitionRepository, Transition transition, string transactionId)
        {
            transitionRepository.RemoveTransition(transition.Id.StreamId, transition.Id.Version);
            transition = new Transition(new TransitionId(transactionId, transition.Id.Version), transition.Timestamp, transition.Events, transition.Metadata);
            transitionRepository.SaveTransition(transition);
        }

        public string Name
        {
            get { return "Move transaction events to  Transaction Aggregate root"; }
        }

        public int Id
        {
            get { return 1; }
        }

        public bool UseIncomeTransitions
        {
            get { return true; }
        }
    }
}