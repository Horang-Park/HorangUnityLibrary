using System;
using Modules.UIManager;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utilities;
using Logger = Utilities.Logger;

public class Tester : MonoBehaviour
{
	private void Awake()
	{
		UIManager.Instance.isDataProcessingComplete.Subscribe(b => { Logger.Log(LogPriority.Debug, "UIManager Data Postprocess->초기화 완료"); });
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			var item = UIManager.Instance.GetUIBase<UIBase>("UI1");
			
			item.Show();
		}
		else if (Input.GetKeyDown(KeyCode.F2))
		{
			var item = UIManager.Instance.GetUIBase<UIBase>("UI2");
			
			item.Show();
		}
		else if (Input.GetKeyDown(KeyCode.F3))
		{
			var item = UIManager.Instance.GetUIBase<UIBase>("UI3");
			
			item.Show();
		}
		else if (Input.GetKeyDown(KeyCode.F4))
		{
			var item = UIManager.Instance.GetUIBase<UIBase>("UI4");
			
			item.Show();
		}
		else if (Input.GetKeyDown(KeyCode.F12))
		{
			UIManager.Instance.PutBackUIBase();
		}
	}
}
