using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Modules.PlayerPrefs
{
	public class GetPlayerPrefs
	{
		public int GetInt(string key)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var decryptedValue = Utilities.Encryption.Decrypt(UnityEngine.PlayerPrefs.GetString(encryptedKey));

			return int.Parse(decryptedValue);
		}

		public List<int> GetIntArray(string key)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			
			return EncryptedStringToDecryptedList<int>(encryptedKey);
		}
		
		public float GetFloat(string key)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var decryptedValue = Utilities.Encryption.Decrypt(UnityEngine.PlayerPrefs.GetString(encryptedKey));
			
			return float.Parse(decryptedValue);
		}
		
		public List<float> GetFloatArray(string key)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			
			return EncryptedStringToDecryptedList<float>(encryptedKey);
		}
		
		public string GetString(string key)
		{
			var encryptedKey = Utilities.Encryption.Encrypt(key);
			var decryptedValue = Utilities.Encryption.Decrypt(UnityEngine.PlayerPrefs.GetString(encryptedKey));

			return decryptedValue;
		}

		private static List<T> EncryptedStringToDecryptedList<T>(string key)
		{
			var encryptedValue = UnityEngine.PlayerPrefs.GetString(key);
			var decrypt = Utilities.Encryption.Decrypt(encryptedValue).Split(PlayerPrefsConstants.ArraySeparator);
			var converter = TypeDescriptor.GetConverter(typeof(T));

			return (from item
				in decrypt
				where !string.IsNullOrEmpty(item)
				select (T)converter.ConvertFrom(item)).ToList();
		}
	}
}

// Code Author: ChangsooPark