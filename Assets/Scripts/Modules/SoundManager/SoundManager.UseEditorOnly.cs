using Utilities;

namespace Modules.SoundManager
{
	/// <summary>
	/// USE EDITOR ONLY
	/// </summary>
	public partial class SoundManager
	{
		public static void AddAudioClip(AudioClipData audioClipData)
		{
			if (Instance.IsExistAudioClipKey(audioClipData.hashKey) is true)
			{
				Logger.Log(LogPriority.Warning, $"{audioClipData.name} 오디오 클립은 이미 등록되어 있습니다.");
				
				return;
			}
			
			Instance.currentAddedAudioClip.Add(audioClipData);
			
			Instance.audioClipDatas.Add(audioClipData.hashKey, audioClipData);
			
			Logger.Log(LogPriority.Verbose, $"{audioClipData.name} 오디오 클립이 등록되었습니다.");
		}

		public static bool RemoveSpecificAudioClip(int key)
		{
			if (Instance.IsExistAudioClipKey(key) is false)
			{
				Logger.Log(LogPriority.Error, $"오디오 클립이 없어 삭제할 수 없습니다. 인스펙터를 확인해주세요. / Code: {key}");

				return false;
			}

			var removableData = Instance.currentAddedAudioClip.Find(match => match.hashKey.Equals(key));
			Instance.currentAddedAudioClip.Remove(removableData);
			
			Logger.Log(LogPriority.Debug, "currentAddedAudioClip(List)에서 삭제 완료");

			Instance.audioClipDatas.Remove(key);
			
			Logger.Log(LogPriority.Debug, "audioClipDatas(Dictionary)에서 삭제 완료");

			return true;
		}
		
		public static void RemoveAllAudioClip()
		{
			Instance.currentAddedAudioClip.Clear();
			Instance.audioClipDatas.Clear();
		}
	}
}