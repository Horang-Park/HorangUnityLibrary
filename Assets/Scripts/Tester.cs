using System;
using Modules.SoundManager;
using Modules.UIManager;
using UniRx;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

public class Tester : MonoBehaviour
{
	private void Awake()
	{
		UIManager.Instance.isDataProcessingComplete.Subscribe(b => { Logger.Log(LogPriority.Debug, "UIManager Data Postprocess->초기화 완료"); });
	}

	private void Start()
	{
		SoundManager.Instance.Play("EG_LT_DE01 3");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			SoundManager.Instance.Pause("EG_LT_DE01 3");
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			SoundManager.Instance.Resume("EG_LT_DE01 3");
		}

		if (Input.GetKeyDown(KeyCode.F12))
		{
			SoundManager.Instance.Stop("EG_LT_DE01 3");
		}
	}
}
