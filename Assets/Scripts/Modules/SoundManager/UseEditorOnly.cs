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
			if (IsExistKey(audioClipData.hashKey) is true)
			{
				Logger.Log(LogPriority.Error, $"{audioClipData.name} 오디오 클립은 이미 등록되어 있습니다.");
				
				return;
			}
			
			currentAddedAudioClip.Add(audioClipData);
			
			audioClips.Add(audioClipData.hashKey, audioClipData);
			
			Logger.Log(LogPriority.Debug, $"{audioClipData.name} 오디오 클립이 등록되었습니다.");
		}

		public void RemoveSpecificAudioClip(int key)
		{
			if (IsExistKey(key) is false)
			{
				Logger.Log(LogPriority.Error, $"오디오 클립이 없어 삭제할 수 없습니다. 인스펙터를 확인해주세요. / Code: {key}");
				
				return;
			}

			var removableData = currentAddedAudioClip.Find(match => match.hashKey.Equals(key));
			currentAddedAudioClip.Remove(removableData);
			
			Logger.Log(LogPriority.Debug, "currentAddedAudioClip(List)에서 삭제 완료");

			audioClips.Remove(key);
			
			Logger.Log(LogPriority.Debug, "audioClips(Dictionary)에서 삭제 완료");
		}
		
		public void RemoveAllAudioClip()
		{
			currentAddedAudioClip.Clear();
			audioClips.Clear();
		}

		public void UpdateDictionary()
		{
			if (Instance.currentAddedAudioClip is not null && Instance.currentAddedAudioClip.Count > 0)
			{
				foreach (var clip in Instance.currentAddedAudioClip)
				{
					audioClips.Add(clip.name.GetHashCode(), clip);
				}
			}
		}
	}
}