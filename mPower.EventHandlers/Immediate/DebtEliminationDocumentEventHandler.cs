using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using NLog;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.DebtElimination.Events;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;

namespace mPower.EventHandlers.Immediate
{
    public class DebtEliminationDocumentEventHandler :
        IMessageHandler<DebtElimination_CreatedEvent>,
        IMessageHandler<DebtElimination_Debts_SetEvent>,
        IMessageHandler<DebtElimination_EliminationPlanUpdatedEvent>,
        IMessageHandler<DebtElimination_DisplayMode_UpdatedEvent>,
        IMessageHandler<DebtElimination_AddedToCalendarEvent>,
        IMessageHandler<Ledger_Account_UpdatedEvent>,
        IMessageHandler<Ledger_Account_RemovedEvent>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<DebtElimination_DebtToIncomeRatio_UpdatedEvent>
    {
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly LedgerDocumentService _ledgerService;

        public DebtEliminationDocumentEventHandler(DebtEliminationDocumentService debtEliminationService, LedgerDocumentService ledgerService)
        {
            _debtEliminationService = debtEliminationService;
            _ledgerService = ledgerService;
        }

        public void Handle(DebtElimination_CreatedEvent message)
        {
            var doc = new DebtEliminationDocument
            {
                Id = message.Id,
                LedgerId = message.LedgerId,
                UserId = message.UserId,
            };

            _debtEliminationService.Insert(doc);
        }

        public void Handle(DebtElimination_Debts_SetEvent message)
        {
            var query = Query.EQ("_id", message.DebtEliminationId);
            
            var debts = message.Debts ?? new List<DebtItemData>();
            // for compatibility with old version of messages
            if (debts.Count == 0 && message.DebtsIds != null && message.DebtsIds.Count > 0)
            {
                var debtProgram = _debtEliminationService.GetById(message.DebtEliminationId);
                if (debtProgram != null)
                {
                    var ledger = _ledgerService.GetById(debtProgram.LedgerId);
                    if (ledger != null)
                    {
                        debts =
                            ledger.Accounts.Where(x => message.DebtsIds.Contains(x.Id)).Select(x =>
                            new DebtItemData
                                {
                                    DebtId = x.Id,
                                    Name = x.Name,
                                    BalanceInCents = x.ActualBalance,
                                    InterestRatePerc = x.InterestRatePerc,
                                    MinMonthPaymentInCents = x.MinMonthPaymentInCents
                                }).ToList();
                    }
                }
            }
            var update = Update<DebtEliminationDocument>.Set(x => x.Debts, debts);

            _debtEliminationService.Update(query, update);
        }

        public void Handle(DebtElimination_EliminationPlanUpdatedEvent message)
        {
            var query = Query.EQ("_id", message.Id);

            var update = Update<DebtEliminationDocument>.Set(x => x.PlanId, message.PlanId)
                               .Set(x => x.MonthlyBudgetInCents, message.MonthlyBudgetInCents);

            _debtEliminationService.Update(query, update);
        }

        public void Handle(DebtElimination_DisplayMode_UpdatedEvent message)
        {
            var query = Query.EQ("_id", message.Id);

            var update = Update<DebtEliminationDocument>.Set(x => x.DisplayMode, message.DisplayMode);

            _debtEliminationService.Update(query, update);
        }

        public void Handle(DebtElimination_AddedToCalendarEvent message)
        {
            var query = Query.EQ("_id", message.DebtEliminationId);
            var update = Update<DebtEliminationDocument>.Set(x => x.AddedToCalendar, true);
            _debtEliminationService.Update(query, update);
        }

        public void Handle(Ledger_Account_UpdatedEvent message)
        {
            var query = Query.And(Query.EQ("LedgerId", message.LedgerId), Query.EQ("Debts.DebtId", message.AccountId));
            var update = Update.Set("Debts.$.Name", BsonValue.Create(message.Name) ?? BsonNull.Value);

            _debtEliminationService.UpdateMany(query, update);
        }

        public void Handle(Ledger_Account_RemovedEvent message)
        {
            var query = Query.EQ("LedgerId", message.LedgerId);
            var update = Update.Pull("Debts", Query.EQ("DebtId", message.AccountId));

            _debtEliminationService.UpdateMany(query, update);
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var filter = new DebtEliminationFilter {LedgerId = message.LedgerId};
            _debtEliminationService.Remove(filter);
        }
		
		public void Handle(DebtElimination_DebtToIncomeRatio_UpdatedEvent message)
        {
            var doc = new DebtToIncomeRatioDocument
            {
                DebtToIncomeRatio = message.DebtToIncomeRatio,
                MonthlyGrossIncomeInCents = message.MonthlyGrossIncomeInCents,
                TotalMonthlyDebtInCents = message.TotalMonthlyDebtInCents,
                TotalMonthlyPitiaInCents = message.TotalMonthlyPitiaInCents,
                TotalMonthlyRentInCents = message.TotalMonthlyRentInCents,
                DebtToIncomeRatioString = message.DebtToIncomeRatioString
            };

            var query = Query.EQ("_id", message.Id);
            var update = Update<DebtEliminationDocument>.Set(x => x.DebtToIncomeRatio, doc);

            _debtEliminationService.Update(query, update);

        }
    }
}
