using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Device;

namespace Utilities
{
	public static class Encryption
	{
		private static readonly string DeviceIdentifier = SystemInfo.deviceUniqueIdentifier.Replace("-", string.Empty);
		private static readonly RijndaelManaged RijndaelManaged = CreateRijndaelManaged();
		
		/// <summary>
		/// 암호화
		/// </summary>
		/// <param name="toEncrypt">암호화 할 데이터</param>
		/// <returns>암호화 된 데이터</returns>
		public static string Encrypt(string toEncrypt)
		{
			var toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
			var cTransform = RijndaelManaged.CreateEncryptor();
			var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}
		
		/// <summary>
		/// 복호화
		/// </summary>
		/// <param name="toDecrypt">복호화 할 데이터</param>
		/// <returns>복호화 된 데이터</returns>
		public static string Decrypt(string toDecrypt)
		{
			var toDecryptArray = Convert.FromBase64String(toDecrypt);
			var cTransform = RijndaelManaged.CreateDecryptor();
			var resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
			
			return Encoding.UTF8.GetString(resultArray);
		}
		
		private static RijndaelManaged CreateRijndaelManaged()
		{
			var keyArray = Encoding.UTF8.GetBytes(DeviceIdentifier);
			var result = new RijndaelManaged();

			var newKeysArray = new byte[32];
			Array.Copy(keyArray, 0, newKeysArray, 0, 32);

			result.Key = newKeysArray;
			result.Mode = CipherMode.ECB;
			result.Padding = PaddingMode.PKCS7;
			
			return result;
		}
	}
}

// Code Author: ChangsooPark