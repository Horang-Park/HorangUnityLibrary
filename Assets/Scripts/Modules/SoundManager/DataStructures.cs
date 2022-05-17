using UnityEngine;

namespace Modules.SoundManager
{
	public enum AudioClipType
	{
		BGM,
		StoppableSFX,
		LoopableSFX,
		OneShotSFX,
	}
		
	[System.Serializable]
	public struct AudioClipData
	{
		public AudioClipType type;
		public AudioClip clip;
		public string name;
	}
}