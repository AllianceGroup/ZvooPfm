using java.util;

namespace mPower.Domain.Yodlee
{
    public static class YodleeSettings
    {
        public static string CobrandUsername = "mpower";
        public static string CobrandPassword = "mP0werPub5dk";
        public static long CobrandId = 5110005220;
        public static string ApplicationId = "76CAC630985EE34CFFB082A55C0EA800";
        public static string SoapUrl = "https://sdk.smarteronlinebanking.com/yodsoap/services";
        public static long TncVersion = 2;
        public static Locale Locale = new Locale {country = "US"};
        public static string CurrencyCode = "USD";
    }
}