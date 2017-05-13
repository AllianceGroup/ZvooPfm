namespace mPower.Framework.Utils.Security
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Decodes data that has been encrypted.
        /// </summary>
        /// <param name="encodedData">The encrypted data to decrypt.</param>
        /// <returns>A String that represents the decrypted data.</returns>
        string Decode(string encodedData);

        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted value.</returns>
        string Encode(string data);
    }
}