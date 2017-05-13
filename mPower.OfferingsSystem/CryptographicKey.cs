using System.Text;

namespace mPower.OfferingsSystem
{
    using System;
    using System.Text.RegularExpressions;


    /// <summary>
    /// Represents a key used in public key cryptography
    /// </summary>
    public class CryptographicKey
    {

        #region Regular Expressions

        private static readonly Regex ReAsciiArmor = new Regex(
            @"^-----BEGIN PGP (PUBLIC|PRIVATE) KEY BLOCK-----\n" +
            @"(?:Version: .*\n)?" +
            @"((?:.*\n)*)" +
            @"-----END PGP (PUBLIC|PRIVATE) KEY BLOCK-----\n?$",

            RegexOptions.Multiline | RegexOptions.Compiled
        );

        private static readonly Regex ReNewlinish = new Regex(@"[\r\n]+");

        #endregion


        public int Bits
        {
            get { return Value.Length; }
        }

        public byte[] Value { get; private set; }

        public CryptographicKey(byte[] value)
        {
            Value = new byte[value.Length];
            value.CopyTo(Value, 0);
        }

        public static CryptographicKey CreateInsecureKey()
        {
            var generator = new Random();
            var keyBytes = new byte[2048];
            generator.NextBytes(keyBytes);

            return new CryptographicKey(keyBytes);
        }


        public static CryptographicKey CreateFromAsc(string textAsAsc)
        {
            var processedText = ReNewlinish.Replace(textAsAsc, "\n");
            var match = ReAsciiArmor.Match(processedText);

            if (!match.Success || match.Groups[1].Value != match.Groups[3].Value)
            {
                throw new ArgumentException("Input is not a valid ASCII-Armored OpenPGP key!");
            }

            var textAsBase64 = match.Groups[2].Value;

            // Remove the checksum
            var reChecksum = new Regex(@"^=[a-zA-Z0-9\/\+]+\s*$", RegexOptions.Multiline);
            var textAsPureBase64 = reChecksum.Replace(textAsBase64, "");

            var textAsBytes = Org.BouncyCastle.Utilities.Encoders.Base64.Decode(textAsPureBase64);
            var key = new CryptographicKey(textAsBytes);

            return key;
        }


        public static CryptographicKey CreateFromString(string textAsString)
        {
            var textAsBytes = Encoding.UTF8.GetBytes(textAsString);
            var key = new CryptographicKey(textAsBytes);

            return key;
        }
    }
}
