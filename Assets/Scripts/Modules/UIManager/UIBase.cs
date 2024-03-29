﻿using UnityEngine;
using Utilities;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

namespace Modules.UIManager
{
	public class UIBase : MonoBehaviour
	{
		public bool IsVisible => gameObject.activeSelf;

		private bool isInitialized;
		
		public virtual void Show()
		{
			if (gameObject.activeSelf)
			{
				return;
			}
			
			gameObject.SetActive(true);

			OnShowInitialize();
		}

		public virtual void Hide()
		{
			if (gameObject.activeSelf is false)
			{
				return;
			}

			OnHideInitialize();
			
			gameObject.SetActive(false);
		}

		protected virtual void Awake()
		{
			Initialize();
		}

		protected virtual void OnEnable()
		{
			Initialize();
		}

		/// <summary>
		/// UI가 활성화 될 때 마다 실행
		/// </summary>
		protected virtual void OnShowInitialize()
		{
			Logger.Log(LogPriority.Information, $"{gameObject.name} UI가 켜지고 초기화 되었습니다.");
		}

		/// <summary>
		/// UI가 비활성화 될 때 마다 실행
		/// </summary>
		protected virtual void OnHideInitialize()
		{
			Logger.Log(LogPriority.Information, $"{gameObject.name} UI가 초기화되고 꺼졌습니다.");
		}

		/// <summary>
		/// Awake 또는 Enable 될 때 최초 1회만 실행
		/// </summary>
		protected virtual void OnInitialize()
		{
			Logger.Log(LogPriority.Information, $"{gameObject.name} UI가 초기화 되었습니다.");
		}

		private void Initialize()
		{
			if (isInitialized)
			{
				return;
			}
			
			OnInitialize();

			isInitialized = true;
		}
	}
}

// Code Author: ChangsooPark