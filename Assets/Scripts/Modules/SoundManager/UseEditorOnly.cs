using Utilities;

namespace Modules.SoundManager
{
	/// <summary>
	/// USE EDITOR ONLY
	/// </summary>
	public partial class SoundManager
	{
		public void AddAudioClip(AudioClipData audioClipData)
		{
			if (IsValidKey(audioClipData.name) is false)
			{
				return;
			}
			
			currentAddedAudioClip.Add(audioClipData);
			
			var key = audioClipData.name.GetHashCode();
			
			audioClips.Add(key, audioClipData);
			
			Logger.Log(LogPriority.Debug, $"{audioClipData.name} 오디오 클립이 등록되었습니다.");
		}
		
		public void RemoveAllAudioClip()
		{
			currentAddedAudioClip.Clear();
			audioClips.Clear();
		}

		public static void UpdateDictionary()
		{
			if (Instance.currentAddedAudioClip is not null && Instance.currentAddedAudioClip.Count > 0)
			{
				foreach (var clip in Instance.currentAddedAudioClip)
				{
					Instance.audioClips.Add(clip.name.GetHashCode(), clip);
				}
			}
		}
	}
}