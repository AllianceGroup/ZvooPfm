using System;
using com.yodlee.common;
using com.yodlee.core.dataservice;
using com.yodlee.soap.common;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Domain.Yodlee.Storage.Documents
{
    [BsonKnownTypes(typeof(GeographicRegion_US))]
    public class ContentServiceDocument
    {
        public const int keyWordsRequired = 1;
        public const int loginFormLastModifiedDateRequired = 2;
        public const int autoRegFormLastModifiedDateRequired = 4;
        public const int autoRegFormRequired = 8;
        public const int loginFormRequired = 16;
        public const int deletedContentServiceIdNeeded = 32;
        public const int directTransferProfileRequired = 64;
        public const int iconAndFaviconImagesRequired = 128;
        [BsonId]
        public long contentServiceId;
        public long serviceId;
        public string contentServiceDisplayName;
        public long organizationId;
        public string organizationDisplayName;
        public long siteId;
        public string siteDisplayName;
        public bool custom;
        public string loginUrl;
        public string homeUrl;
        public string registrationUrl;
        public string passwordHelpUrl;
        public string contactPhoneNumber;
        public string contactEmailAddress;
        public string contactUrl;
        public ContainerInfo containerInfo;
        public bool isCredentialRequired;
        public bool autoRegistrationSupported;
        public int autoLoginType;
        public GeographicRegion[] geographicRegionsServed;
        public bool autoPayCardSetupSupported;
        public bool directCardPaymentSupported;
        public bool directCheckPaymentSupported;
        public bool autoPayCardCancelSupported;
        public bool paymentVerificationSupported;
        public string autoPayCardEnrollMessage;
        public long[] supportedAutoPaySetupCardTypeIds;
        public long[] supportedDirectPaymentCardTypeIds;
        public bool hasPaymentHistory;
        public long timeToUpdatePaymentHistory;
        public long timeToPostDirectCardPayment;
        public bool isCSCForDirectPaymRequired;
        public bool isCSCForAutoPayRequired;
        public string timeZoneId;
        public bool isIAVFastSupported;
        public bool hasSiblingContentServices;
        public bool isFTEnabled;
        public bool isOnlinePaymentSupported;
        public PaperBillSuppressionType autoRegistrationPaperBillSuppressionType;
        public PaperBillSuppressionTypeEnum? _autoRegistrationPaperBillSuppressionTypeEnum;
        public string autoRegistrationDisclaimer;
        public PaperBillSuppressionType autoPayCardPaperBillSuppressionType;
        public PaperBillSuppressionTypeEnum? _autoPayCardPaperBillSuppressionTypeEnum;
        public string autoPayCardDisclaimer;
        public PaperBillSuppressionType directCardPaymentPaperBillSuppressionType;
        public PaperBillSuppressionTypeEnum? _directCardPaymentPaperBillSuppressionTypeEnum;
        public string directCardPaymentDisclaimer;
        public string[] keywords;
        public com.yodlee.common.Form loginForm;
        public com.yodlee.common.Form autoRegForm;
        public DateTime loginFormLastModified;
        public DateTime autoRegistrationFormLastModified;
        public string marketingContent;
        public bool addItemAccountSupported;
        public bool isAddAccountMultiFormAction;
        public bool isAutoRegistrationMultiFormAction;
        public bool isAddItemAccountMultiFormAction;
        public bool isSiteCredentialsStored;
        public bool isPaymentAmountRequiredForAutopay;
        public bool isNumberOfPaymentsRequiredForAutopay;
        public bool isFrequencyRequiredForAutopay;
        public long[] supportedAutopayFrequencyTypes;
        public bool isConveninceFeeChargedForDirectCardPayment;
        public string conveninceFeeRuleMessage;
        public YMoney autopayThresholdAmount;
        public bool isEBillPaymSupprtd;
        public bool isBetaSite;
        public bool isBPAASource;
        public bool isBPAADest;
        public long[] supportedBPSRecurringFrequencies;
        public int checkLeadInterval;
        public int cardLeadInterval;
        public MfaType mfaType;
        public MfaTypeEnum? _mfaTypeEnum;
        public bool isDirectTransferSupported;
        public DirectTransferProfile directTransferProfile;
        public bool isSiblingAutoAdditionSafe;
        public byte[] iconImageContent;
        public byte[] faviconImageContent;
    }

    public class ContainerInfo
    {
        public string containerName;
        public long?  assetType;

    }
}
