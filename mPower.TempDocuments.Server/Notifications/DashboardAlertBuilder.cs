using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Application.Enums;
using mPower.Framework.Environment;
using mPower.Framework.Utils.Extensions;
using mPower.Framework.Utils.Notification;
using mPower.TempDocuments.Server.DocumentServices.Filters;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using ExpandedEntryData = mPower.Domain.Accounting.Transaction.Data.ExpandedEntryData;

namespace mPower.TempDocuments.Server.Notifications
{
    public class DashboardAlertBuilder
    {
        private readonly IIdGenerator _idGenerator;
        private readonly UserDocumentService _userService;
        private readonly LedgerDocumentService _ledgerService;
        private readonly CalendarDocumentService _calendarService;
        private readonly TransactionsStatisticDocumentService _statisticService;
        private readonly NotificationBuilder _notificationBuilder;
        private readonly NotificationTempService _notificationService;
        private readonly DashboardAlertTempService _dashboardAlertService;

        public DashboardAlertBuilder(IIdGenerator idGenerator, 
            UserDocumentService userService, 
            LedgerDocumentService ledgerService, 
            CalendarDocumentService calendarService,
            TransactionsStatisticDocumentService statisticService, 
            NotificationBuilder notificationBuilder, 
            NotificationTempService notificationService, 
            DashboardAlertTempService dashboardAlertService)
        {
            _idGenerator = idGenerator;
            _userService = userService;
            _ledgerService = ledgerService;
            _calendarService = calendarService;
            _statisticService = statisticService;
            _notificationBuilder = notificationBuilder;
            _notificationService = notificationService;
            _dashboardAlertService = dashboardAlertService;
        }

        public virtual void CheckForLowBalance(BalanceChangedData data)
        {
            const EmailTypeEnum type = EmailTypeEnum.LowBalance;
            var account = GetAccount(data.LedgerId, data.AccountId);
            if (account != null && account.LabelEnum == AccountLabelEnum.Bank)
            {
                var borderValue = GetBoarderValue(data.UserId, type);
                if (borderValue.HasValue)
                {
                    var borderValueInCents = AccountingFormatter.DollarsToCents((decimal) borderValue);
                    if (data.OldValueInCents >= borderValueInCents && borderValueInCents > data.BalanceInCents)
                    {
                        var alert = new LowBalanceAlertDocument
                        {
                            AccountName = data.AccountName,
                            NewBalance = data.BalanceInCents,
                            Text = string.Format("{0} has a low balance of {1}.", data.AccountName, AccountingFormatter.ConvertToDollarsThenFormat(data.BalanceInCents)),
                        };
                        HandleAlert(alert, type, data.UserId, data.LedgerId, data.Date);
                    }
                }
            }
        }

        public virtual void CheckForLargePurchase(string userId, string ledgerId, List<ExpandedEntryData> entries, DateTime date)
        {
            const EmailTypeEnum type = EmailTypeEnum.LargePurchases;
            var fromAccountTypes = new List<AccountLabelEnum> { AccountLabelEnum.Bank, /*AccountLabelEnum.CreditCard*/};
            var toAccountTypes = new List<AccountLabelEnum> { AccountLabelEnum.Expense, AccountLabelEnum.OtherExpense };

            var borderValue = GetBoarderValue(userId, type);
            if (borderValue.HasValue)
            {
                var limitInCents = AccountingFormatter.DollarsToCents((decimal)borderValue);
                var fromEntry = entries.Find(e => e.CreditAmountInCents > limitInCents && fromAccountTypes.Contains(e.AccountLabel));
                var toEntry = entries.Find(e => e.DebitAmountInCents > 0 && toAccountTypes.Contains(e.AccountLabel));
                if (fromEntry != null && toEntry != null)
                {
                    var alert = new LargePurchaseAlertDocument
                    {
                        AccountName = fromEntry.AccountName,
                        PurchaseInCents = fromEntry.CreditAmountInCents,
                        Text = string.Format("{0} shows a large purchase of {1}.", fromEntry.AccountName, AccountingFormatter.ConvertToDollarsThenFormat(fromEntry.CreditAmountInCents)),
                    };
                    HandleAlert(alert, type, userId, ledgerId, date);
                }
            }
        }

