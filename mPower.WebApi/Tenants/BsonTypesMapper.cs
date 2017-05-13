using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson.Serialization;

namespace mPower.WebApi.Tenants
{
    public static class BsonTypesMapper
    {
        public static void RegisterBsonMaps()
        {
            // documents
            BsonClassMap.RegisterClassMap<LowBalanceAlertDocument>();
            BsonClassMap.RegisterClassMap<LargePurchaseAlertDocument>();
            BsonClassMap.RegisterClassMap<AvailableCreditAlertDocument>();
            BsonClassMap.RegisterClassMap<UnusualSpendingAlertDocument>();
            BsonClassMap.RegisterClassMap<OverBudgetAlertDocument>();
            BsonClassMap.RegisterClassMap<CalendarEventAlertDocument>();
            BsonClassMap.RegisterClassMap<DashboardAlertDocument>();
            BsonClassMap.RegisterClassMap<TransactionDuplicateDocument>();
            BsonClassMap.RegisterClassMap<EntryDuplicateDocument>();
            BsonClassMap.RegisterClassMap<LegalItem>();
            BsonClassMap.RegisterClassMap<TaxLien>();
            BsonClassMap.RegisterClassMap<Bankruptcy>();
            BsonClassMap.RegisterClassMap<SelectList>();
            BsonClassMap.RegisterClassMap<MonthYear>();
        }
    }
}