namespace mPower.Domain.Yodlee.Enums
{
    public enum RefreshStatus
    {
        STATUS_UNKNOWN_CODE = 0,
        SUCCESS_NEXT_REFRESH_SCHEDULED_CODE = 1,
        REFRESH_ALREADY_IN_PROGRESS_CODE = 2,
        UNSUPPORTED_OPERATION_FOR_SHARED_ITEM_CODE = 3,
        SUCCESS_START_REFRESH_CODE = 4,
        ITEM_CANNOT_BE_REFRESHED_CODE = 5,
        ALREADY_REFRESHED_RECENTLY_CODE = 6,
        UNSUPPORTED_OPERATION_FOR_CUSTOM_ITEM_CODE = 7,
        SUCCESS_REFRESH_WAIT_FOR_MFA_CODE = 8,
        InvalidItem // This is used when we get an error thrown from Yodlee
    }

    public enum DataUpdateAttemptStatus
    {
        SUCCESS,
        IN_PROCESS,
        USER_ACTION_REQUIRED,
        LOGIN_FAILURE,
        DATA_SOURCE_ERROR,
        OTHER_ERROR,
    }
}