        public virtual void CheckAvailableCredit(BalanceChangedData data)
        {
            const EmailTypeEnum type = EmailTypeEnum.AvailableCredit;
            var account = GetAccount(data.LedgerId, data.AccountId);
            if (account != null && account.LabelEnum == AccountLabelEnum.CreditCard)
            {
                var borderValue = GetBoarderValue(data.UserId, type);
                if (borderValue.HasValue)
                {
                    var borderValueInCents = AccountingFormatter.DollarsToCents((decimal)borderValue);
                    if (account.CreditLimitInCents - data.OldValueInCents >= borderValueInCents && account.CreditLimitInCents - data.BalanceInCents < borderValueInCents)
                    {
                        var alert = new AvailableCreditAlertDocument
                        {
                            AccountName = data.AccountName,
                            SetAmountInCents = borderValueInCents,
                            AvailableCreditInCents = account.CreditLimitInCents - data.BalanceInCents,
                            Text = string.Format("Your available credit amount of {0} fell below {1}.", data.AccountName, AccountingFormatter.ConvertToDollarsThenFormat(borderValueInCents))
                        };
                        HandleAlert(alert, type, data.UserId, data.LedgerId, data.Date);
                    }
                }
            }
        }

        public virtual void CheckForUnusualSpending(string userId, string ledgerId, List<ExpandedEntryData> entries, DateTime date)
        {
            const EmailTypeEnum type = EmailTypeEnum.UnusualSpending;
            var fromAccountTypes = new List<AccountLabelEnum> { AccountLabelEnum.Bank, /*AccountLabelEnum.CreditCard*/};
            var toAccountTypes = new List<AccountLabelEnum> { AccountLabelEnum.Expense, AccountLabelEnum.OtherExpense };


            var borderValue = GetBoarderValue(userId, type);
            if (borderValue.HasValue)
            {
                var fromEntry = entries.Find(e => e.CreditAmountInCents > 0 && fromAccountTypes.Contains(e.AccountLabel));
                if (fromEntry != null)
                {
                    var toEntries = entries.Where(e => e.DebitAmountInCents > 0 && toAccountTypes.Contains(e.AccountLabel));
                    foreach (var expenseEntry in toEntries)
                    {
                        var monthlyAmount = GetMonthlyAmountInCents(ledgerId, expenseEntry.AccountId, expenseEntry.BookedDate);
                        var avrAmount = (long)Math.Round(_statisticService.GetLastSixMonthsAvr(ledgerId, expenseEntry.AccountId, expenseEntry.BookedDate), 0, MidpointRounding.AwayFromZero);
                        var limitInCents = AccountingFormatter.DollarsToCents((decimal)borderValue);

                        if (monthlyAmount - avrAmount > limitInCents)
                        {
                            var alert = new UnusualSpendingAlertDocument
                            {
                                AccountName = expenseEntry.AccountName,
                                MonthlyAmountInCents = monthlyAmount,
                                AverageAmountInCents = avrAmount,
                                Text = string.Format("Unusual spending in {0}.", expenseEntry.AccountName),
                            };
                            HandleAlert(alert, type, userId, ledgerId, date);
                        }
                    }
                }
            }
        }

        public virtual void CreateOverBudget(BudgetExceededData data)
        {
            const EmailTypeEnum type = EmailTypeEnum.OverBudget;
            var monthStr = new DateTime(2000, data.Month, 1).ToString("MMMM");
            var alert = new OverBudgetAlertDocument
            {
                AccountName = data.AccountName,
                Month = data.Month,
                BudgetAmount = data.BudgetAmount,
                SpentAmountWithSubAccounts = data.SpentAmountWithSubAccounts,
                Text = string.Format("The budget for account {0} in {1} has exceeded.", data.AccountName, monthStr),
            };
            HandleAlert(alert, type, data.UserId, data.LedgerId, data.Date);
        }

        public virtual void CreateCalendarEventAlert(OnetimeCalendarEventData data, string calendarId)
        {
            CreateCalendarEventAlert(new List<OnetimeCalendarEventData> {data}, calendarId);
        }

