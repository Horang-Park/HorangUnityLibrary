using UnityEngine;

namespace Modules.SoundManager
{
	public enum AudioClipType
	{
		BGM,
		LoopableSFX,
		OneShotSFX,
	}
		
	[System.Serializable]
	public struct AudioClipData
	{
		public AudioClipType type;
		public AudioClip clip;
		public int hashKey;
		
		public string name;
		public float volume;
		public float panning;
	}
}

// Code Author: ChangsooPark