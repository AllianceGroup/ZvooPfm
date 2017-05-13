using System.ComponentModel;
using mPower.Framework.Utils.Notification;

namespace mPower.Domain.Application.Enums
{
    public enum EmailTypeEnum
    {

        #region System emails

        /// <summary>
        /// This type of mail is send by affiliate manually
        /// </summary>
        [Description("Manually created"), NotificationGroup(NotificationGroupEnum.System)]
        ManuallyCreated = 2,

        /// <summary>
        /// Send mail of this type when user forgot his password and asking about reminder
        /// </summary>
        [Description("Forgot password"), NotificationGroup(NotificationGroupEnum.System)]
        ForgotPassword = 3,

        /// <summary>
        /// Send email of this type when calendar event came
        /// </summary>
        [Description("Calendar event came"), NotificationGroup(NotificationGroupEnum.System)]
        CalendarEventCame = 4,

        #endregion

        #region User can configure sending

        [Description("Low Balance"), NotificationGroup(NotificationGroupEnum.User)]
        LowBalance = 5,

        [Description("Large Purchases"), NotificationGroup(NotificationGroupEnum.User)]
        LargePurchases = 6,

        [Description("Bill Reminder"), NotificationGroup(NotificationGroupEnum.User)]
        BillReminder = 7,

        [Description("Available Credit"), NotificationGroup(NotificationGroupEnum.User)]
        AvailableCredit = 8,

        [Description("Unusual Spending"), NotificationGroup(NotificationGroupEnum.User)]
        UnusualSpending = 9,

        [Description("Over Budget"), NotificationGroup(NotificationGroupEnum.User)]
        OverBudget = 10,

        #endregion

        #region Affiliate can configure: Triggers

        [Description("Succesfull signup."), NotificationGroup(NotificationGroupEnum.Affiliate)]
         Signup = 50,

        [Description("Admin deactivated your account."), NotificationGroup(NotificationGroupEnum.Affiliate)]
        UserDeactivation = 51,

        [Description("Admin activated your account."), NotificationGroup(NotificationGroupEnum.Affiliate)]
        UserReactivation = 52,

        [Description("You've aggregated new account."), NotificationGroup(NotificationGroupEnum.Affiliate)]
        NewAccountAggregation = 53,

        [Description("You've added new credit identity."), NotificationGroup(NotificationGroupEnum.Affiliate)]
        NewCreditIdentity = 54,

        #endregion
    }
}
