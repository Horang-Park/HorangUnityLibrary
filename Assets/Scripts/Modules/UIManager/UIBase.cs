using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

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

		protected virtual void OnShowInitialize()
		{
			Logger.Log(LogPriority.Information, $"{gameObject.name} UI가 켜지고 초기화 되었습니다.");
		}

		protected virtual void OnHideInitialize()
		{
			Logger.Log(LogPriority.Information, $"{gameObject.name} UI가 숨겨지고 초기화 되었습니다.");
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