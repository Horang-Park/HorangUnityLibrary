using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Modules.PlayerPrefs
{
	public class SetPlayerPrefs
	{
		public void SetInt(string key, int value)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var encryptedValue = Utilities.Encryption.Encrypt(value.ToString());
			
			UnityEngine.PlayerPrefs.SetString(encryptedKey, encryptedValue);
		}

		public void SetInt(string key, int[] value)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var encryptedValue = ArrayToEncryptedString(value);
			
			UnityEngine.PlayerPrefs.SetString(encryptedKey, encryptedValue);
		}
		
		public void SetFloat(string key, float value)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var encryptedValue = Utilities.Encryption.Encrypt(((double)value).ToString(CultureInfo.InvariantCulture));
			
			UnityEngine.PlayerPrefs.SetString(encryptedKey, encryptedValue);
		}
		
		public void SetFloat(string key, float[] value)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var encryptedValue = ArrayToEncryptedString(value);
			
			UnityEngine.PlayerPrefs.SetString(encryptedKey, encryptedValue);
		}
		
		public void SetString(string key, string value)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var encryptedValue = Utilities.Encryption.Encrypt(value);
			
			UnityEngine.PlayerPrefs.SetString(encryptedKey, encryptedValue);
		}

		private static string ArrayToEncryptedString<T>(IEnumerable<T> value)
		{
			var sb = new StringBuilder();
			
			foreach (var item in value)
			{
				sb.Append(item);
				sb.Append(PlayerPrefsConstants.ArraySeparator);
			}
			
			return Utilities.Encryption.Encrypt(sb.ToString());
		}
	}
}

// Code Author: ChangsooPark