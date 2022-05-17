using System.Collections.Generic;
using Structural;
using UnityEngine;

namespace Modules.SoundManager
{
	public partial class SoundManager : MonoSingleton<SoundManager>
	{
		[Header("정상 등록 확인용")]
		[SerializeField] [ReadOnly] private List<AudioClipData> currentAddedAudioClip = new();

		private readonly Dictionary<int, AudioClipData> audioClips = new();
		
		private bool IsExistKey(int key)
		{
			return audioClips.ContainsKey(key);
		}
	}
}