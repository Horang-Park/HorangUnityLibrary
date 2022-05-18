﻿using System;
using System.Collections;
using System.Text;
using UniRx;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.SoundManager
{
	public class AudioObject : MonoBehaviour
	{
		private AudioClipData audioClipData;
		private AudioSource audioSource;
		
		private float targetVolume;
		private bool fromPause;

		public void Initialize(AudioClipData clipData)
		{
			var sb = new StringBuilder("AudioObject [");
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

			switch (clipData.type)
			{
				case AudioClipType.BGM:
				case AudioClipType.LoopableSFX:
					audioSource.loop = true;
					break;
				case AudioClipType.OneShotSFX:
					audioSource.loop = false;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			sb.Clear();
		}

		public void Play()
		{
			audioSource.Play();

			fromPause = false;
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
			}
		}

		public void Stop()
		{
			Destroy(gameObject);
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
	}
}