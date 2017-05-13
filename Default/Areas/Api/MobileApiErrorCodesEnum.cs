namespace Default.Areas.Api
{
    public enum MobileApiErrorCodesEnum
    {
        None = 0,
        // 1 - reserved for model state's errors
        UserNotFound = 2,
        LedgerNotFound = 3,
        InvalidAccessToken = 4,
        CreditIdentityNotFound = 5,
        TransactionNotFound = 6,
        LedgerAccountNotFound = 7,
        RequiredParameterMissed = 8,
        IllegalParameterValue = 9,
    }
}