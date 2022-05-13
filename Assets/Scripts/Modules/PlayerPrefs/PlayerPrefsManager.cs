using Utilities;
using Logger = Utilities.Logger;

namespace Modules.PlayerPrefs
{
	public static class PlayerPrefsManager
	{
		private static GetPlayerPrefs getPlayerPrefs = new();
		public static GetPlayerPrefs GetPlayerPrefs => getPlayerPrefs;

		private static SetPlayerPrefs setPlayerPrefs = new();
		public static SetPlayerPrefs SetPlayerPrefs => setPlayerPrefs;

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
				
				Logger.Log(LoggerPriority.Verbose, $"{key}이(가) 삭제되었습니다.");
			}
			
			Logger.Log(LoggerPriority.Warning, $"{key}이(가) 존재하지 않거나, 이미 삭제되었습니다.");
		}
	}
}