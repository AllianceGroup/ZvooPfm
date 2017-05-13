using System;
using System.Security.Cryptography;
using System.Text;

namespace mPower.Framework.Utils.Security
{
    public class EncryptionService : IEncryptionService
    {
        private string _hashKey;

        public EncryptionService(string hashKey)
        {
            _hashKey = hashKey;
        }

        public string Decode(string encodedData)
        {
            string rv = "";

            encodedData = encodedData.Replace(" ", "+");

            MACTripleDES mac3des = new MACTripleDES();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            mac3des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_hashKey));

            try
            {
                string dataValue = Encoding.UTF8.GetString(Convert.FromBase64String(encodedData.Split('-')[0]));
                string storedHash = Encoding.UTF8.GetString(Convert.FromBase64String(encodedData.Split('-')[1]));
                string calcHash = Encoding.UTF8.GetString(mac3des.ComputeHash(Encoding.UTF8.GetBytes(dataValue)));
                if (storedHash == calcHash)
                {
                    rv = dataValue;
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }

            return rv;
        }

        public string Encode(string data)
        {
            string rv = "";

            MACTripleDES mac3des = new MACTripleDES();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            mac3des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_hashKey));
            try
            {
                rv = Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) + "-" + Convert.ToBase64String(mac3des.ComputeHash(Encoding.UTF8.GetBytes(data)));
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return rv;
        }
    }
}