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

		private void Awake()
		{
			if (currentAddedAudioClip is not null && currentAddedAudioClip.Count > 0 && audioClipDatas.Count <= 0)
			{
				foreach (var clip in currentAddedAudioClip)
				{
					audioClipDatas.Add(clip.name.GetHashCode(), clip);
				}
			}
		}

		/// <summary>
		/// 오디오 재생
		/// </summary>
		/// <param name="audioClipName">재생할 오디오 클립 이름</param>
		public void Play(string audioClipName, bool useFadeIn = false)
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
			
			if (useFadeIn)
			{
				audioObject.FadeIn();
			}
			else
			{
				audioObject.Play();
			}
		}

		/// <summary>
		/// 오디오 일시정지
		/// </summary>
		/// <param name="audioClipName">일시정지할 오디오 클립 이름 (재생중이어야 작동)</param>
		public void Pause(string audioClipName)
		{
			var hashKey = audioClipName.GetHashCode();
			AudioObject audioObject;

			if (TryGetAudioObject(hashKey, out audioObject) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오를 일시정지 할 수 없습니다.");

				return;
			}
			
			audioObject.Pause();
		}

		/// <summary>
		/// 오디오 재생 재개
		/// </summary>
		/// <param name="audioClipName">다시 재생할 오디오 클립 이름</param>
		public void Resume(string audioClipName)
		{
			var hashKey = audioClipName.GetHashCode();
			AudioObject audioObject;

			if (TryGetAudioObject(hashKey, out audioObject) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오 재생을 재개 할 수 없습니다.");

				return;
			}
			
			audioObject.Resume();
		}

		/// <summary>
		/// 오디오 정지
		/// <br></br>
		/// 오디오 오브젝트를 파괴함
		/// </summary>
		/// <param name="audioClipName">정지할 오디오 클립 이름 (재생중이거나 일시정지 상태여야 작동)</param>
		public void Stop(string audioClipName, bool useFadeOut = false)
		{
			var hashKey = audioClipName.GetHashCode();
			AudioObject audioObject;

			if (TryGetAudioObject(hashKey, out audioObject) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오를 정지 할 수 없습니다.");

				return;
			}

			if (useFadeOut)
			{
				audioObject.FadeOut();
				
				audioObjects.Remove(hashKey);
			}
			else
			{
				audioObject.Stop();
			
				audioObjects.Remove(hashKey);
			}
		}
		
		private bool IsExistAudioClipKey(int key)
		{
			return audioClipDatas.ContainsKey(key);
		}

		private bool IsExistAudioObjectKey(int key)
		{
			return audioObjects.ContainsKey(key);
		}

		private bool TryGetAudioObject(int key, out AudioObject audioObject)
		{
			if (IsExistAudioObjectKey(key) is false)
			{
				Logger.Log(LogPriority.Error, "재생 중이 아니거나, 등록되지 않은 오디오입니다.");

				audioObject = null;

				return false;
			}

			audioObject = audioObjects[key];

			return true;
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