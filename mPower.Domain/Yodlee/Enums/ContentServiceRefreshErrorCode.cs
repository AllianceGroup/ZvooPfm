using System.ComponentModel;

namespace mPower.Domain.Yodlee.Enums
{
    public enum ContentServiceRefreshErrorCode
    {
        [Description("STATUS_OK")]
        STATUS_OK = 0,
        [Description("This institution's site can't be reached at this time. Please try again in 24hrs.")]
        STATUS_INVALID_GATHERER_REQUEST = 400,
        [Description("Your request has timed out. Please try again.")]
        STATUS_NO_CONNECTION = 401,
        [Description("Your credentials are incorrect. Please verify them at your institution's site, then retry.")]
        STATUS_LOGIN_FAILED = 402,
        [Description("Your institution's site may not be responding. Please contact support for more information and/or try again after 2-3 hours.")]
        STATUS_INTERNAL_ERROR = 403,
        [Description("This site is experiencing temporary technical difficulties. Please verify that you can log in to your institution's site directly, then contact support.")]
        STATUS_LOST_REQUEST = 404,
        [Description("The request was cancelled. Please try again. Contact support if the issue returns.")]
        STATUS_ABORT_REQUEST = 405,
        [Description("Your password has expired. To resolve, please visit your institution's site.")]
        STATUS_PASSWORD_EXPIRED = 406,
        [Description("Your account has been locked. To resolve, contact your institution.")]
        STATUS_ACCOUNT_LOCKED = 407,
        [Description("We found no accounts to update. Visit your institution's site and confirm your account has been set up, then try again.")]
        STATUS_DATA_EXPECTED = 408,
        [Description("Your institution reported that their service is temporarily unavailable. Please try logging in to and navigating through their site first, then try again later.")]
        STATUS_SITE_UNAVILABLE = 409,
        [Description("This site is experiencing technical difficulties. Please try again later.")]
        STATUS_POP3_SERVER_FAILED = 410,
        [Description("This institution no longer provides online services to its customer. Please contact our support department.")]
        STATUS_SITE_OUT_OF_BUSINESS = 411,
        [Description("This site is not refreshing. Please try again later.")]
        STATUS_SITE_APPLICATION_ERROR = 412,
        [Description("Your institution's site may have changed. Please contact support.")]
        STATUS_REQUIRED_FIELD_UNAVAILABLE = 413,
        [Description("Your account(s) can't be found at this site. Please verify you are selecting the correct site and/or category.")]
        STATUS_NO_ACCOUNT_FOUND = 414,
        [Description("Your institution's site is experiencing technical difficulties. Please try again later.")]
        STATUS_SITE_TERMINATED_SESSION = 415,
        [Description("Another session is already established with this site. This can happen if you are logged in to your institution's site at the same time you are attempting to add the account.")]
        STATUS_SITE_SESSION_ALREADY_ESTABLISHED = 416,
        [Description("This type of account isn't supported. Please notify support so they can remove the site from the list.")]
        STATUS_DATA_MODEL_NO_SUPPORT = 417,
        [Description("Your institution's site is experiencing technical difficulties. Please try again later.")]
        STATUS_HTTP_DNS_ERROR = 418,
        [Description("Your institution's site may have changed. Please contact support.")]
        STATUS_LOGIN_NOT_COMPLETED = 419,
        [Description("This site has merged with another site. Please contact support for more information.")]
        STATUS_SITE_MERGED_ERROR = 420,
        [Description("The language setting for your account isn't English. If you would like to add this account, please visit your institution's site and change the settings to English, then retry.")]
        STATUS_UNSUPPORTED_LANGUAGE_ERROR = 421,
        [Description("Your institution's appears to have closed or cancelled your account. Please contact their support department.")]
        STATUS_ACCOUNT_CANCELLED = 422,
        [Description("Your account information is unavailable. Oftentimes, this issue will resolve within 24 hours. Try again later. If this issue persists, please contact support.")]
        STATUS_ACCT_INFO_UNAVAILABLE = 423,
        [Description("Your institution's site is down for maintenance. Please try again in 2-3 hours.")]
        STATUS_SITE_DOWN_FOR_MAINTENANCE = 424,
        [Description("This site is experiencing technical difficulties. Please try again later.")]
        STATUS_SITE_CERTIFICATE_ERROR = 425,
        [Description("Please contact support. Your institution is blocking the service. We will need to contact the institution directly.")]
        STATUS_SITE_BLOCKING_ERROR = 426,
        [Description("A promotion needs to be viewed on your institution's site. Please view this promotion, then try to add your account again.")]
        STATUS_NEW_SPLASH_PAGE = 427,
        [Description("Your institution requires that you accept their new terms and conditions. Please contact their support/site to do so.")]
        STATUS_NEW_TERMS_AND_CONDITIONS = 428,
        [Description("Your institution requires you to update information on their site before adding your account. Please contact their site to resolve, then return.")]
        STATUS_UPDATE_INFORMATION_ERROR = 429,
        [Description("This site is no longer supported. Please contact support for more information.")]
        STATUS_SITE_NOT_SUPPORTED = 430,
        [Description("This site is experiencing technical difficulties. Please try again later.")]
        STATUS_HTTP_FILE_NOT_FOUND_ERROR = 431,
        [Description("This site is experiencing technical difficulties. Please try again later.")]
        STATUS_HTTP_INTERNAL_SERVER_ERROR = 432,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REGISTRATION_PARTIAL_SUCCESS = 433,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REGISTRATION_FAILED_ERROR = 434,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REGISTRATION_INVALID_DATA = 435,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REGISTRATION_ACCOUNT_ALREADY_REGISTERED = 436,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REGISTRATION_TIMEOUT = 404,
        [Description("Your institution's site may have changed. Please contact support.")]
        UNIQUEID_FROM_DATA_SOURCE_ERROR = 475,
        [Description("Your institution's site may have changed. Please contact support.")]
        ACCOUNT_REQUIRED_FIELDS_NOT_SET = 476,
        [Description("Your institution's site may have changed. Please contact support.")]
        BILL_REQUIRED_FIELDS_NOT_SET = 477,
        [Description("Your institution's site may have changed. Please contact support.")]
        STATUS_DUPLICATE_BILL = 478,
        [Description("We've encountered an unexpected error, please contact support.")]
        STATUS_COULD_NOT_GENERATE_AUTOREGISTER_CREDENTIALS = 479,
        [Description("We can't add the account for security reasons, please contact support.")]
        STATUS_MAX_REGISTRATION_ATTEMPTS_EXCEEDED = 481,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_ACCOUNT_REGISTERED_ELSE_WHERE = 484,
        [Description("Your account isn't supported in this region. Please contact support.")]
        STATUS_REGISTRATION_BOT_SUPPORTED_FOR_REGION = 485,
        [Description("Your account isn't supported in this region. Please contact support.")]
        STATUS_REGISTRATION_NOT_SUPPORTED_FOR_REGION = 485,
        [Description("Your account isn't supported. Please contact support.")]
        STATUS_UNSUPPORTED_REGISTRATION_ACCOUNT_TYPE = 486,
        [Description("We've encountered an unexpected error. Please contact support.")]
        REWARDS_PROGRAM_REQUIRED_FIELDS_NOT_SET = 491,
        [Description("We've encountered an unexpected error. Please contact support.")]
        REWARDS_ACTIVITY_REQUIRED_FIELDS_NOT_SET = 492,
        [Description("We've encountered an unexpected error. Please contact support.")]
        TAX_LOT_REQUIRED_FIELDS_NOT_SET = 493,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INVESTMENT_TRANSACTION_REQUIRED_FIELDS_NOT_SET = 494,
        [Description("We've encountered an unexpected error. Please contact support.")]
        LOAN_TRANSACTION_REQUIRED_FIELDS_NOT_SET = 495,
        [Description("We've encountered an unexpected error. Please contact support.")]
        CARD_TRANSACTION_REQUIRED_FIELDS_NOT_SET = 496,
        [Description("We've encountered an unexpected error. Please contact support.")]
        BANK_TRANSACTION_REQUIRED_FIELDS_NOT_SET = 497,
        [Description("We've encountered an unexpected error. Please contact support.")]
        HOLDING_REQUIRED_FIELDS_NOT_SET = 498,
        [Description("We've encountered an error with your institution's security system. Please contact support.")]
        SITE_CURRENTLY_NOT_SUPPORTED = 505,
        [Description("There is new login information required for this site. Please contact support.")]
        NEW_LOGIN_INFO_REQUIRED_FOR_SITE = 506,
        [Description("Your account could not update. Please try again within the next 1-2 hours.")]
        BETA_SITE_WORK_IN_PROGRESS = 507,
        [Description("Please try again and answer the security questions when prompted.")]
        STATUS_INSTANT_REQUEST_TIMEDOUT = 508,
        [Description("We've encountered an unexpected error. Please contact support.")]
        TOKEN_ID_INVALID = 509,
        [Description("The property information provided isn't correct. Please verify your property information and try again.")]
        PROPERTY_RECORD_NOT_FOUND = 510,
        [Description("The home value can't be found. Please contact support.")]
        HOME_VALUE_NOT_FOUND = 511,
        [Description("We've encountered an unexpected error. Please contact support.")]
        NO_PAYEE_FOUND = 512,
        [Description("We've encountered an unexpected error. Please contact support.")]
        NO_PAYEE_RETRIEVED = 513,
        [Description("We've encountered an unexpected error. Please contact support.")]
        SOME_PAYEE_NOT_RETRIEVED = 514,
        [Description("We've encountered an unexpected error. Please contact support.")]
        NO_PAYMENT_ACCOUNT_FOUND = 515,
        [Description("We've encountered an unexpected error. Please contact support.")]
        NO_PAYMENT_ACCOUNT_SELECTED = 516,
        [Description("We've encountered technical difficulties. Please try again later.")]
        GENERAL_EXCEPTION_WHILE_GATHERING_MFA_DATA = 517,
        [Description("The security information you've provided is incorrect. Please verify and retry.")]
        NEW_MFA_INFO_REQUIRED_FOR_AGENTS = 518,
        [Description("The security information provided is incorrect. Please verify and retry.")]
        MFA_INFO_NOT_PROVIDED_TO_YODLEE_BY_USER_FOR_AGENTS = 519,
        [Description("The security information you've provided is incorrect. Please verify and retry.")]
        MFA_INFO_MISMATCH_FOR_AGENTS = 520,
        [Description("The security information you've provided is incorrect. Please verify and retry. Please contact support.")]
        ENROLL_IN_MFA_AT_SITE = 521,
        [Description("The request has timed out. Please try again.")]
        MFA_INFO_NOT_PROVIDED_IN_REAL_TIME_BY_USER_VIA_APP = 522,
        [Description("The security information you've provided is incorrect. Please verify and retry. Please contact support.")]
        INVALID_MFA_INFO_IN_REAL_TIME_BY_USER_VIA_APP = 523,
        [Description("The security information you've provided has expired. Please verify and retry. Please contact support.")]
        USER_PROVIDED_REAL_TIME_MFA_DATA_EXPIRED = 524,
        [Description("We've encountered an unexpected error. Please contact support.")]
        MFA_INFO_NOT_PROVIDED_IN_REAL_TIME_BY_GATHERER = 525,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INVALID_MFA_INFO_OR_CREDENTIALS = 526,
        [Description("The information provided is insufficient. Please try again.")]
        STATUS_DBFILER_SUMMARY_SAVE_ERROR = 601,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REQUEST_GENERATION_ERROR = 602,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REQUEST_DISPATCH_ERROR = 603,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REQUEST_GENERATION_ERROR_LOGIN_FAILURE = 604,
        [Description("We've encountered an unexpected error. Please contact support.")]
        STATUS_REQUEST_GENERATION_ERROR_DELETED_ITEM = 605,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INPUT_INVALID_DATA = 701,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INPUT_LENGTH_ERROR = 702,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INPUT_FORMAT_ERROR = 703,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INPUT_USERNAME_ALREADY_TAKEN_ERROR = 704,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INPUT_VALUE_TOO_SMALL = 705,
        [Description("We've encountered an unexpected error. Please contact support.")]
        INPUT_VALUE_TOO_LARGE = 706,
        [Description("This account needs to be refreshed. Please contact support.")]
        REFRESH_NEVER_DONE = 801,
        [Description("This account needs to be refreshed. Please contact support.")]
        REFRESH_NEVER_DONE_AFTER_CREDENTIALS_UPDATE = 802,

    }
}