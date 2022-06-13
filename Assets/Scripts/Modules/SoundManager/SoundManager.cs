using System;
using System.Collections.Generic;
using System.Linq;
using Structural;
using UniRx;
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
		private readonly Dictionary<int, SoundObject> soundObjects = new();
		private readonly Dictionary<AudioClipCategory, List<SoundObject>> soundObjectsByCategory = new();

		#region 기본 사용 인터페이스

		/// <summary>
		/// 오디오 재생
		/// </summary>
		/// <param name="audioClipName">재생할 오디오 클립 이름</param>
		/// <param name="getPlayTime">현재 오디오 재생 시간 갖고오는 콜백 함수</param>
		/// <param name="useFadeIn">페이드 인 사용 여부</param>
		/// <returns>Disposable로써, 반환된 값을 이용하여 Subscription을 취소 (Stop 호출 후 Dispose)</returns>
		public IDisposable Play(string audioClipName, Action<float> getPlayTime = null, bool useFadeIn = false)
		{
			var hashKey = audioClipName.GetHashCode();
			
			SoundObject soundObject;

			if (IsExistAudioClipKey(hashKey) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오 클립을 찾을 수 없습니다. 인스펙터 혹은 작성한 코드를 확인해주세요.");

				return null;
			}
			
			var clipData = audioClipDatas[hashKey];
			
			if (IsExistSoundObjectKey(hashKey) is false)
			{
				Logger.Log(LogPriority.Verbose, $"{clipData.name}의 사운드 객체가 없어, 새로 생성합니다.");

				soundObject = CreateAudioObject(clipData);
			}
			else
			{
				soundObject = soundObjects[hashKey];
			}
			
			var disposable = soundObject.CurrentPlayTime
				.Subscribe(t => getPlayTime?.Invoke(t));
			
			if (useFadeIn)
			{
				soundObject.FadeIn();
			}
			else
			{
				soundObject.Play();
			}

			return disposable;
		}

		/// <summary>
		/// 오디오 일시정지
		/// </summary>
		/// <param name="audioClipName">일시정지할 오디오 클립 이름 (재생중이어야 작동)</param>
		public void Pause(string audioClipName)
		{
			var hashKey = audioClipName.GetHashCode();

			if (TryGetAudioObject(hashKey, out var soundObject) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오를 일시정지 할 수 없습니다.");

				return;
			}
			
			soundObject.Pause();
		}

		/// <summary>
		/// 오디오 재생 재개
		/// </summary>
		/// <param name="audioClipName">다시 재생할 오디오 클립 이름</param>
		public void Resume(string audioClipName)
		{
			var hashKey = audioClipName.GetHashCode();

			if (TryGetAudioObject(hashKey, out var soundObject) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오 재생을 재개 할 수 없습니다.");

				return;
			}
			
			soundObject.Resume();
		}

		/// <summary>
		/// 오디오 정지
		/// </summary>
		/// <param name="audioClipName">정지할 오디오 클립 이름 (재생중이거나 일시정지 상태여야 작동)</param>
		/// <param name="useFadeOut">페이드 아웃 사용 여부</param>
		public void Stop(string audioClipName, bool useFadeOut = false)
		{
			var hashKey = audioClipName.GetHashCode();

			if (TryGetAudioObject(hashKey, out var soundObject) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오를 정지 할 수 없습니다.");

				return;
			}

			if (useFadeOut)
			{
				soundObject.FadeOut();
			}
			else
			{
				soundObject.Stop();
			}
		}
		
		#endregion

		#region 부가 인터페이스

		/// <summary>
		/// 카테고리에 해당하는 사운드 오브젝트 음소거
		/// </summary>
		/// <param name="audioClipCategory">카테고리</param>
		public void MuteByCategory(AudioClipCategory audioClipCategory)
		{
			foreach (var soundObject in soundObjectsByCategory[audioClipCategory])
			{
				soundObject.Mute();
			}
		}

		/// <summary>
		/// 카테고리에 해당하는 사운드 오브젝트 음소거 해제
		/// </summary>
		/// <param name="audioClipCategory">카테고리</param>
		public void UnmuteByCategory(AudioClipCategory audioClipCategory)
		{
			foreach (var soundObject in soundObjectsByCategory[audioClipCategory])
			{
				soundObject.Unmute();
			}
		}

		/// <summary>
		/// 오디오 길이 반환
		/// </summary>
		/// <param name="audioClipName">알고 싶은 오디오 클립 이름</param>
		/// <returns>오디오 클립의 길이</returns>
		public float GetAudioLength(string audioClipName)
		{
			var hashKey = audioClipName.GetHashCode();

			if (IsExistAudioClipKey(hashKey) is false)
			{
				Logger.Log(LogPriority.Error, $"{audioClipName} 오디오 클립을 찾을 수 없습니다. 인스펙터 혹은 작성한 코드를 확인해주세요.");

				return -1.0f;
			}
			
			var clipData = audioClipDatas[hashKey];

			return clipData.clip.length;
		}
		
		#endregion

		#region 내부 사용 메서드

		protected override void Awake()
		{
			base.Awake();
			
			if (currentAddedAudioClip is not null && currentAddedAudioClip.Count > 0 && audioClipDatas.Count <= 0)
			{
				foreach (var clip in currentAddedAudioClip)
				{
					audioClipDatas.Add(clip.name.GetHashCode(), clip);
				}
			}

			foreach (var type in Enum.GetValues(typeof(AudioClipCategory)))
			{
				soundObjectsByCategory.Add((AudioClipCategory)type, new List<SoundObject>());
			}
		}

		private bool IsExistAudioClipInList(int key)
		{
			return currentAddedAudioClip.Any(audioClip => audioClip.hashKey.Equals(key));
		}
		
		private bool IsExistAudioClipKey(int key)
		{
			return audioClipDatas.ContainsKey(key);
		}

		private bool IsExistSoundObjectKey(int key)
		{
			return soundObjects.ContainsKey(key);
		}

		private bool TryGetAudioObject(int key, out SoundObject soundObject)
		{
			if (IsExistSoundObjectKey(key) is false)
			{
				Logger.Log(LogPriority.Error, "재생 중이 아니거나, 등록되지 않은 오디오입니다.");

				soundObject = null;

				return false;
			}

			soundObject = soundObjects[key];

			return true;
		}

		private SoundObject CreateAudioObject(AudioClipData audioClipData)
		{
			var go = new GameObject();
			go.transform.SetParent(gameObject.transform);

			if (go.AddComponent(typeof(SoundObject)) is not SoundObject co || !co)
			{
				Logger.Log(LogPriority.Error, $"{audioClipData.name}의 오디오 객체를 생성할 수 없습니다.");

				return null;
			}
			
			co.Initialize(audioClipData);
			
			soundObjects.Add(audioClipData.hashKey, co);
			soundObjectsByCategory[audioClipData.category].Add(co);

			return co;
		}
		
		#endregion
	}
}

// Code Author: ChangsooPark