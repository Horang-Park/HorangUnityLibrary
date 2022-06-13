using System;
using System.Collections;
using System.Text;
using UniRx;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.SoundManager
{
	public class SoundObject : MonoBehaviour
	{
		private AudioClipData audioClipData;
		private AudioSource audioSource;
		
		private float targetVolume;
		private bool fromPause;

		public readonly Subject<float> CurrentPlayTime = new();

		public void Initialize(AudioClipData clipData)
		{
			var sb = new StringBuilder("SoundObject [");
			sb.Append(clipData.name);
			sb.Append(']');

			gameObject.name = sb.ToString();
			audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

			targetVolume = clipData.volume;

			if (audioSource is null || !audioSource)
			{
				Logger.Log(LogPriority.Error, $"{gameObject.name} 게임 오브젝트에 AudioSource 컴포넌트를 추가할 수 없습니다.");

				return;
			}

			audioSource.playOnAwake = false;
			audioSource.clip = clipData.clip;
			audioSource.volume = clipData.volume;
			audioSource.panStereo = clipData.panning;

			switch (clipData.category)
			{
				case AudioClipCategory.BGM:
				case AudioClipCategory.LoopableSFX:
					audioSource.loop = true;
					break;
				case AudioClipCategory.OneShotSFX:
				case AudioClipCategory.QuestionSFX:
					audioSource.loop = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			Stop();

			sb.Clear();
		}

		public void Play()
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);

				audioSource.Play();

				fromPause = false;
			}
			else
			{
				Logger.Log(LogPriority.Warning, $"{audioSource.gameObject.name}의 상태가 Stop이 아닙니다. Play는 Stop 후, 사용할 수 있습니다.");
			}

			Observable.FromCoroutine<float>(PlayTimeUpdater)
				.Subscribe(t => CurrentPlayTime.OnNext(t))
				.AddTo(gameObject);
		}

		public void Pause()
		{
			audioSource.Pause();

			fromPause = true;
		}

		public void Resume()
		{
			if (fromPause)
			{
				audioSource.Play();
				
				fromPause = false;
			}
			else
			{
				Logger.Log(LogPriority.Warning, $"{audioSource.gameObject.name}의 상태가 Pause가 아닙니다. Resume은 Pause 후, 사용할 수 있습니다.");
			}
		}

		public void Stop()
		{
			audioSource.Stop();

			fromPause = false;
			
			gameObject.SetActive(false);
		}

		public void FadeOut()
		{
			if (!audioSource.isPlaying)
			{
				Logger.Log(LogPriority.Warning, $"{gameObject.name} 오디오가 재생중이지 않아 페이드 아웃을 적용할 수 없습니다.");

				return;
			}

			Observable.FromMicroCoroutine(FadeOutCoroutine).Subscribe();
		}

		public void FadeIn()
		{
			audioSource.volume = 0.0f;

			Observable.FromMicroCoroutine(FadeInCoroutine).Subscribe();
		}

		public void Mute()
		{
			audioSource.mute = true;
		}

		public void Unmute()
		{
			audioSource.mute = false;
		}

		private IEnumerator FadeOutCoroutine()
		{
			while (audioSource.volume > 0.0f)
			{
				audioSource.volume -= Time.deltaTime;
				
				yield return null;
			}
			
			Stop();
		}

		private IEnumerator FadeInCoroutine()
		{
			Play();
			
			while (audioSource.volume < targetVolume)
			{
				audioSource.volume += Time.deltaTime;
				
				yield return null;
			}
		}

		private IEnumerator PlayTimeUpdater(IObserver<float> observer)
		{
			while (true)
			{
				if (!audioSource.isPlaying)
				{
					yield return null;

					continue;
				}

				//CurrentPlayTime.OnNext(audioSource.time);
				observer.OnNext(audioSource.time);

				yield return null;
			}
		}
	}
}

// Code Author: ChangsooPark