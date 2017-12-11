using ShoppingPeeker.Utilities.DataStructure;
using System;

namespace ShoppingPeeker.Utilities.DEncrypt
{
    public class EncryptionService
    {

        #region 字段
        /// <summary>
        /// 用户设置的加密私钥 键值名
        /// </summary>
        private const string EncryptionKeyName = "EncryptionKey";
        /// <summary>
        /// 如果用户未设置 那么使用默认的私钥
        /// </summary>
        private readonly string _defaultEncrytionKeyValue = StringExtension.DEFAULT_ENCRYPT_KEY;
        #endregion


        #region  属性




        private string _encryptionKeyValue;
        /// <summary>
        /// 数据库中用户设置的加密密钥
        /// </summary>
        public string EncryptionKeyValue
        {
            get
            {
                if (null == _encryptionKeyValue)
                {
                    //查询出私钥
                    //if (null != dal_Setting)
                    //{
                    //    var setting = dal_Setting.GetElementsByCondition(x => x.Name == EncryptionKeyName).FirstOrDefault();
                    //    if (null != setting)
                    //    {
                    //        _encryptionKeyValue = setting.Value;
                    //    }
                    //}
                    if (string.IsNullOrEmpty(_encryptionKeyValue))
                    {
                        _encryptionKeyValue = this._defaultEncrytionKeyValue;
                    }
                }
                return _encryptionKeyValue;

            }
            set { _encryptionKeyValue = value; }
        }


        #endregion


        public EncryptionService()
        {

        }



        ///// <summary>
        ///// Encrypt text
        ///// </summary>
        ///// <param name="plainText">Text to encrypt</param>
        ///// <param name="encryptionPrivateKey">Encryption private key</param>
        ///// <returns>Encrypted text</returns>
        //public virtual string EncryptText(string plainText, string encryptionPrivateKey = "")
        //{
        //    if (string.IsNullOrEmpty(plainText))
        //        return plainText;

        //    if (String.IsNullOrEmpty(encryptionPrivateKey))
        //        encryptionPrivateKey = this.EncryptionKeyValue;

        //    var tDESalg = new TripleDESCryptoServiceProvider();
        //    tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
        //    tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

        //    byte[] encryptedBinary = EncryptTextToMemory(plainText, tDESalg.Key, tDESalg.IV);
        //    return Convert.ToBase64String(encryptedBinary);
        //}

        ///// <summary>
        ///// Decrypt text
        ///// </summary>
        ///// <param name="cipherText">Text to decrypt</param>
        ///// <param name="encryptionPrivateKey">Encryption private key</param>
        ///// <returns>Decrypted text</returns>
        //public virtual string DecryptText(string cipherText, string encryptionPrivateKey = "")
        //{
        //    if (String.IsNullOrEmpty(cipherText))
        //        return cipherText;

        //    if (String.IsNullOrEmpty(encryptionPrivateKey))
        //        encryptionPrivateKey = this.EncryptionKeyValue;

        //    var tDESalg = new TripleDESCryptoServiceProvider();
        //    tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
        //    tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

        //    byte[] buffer = Convert.FromBase64String(cipherText);
        //    return DecryptTextFromMemory(buffer, tDESalg.Key, tDESalg.IV);
        //}

        //#region Utilities


        ///// <summary>
        ///// 加密数据到字节
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="key"></param>
        ///// <param name="iv"></param>
        ///// <returns></returns>
        //private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write))
        //        {
        //            byte[] toEncrypt = new UnicodeEncoding().GetBytes(data);
        //            cs.Write(toEncrypt, 0, toEncrypt.Length);
        //            cs.FlushFinalBlock();
        //        }

        //        return ms.ToArray();
        //    }
        //}

        ///// <summary>
        ///// 解密数据还原字符串
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="key"></param>
        ///// <param name="iv"></param>
        ///// <returns></returns>
        //private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        //{
        //    using (var ms = new MemoryStream(data))
        //    {
        //        using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
        //        {
        //            var sr = new StreamReader(cs, new UnicodeEncoding());
        //            return sr.ReadLine();
        //        }
        //    }
        //}

        //#endregion

        /// <summary>
        /// 生成一个票据
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public AuthenticationTicket GenerateFormsAuthenticationTicket(string userData, TimeSpan timeOut)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new AuthenticationTicket() { UserData = userData, Expiration = now.Add(timeOut) };

            return ticket;
        }

        /// <summary>
        /// 加密表单验证的票据
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public string EncryptFormsAuthenticationTicket(AuthenticationTicket ticket)
        {
            var jsonTicket = ticket.ToJson();
            return DESEncrypt.Encrypt(jsonTicket,this.EncryptionKeyValue);
        }

        /// <summary>
        /// 将加密的票据  解密处理
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        public AuthenticationTicket DecryptFormsAuthenticationTicket(string encryptedTicket)
        {
            AuthenticationTicket ticket = null;

            try
            {
                //解密出来json
                var jsonTicket = DESEncrypt.Decrypt(encryptedTicket,this.EncryptionKeyValue);
                ticket = jsonTicket.FromJson<AuthenticationTicket>();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ticket;
        }
    }
}
