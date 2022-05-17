using System.Collections.Generic;
using Structural;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.SoundManager
{
	public partial class SoundManager : MonoSingleton<SoundManager>
	{
		[Header("정상 등록 확인용")]
		[SerializeField] [ReadOnly] private List<AudioClipData> currentAddedAudioClip = new();

		private readonly Dictionary<int, AudioClipData> audioClips = new();
		
		private bool IsValidKey(string key)
		{
			var hashKey = key.GetHashCode();

			if (!audioClips.ContainsKey(hashKey))
			{
				return true;
			}
			
			Logger.Log(LogPriority.Error, $"{key} 오디오 클립은 이미 등록되어 있습니다.");

			return false;
		}
	}
}