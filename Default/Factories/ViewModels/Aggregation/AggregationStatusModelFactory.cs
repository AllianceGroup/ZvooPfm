using Default.Models;
using mPower.Aggregation.Client;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc;
using System;
using System.Linq;

namespace Default.Factories.ViewModels.Aggregation
{
    public class AggregationStatusModelFactory : IObjectFactory<AggregationStatusModelFilter, AggregationStatusModel>
    {
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly IAggregationClient _aggregation;

        public AggregationStatusModelFactory(LedgerDocumentService ledgerDocumentService, IAggregationClient aggregation)
        {
            _ledgerDocumentService = ledgerDocumentService;
            _aggregation = aggregation;
        }

        public AggregationStatusModel Load(AggregationStatusModelFilter input)
        {
            var ledger = _ledgerDocumentService.GetById(input.LedgerId);
            var account = ledger.Accounts.Single(x => x.Id == input.AccountId);
            var model = new AggregationStatusModel();

            if (!account.IntuitInstitutionId.HasValue || !account.IntuitAccountId.HasValue)
            {
                model.Message = "Account data is corrupted. Please remove this account and aggregate it once more.";
            }
            else
            {
                model.Status = account.AggregatedAccountStatus;
                switch (model.Status)
                {
                    case 0:
                    case AggregatedAccountStatusEnum.Normal:
                    case AggregatedAccountStatusEnum.PullingTransactions:
                    case AggregatedAccountStatusEnum.BeginPullingTransactions:
                        // do nothing
                        break;

                    case AggregatedAccountStatusEnum.AccountBeingAggregated:
                        model.Message = "The account is currently being aggregated. Please check later for aggregation results.";
                        break;
                    case AggregatedAccountStatusEnum.NeedReauthentication:
                        model.Message = "Can't login to aggregation service with specified credentials.";
                        model.ContentServiceId = account.IntuitInstitutionId.Value;
                        model.IntuitAccountId = account.IntuitAccountId.Value;
                        break;
                    case AggregatedAccountStatusEnum.NeedInteractiveRefresh:
                        model.Message = "To refresh this account your need to answer security questions.";
                        model.IntuitAccountId = account.IntuitAccountId.Value;
                        break;
                    case AggregatedAccountStatusEnum.TimeoutTerminated:
                        model.Message = "Pulling transactions was terminated by timeout. Please try again later or contact administrator.";
                        model.IntuitAccountId = account.IntuitAccountId.Value;
                        break;
                    case AggregatedAccountStatusEnum.UnexpectedErrorOccurred:
                          model.Message = "The service to add this account is currently unavailable. The end-site may be temporarily down. Please retry later. If this error persists please contact us via the support tab on the side of the screen and provide the error message below:";
                        if (!string.IsNullOrEmpty(account.AggregationExceptionId))
                        {
                            var exception = _aggregation.GetAggregationException(account.AggregationExceptionId);
                            model.Message = exception.Message + model.Message;
                            model.ErrorId = account.AggregationExceptionId;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Status", "Invalid aggregation status");
                }
            }
            return model;
        }
    }
}