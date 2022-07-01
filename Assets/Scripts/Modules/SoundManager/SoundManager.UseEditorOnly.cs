using Utilities;
using Utilities.Debug;

namespace Modules.SoundManager
{
	/// <summary>
	/// USE EDITOR ONLY
	/// </summary>
	public partial class SoundManager
	{
		public static void AddAudioClip(AudioClipData audioClipData)
		{
			if (Instance.IsExistAudioClipInList(audioClipData.hashKey))
			{
				Logger.Log(LogPriority.Warning, $"{audioClipData.name} 오디오 클립은 이미 등록되어 있습니다.");
				
				return;
			}
			
			Instance.currentAddedAudioClip.Add(audioClipData);
			
			Instance.audioClipDatas.Add(audioClipData.hashKey, audioClipData);
			
			Logger.Log(LogPriority.Information, $"{audioClipData.name} 오디오 클립이 등록되었습니다.");
		}

		public static bool RemoveSpecificAudioClip(int key)
		{
			if (Instance.IsExistAudioClipInList(key) is false)
			{
				Logger.Log(LogPriority.Error, $"오디오 클립이 없어 삭제할 수 없습니다. 인스펙터를 확인해주세요. / Code: {key}");

				return false;
			}

			var removableData = Instance.currentAddedAudioClip.Find(match => match.hashKey.Equals(key));
			Instance.currentAddedAudioClip.Remove(removableData);

			return true;
		}
		
		public static void RemoveAllAudioClip()
		{
			Instance.currentAddedAudioClip.Clear();
			Instance.audioClipDatas.Clear();
		}
	}
}

// Code Author: ChangsooPark