        public virtual void CreateCalendarEventAlert(List<OnetimeCalendarEventData> data, string calendarId)
        {
            if (data == null || data.Count == 0) return;

            var calendar = _calendarService.GetById(calendarId);
            if (calendar == null) return;

            var alerts = data.Select(x => new CalendarEventAlertDocument
            {
                CalendarEventId = x.EventId,
                Date = x.EventDate,
                Description = x.Description,
                SendAlertOptions = x.SendAlertOptions,
                Text = string.Format("Reminder: {0} - {1}.", x.EventDate, x.Description),
            }).Cast<DashboardAlertDocument>().ToList();
            var firstEvent = data.First();
            HandleAlerts(alerts, EmailTypeEnum.CalendarEventCame, firstEvent.UserId, calendar.LedgerId, firstEvent.CreatedDate);
        }

        public virtual void CreateForgotPasswordNotification(string userId)
        {
            var alert = new DashboardAlertDocument();
            HandleAlert(alert, EmailTypeEnum.ForgotPassword, userId, null, DateTime.Now);
        }

        private void HandleAlert(DashboardAlertDocument alerts, EmailTypeEnum type, string userId, string ledgerId, DateTime date)
        {
            HandleAlerts(new List<DashboardAlertDocument> {alerts}, type, userId, ledgerId, date);
        }

        private void HandleAlerts(List<DashboardAlertDocument> alerts, EmailTypeEnum type, string userId, string ledgerId, DateTime date)
        {
            if (alerts == null || alerts.Count == 0) return;

            InitCommonProperties(alerts, type, userId, ledgerId, date);
            _notificationBuilder.InitSendOptions(alerts.Cast<BaseNotification>().ToList());

            SaveAlerts(alerts);
        }

        public void CancelAlerts(EmailTypeEnum type, List<string> publicKeys)
        {
            RemoveAlerts(null, publicKeys, type);
        }

        private void InitCommonProperties(IEnumerable<DashboardAlertDocument> alerts, EmailTypeEnum type, string userId, string ledgerId, DateTime date)
        {
            alerts.Each(alert =>
            {
                alert.Id = _idGenerator.Generate();
                alert.CreatedDate = date;
                alert.Type = type;
                alert.UserId = userId;
                alert.LedgerId = ledgerId;
            });
        }

        private int? GetBoarderValue(string userId, EmailTypeEnum alertType)
        {
            
            var user = _userService.GetById(userId);
            if (user != null)
            {
                var notifSetting = user.Notifications.Find(n => n.Type == alertType);
                if (notifSetting != null)
                {
                    return notifSetting.BorderValue;
                }
            }
            return null;
        }

        private AccountDocument GetAccount(string ledgerId, string accountId)
        {
            var ledger = _ledgerService.GetById(ledgerId);
            if (ledger != null)
            {
                return ledger.Accounts.Find(a => a.Id == accountId);
            }
            return null;
        }

        private long GetMonthlyAmountInCents(string ledgerId, string accountId, DateTime month)
        {
            var monthlyFilter = new TransactionsStatisticFilter
            {
                LedgerId = ledgerId,
                AccountId = accountId,
                Year = month.Year,
                Month = month.Month,
            };
            var statistic = _statisticService.GetByFilter(monthlyFilter);

            if (statistic.Count > 0)
            {
                var debit = statistic.Sum(s => s.DebitAmountInCents);
                var credit = statistic.Sum(s => s.CreditAmountInCents);

                return AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(debit, credit, statistic[0].AccountType);
            }

            return 0;
        }

        public void RemoveAlerts(List<string> ids, List<string> publicKeys, EmailTypeEnum? type)
        {
            if ((ids == null || ids.Count == 0) && !type.HasValue && (publicKeys == null || publicKeys.Count == 0))
            {
                return;
            }

            var alertFilter = new AlertFilter { Ids = ids, Type = type, PublicKeys = publicKeys };
            _dashboardAlertService.Remove(alertFilter);

            var notificationfilter = new NotificationFilter { Ids = ids, Type = type, PublicKeys = publicKeys };
            _notificationService.Remove(notificationfilter);
        }

        private void SaveAlerts(List<DashboardAlertDocument> alerts)
        {
            var alertsForSaving = alerts.Where(x => x.Type.GetNotificationGroup() == NotificationGroupEnum.User).ToList();
            if (alertsForSaving.Count > 0)
            {
                _dashboardAlertService.InsertMany(alertsForSaving);
            }

            // plan notification if required
            var alertsWithNotifications = alerts.Where(x => x.SendEmail || x.SendText).ToList();
            if (alertsWithNotifications.Count > 0)
            {
                _notificationService.InsertMany(alertsWithNotifications);
            }
        }
    }
}