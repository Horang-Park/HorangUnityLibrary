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

		private readonly Dictionary<int, AudioClipData> audioClipDatas = new();
		private readonly Dictionary<int, AudioObject> audioObjects = new();

		public void Play(string audioClipName)
		{
			var hashKey = audioClipName.GetHashCode();
			
			AudioObject audioObject;
			AudioClipData clipData;

			if (IsExistAudioClipKey(hashKey) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오 클립을 찾을 수 없습니다. 인스펙터 혹은 작성한 코드를 확인해주세요.");

				return;
			}
			
			clipData = audioClipDatas[hashKey];
			
			if (IsExistAudioObjectKey(hashKey) is false)
			{
				Logger.Log(LogPriority.Verbose, $"{clipData.name}의 오디오 객체가 없어, 새로 생성합니다.");

				audioObject = CreateAudioObject(clipData);
			}
			else
			{
				audioObject = audioObjects[hashKey];
			}
			
			audioObject.Play();
		}

		public void Pause(string audioClipName)
		{
			
		}
		
		private bool IsExistAudioClipKey(int key)
		{
			return audioClipDatas.ContainsKey(key);
		}

		private bool IsExistAudioObjectKey(int key)
		{
			return audioObjects.ContainsKey(key);
		}

		private AudioObject CreateAudioObject(AudioClipData audioClipData)
		{
			var go = new GameObject();
			go.transform.SetParent(gameObject.transform);

			if (go.AddComponent(typeof(AudioObject)) is not AudioObject co || !co)
			{
				Logger.Log(LogPriority.Error, $"{audioClipData.name}의 오디오 객체를 생성할 수 없습니다.");

				return null;
			}
			
			co.Initialize(audioClipData);
			
			audioObjects.Add(audioClipData.hashKey, co);

			return co;
		}
	}
}