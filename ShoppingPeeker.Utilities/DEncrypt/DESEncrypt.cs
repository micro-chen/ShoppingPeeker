using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace ShoppingPeeker.Utilities.DEncrypt
{
	/// <summary>
	/// DES加密/解密类。
	/// </summary>
	public class DESEncrypt
	{

        private static readonly string Key = StringExtension.DEFAULT_ENCRYPT_KEY;

        public DESEncrypt()
		{			
		}

		#region ========加密======== 
 
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
		public static string Encrypt(string Text) 
		{
            return Encrypt(Text, Key);
		}
		/// <summary> 
		/// 加密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Encrypt(string Text,string sKey) 
		{
            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(Text);
                provider.Key = ASCIIEncoding.ASCII.GetBytes(sKey.Substring(0, 8));
                provider.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string s = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return s;
            }

        }

        #endregion

        #region ========解密======== 


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Decrypt(string Text) 
		{
            return Decrypt(Text, Key);
		}
		/// <summary> 
		/// 解密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Decrypt(string Text,string sKey) 
		{
            byte[] inputByteArray = Convert.FromBase64String(Text);
            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                provider.Key = ASCIIEncoding.ASCII.GetBytes(sKey.Substring(0, 8));
                provider.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

                MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string s = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return s;
            }
        } 
 
		#endregion 


	}
}
