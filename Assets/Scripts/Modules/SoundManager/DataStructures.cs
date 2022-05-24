using UnityEngine;

namespace Modules.SoundManager
{
	public enum AudioClipCategory
	{
		BGM,
		LoopableSFX,
		OneShotSFX,
		QuestionSFX,
	}
		
	[System.Serializable]
	public struct AudioClipData
	{
		public AudioClipCategory category;
		public AudioClip clip;
		public int hashKey;
		
		public string name;
		public float volume;
		public float panning;
	}
}

// Code Author: ChangsooPark