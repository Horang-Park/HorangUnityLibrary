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

			OnAfterShowInitialize();
		}

		public virtual void Hide()
		{
			if (gameObject.activeSelf is false)
			{
				return;
			}

			OnAfterHideInitialize();
			
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

		protected virtual void OnAfterShowInitialize()
		{
			Logger.Log(LogPriority.Verbose, $"{gameObject.name} UI가 켜지고 초기화 되었습니다.");
		}

		protected virtual void OnAfterHideInitialize()
		{
			Logger.Log(LogPriority.Verbose, $"{gameObject.name} UI가 숨겨지고 초기화 되었습니다.");
		}

		protected virtual void OnInitialize()
		{
			Logger.Log(LogPriority.Verbose, $"{gameObject.name} UI가 초기화 되었습니다.");
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