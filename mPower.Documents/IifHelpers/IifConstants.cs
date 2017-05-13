namespace mPower.Documents.IifHelpers
{
    internal class IifConstants
    {
        public const char HeaderSign = '!';
        public const char Separator = '\t';
        public const string DateFormat = "MM/dd/yy";

        public class Tranaction
        {
            // keys
            public const string Key = "TRNS";
            public const string EndKey = "ENDTRNS";

            // data columns
            public const string Id = "TRNSID";
            public const string Type = "TRNSTYPE";
            public const string Date = "DATE";
            public const string AccountName = "ACCNT";
            public const string Amount = "AMOUNT";
            public const string Name = "NAME";
            public const string Memo = "MEMO";
        }

        public class Spl
        {
            public const string Key = "SPL";

            // data columns
            public const string Id = "SPLID";
            public const string Type = "TRNSTYPE";
            public const string Date = "DATE";
            public const string AccountName = "ACCNT";
            public const string Amount = "AMOUNT";
            public const string Name = "NAME";
            public const string Memo = "MEMO";
        }

        public class Account
        {
            public const string Key = "ACCNT";

            // data columns
            public const string Name = "NAME";
            public const string Type = "ACCNTTYPE";
            public const string Description = "DESC";
        }
    }
}
