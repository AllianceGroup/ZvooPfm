namespace mPower.Domain.Accounting.Enums
{
    public enum AggregatedAccountStatusEnum
    {
        /// <summary>
        /// Data aggregated, everything okay
        /// </summary>
        Normal = 1,
        /// <summary>
        /// This status means that our aggregation work currently
        /// </summary>
        PullingTransactions = 2,
        /// <summary>
        /// cccapture.325: The account is currently being aggregated. Please check back later for aggregation results. 
        /// User have to manually refresh account after some time, or our quartz job will do this within day
        /// </summary>
        AccountBeingAggregated = 3,
        /// <summary>
        /// cccapture.103: Need refresh account credentials, because they was changed on remote FI
        /// </summary>
        NeedReauthentication = 4,
        UnexpectedErrorOccurred = 5,
        /// <summary>
        /// Pulling command sent, this status still allows to run
        /// </summary>
        BeginPullingTransactions = 6,
        /// <summary>
        /// cccapture.185: User action is required to bypass multi-factor authentication
        /// </summary>
        NeedInteractiveRefresh = 7,
        TimeoutTerminated = 8
    }
}
