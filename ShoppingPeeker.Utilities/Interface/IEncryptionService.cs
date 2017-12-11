using System;

namespace ShoppingPeeker.Utilities.Interface
{
    public interface IEncryptionService 
    {
        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        string CreateSaltKey(int size);

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1");

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        string EncryptText(string plainText, string encryptionPrivateKey = "");

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        string DecryptText(string cipherText, string encryptionPrivateKey = "");


        ///// <summary>
        ///// 获取一个表单票据
        ///// </summary>
        ///// <param name="userData"></param>
        ///// <returns></returns>
        //FormsAuthenticationTicket GenerateFormsAuthenticationTicket(string userData);

        ///// <summary>
        ///// 加密表单票据
        ///// </summary>
        ///// <param name="ticket"></param>
        ///// <returns></returns>
        //string EncryptFormsAuthenticationTicket(FormsAuthenticationTicket ticket);

        ///// <summary>
        ///// 解密一个加密过的票据
        ///// </summary>
        ///// <param name="ticket"></param>
        ///// <returns></returns>
        //FormsAuthenticationTicket DecryptFormsAuthenticationTicket(string encryptedTicket);
    }
}
