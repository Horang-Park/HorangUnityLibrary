using Utilities;
using Logger = Utilities.Logger;

namespace Modules.PlayerPrefs
{
	public static class PlayerPrefsManager
	{
		public static GetPlayerPrefs GetPlayerPrefs { get; } = new();
		public static SetPlayerPrefs SetPlayerPrefs { get; } = new();

		/// <summary>
		/// 키가 존재하는지 확인
		/// </summary>
		/// <param name="key">키</param>
		/// <returns>존재하면 true, 존재하지 않으면 false</returns>
		public static bool IsValidKey(string key)
		{
			var encryptedKey = UnityEngine.PlayerPrefs.GetString(key);

			return UnityEngine.PlayerPrefs.HasKey(encryptedKey);
		}

		/// <summary>
		/// 키 삭제
		/// </summary>
		/// <param name="key">키</param>
		public static void DeleteKey(string key)
		{
			var encryptedKey = UnityEngine.PlayerPrefs.GetString(key);

			if (IsValidKey(encryptedKey))
			{
				UnityEngine.PlayerPrefs.DeleteKey(encryptedKey);
				
				Logger.Log(LogPriority.Verbose, $"{key}이(가) 삭제되었습니다.");
			}
			
			Logger.Log(LogPriority.Warning, $"{key}이(가) 존재하지 않거나, 이미 삭제되었습니다.");
		}

		/// <summary>
		/// 키 전체 삭제
		/// </summary>
		public static void DeleteEveryKey()
		{
			UnityEngine.PlayerPrefs.DeleteAll();
			
			Logger.Log(LogPriority.Verbose, $"PlayerPrefs 데이터가 모두 삭제되었습니다.");
		}
	}
}

// Code Author: ChangsooPark