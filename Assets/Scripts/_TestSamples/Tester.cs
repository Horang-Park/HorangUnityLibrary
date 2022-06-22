using System;
using System.Collections.Generic;
using _TestSamples;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Modules.API;
using Modules.CameraManager;
using Modules.InputManager.Interfaces.KeyboardInput;
using Modules.InputManager.Interfaces.MouseInput;
using Modules.SoundManager;
using Modules.StopwatchManager;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Utilities;
using Logger = Utilities.Logger;
using SceneManager = Modules.SceneManager.SceneManager;

public class Tester : MonoBehaviour, IMouseButtonDown, IKeyboardKeyDown
{
	private FSMTestFacade fsmTestFacade;

	public CameraElement mainCamera;

	private void Awake()
	{
		fsmTestFacade = GetComponent(typeof(FSMTestFacade)) as FSMTestFacade;
	}

	private async void Start()
	{
		SceneManager.Instance.isLoadDone.Subscribe(OnLoadDone);
		SceneManager.Instance.isUnloadDone.Subscribe(OnUnloadDone);
		SceneManager.Instance.currentActiveScene.Subscribe(OnSceneChange);

		var www = UnityWebRequest.Get("https://api.biboboo.com/api/splash");
		var res = await Requester.Send(www, msg => msg.ToLog(LogPriority.Error));

		res.ToLog(LogPriority.Debug);

		CameraManager.Instance.AddCamera("Main Camera", mainCamera);
	}

	private void OnLoadDone(bool b)
	{
		// if (b)
		// {
		// 	Logger.Log(LogPriority.Debug, $"{b}");
		// }
	}

	private void OnUnloadDone(bool b)
	{
		// if (b)
		// {
		// 	Logger.Log(LogPriority.Debug, $"{b}");
		// }
	}

	private void StopwatchSubscription(long t)
	{
		Logger.Log(LogPriority.Debug, $"{t}");
	}

	public void OnMouseButtonDown()
	{
		if (Input.GetMouseButtonDown(0))
		{
			"MouseDown".ToLog(LogPriority.Debug);
		}
	}

	private void OnSceneChange(Scene observer)
	{
		Logger.Log(LogPriority.Debug, $"SceneName: {observer.name} / ScenePath: {observer.path}");
	}

	public void OnKeyboardKeyDown()
	{
		if (Input.GetKeyDown(KeyCode.F5))
		{
			UniTask.Create(GetSprite3);
		}
		
		// if (Input.GetKeyDown(KeyCode.F1))
		// {
		// 	var path = Application.streamingAssetsPath + "/SampleZipZip.zip";
		// 	var files = Compression.GetFileListInZip(path);
		// 	
		// 	foreach (var name in files)
		// 	{
		// 		name.ToLog(LogPriority.Debug);
		// 	}
		// }
		
		// if (Input.GetKeyDown(KeyCode.F2))
		// {
		// 	var src = Application.streamingAssetsPath + "/SampleZipZip.zip";
		// 	var des = Application.streamingAssetsPath;
		// 	var files = Compression.Unzip(src, des);
		// }
		
		// if (Input.GetKeyDown(KeyCode.F1))
		// {
		// 	StopwatchManager.Instance.StartStopwatch("Tester");
		// 	StopwatchManager.Instance.SubscribeStopwatchTimeUpdate("Tester", StopwatchSubscription);
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F2))
		// {
		// 	StopwatchManager.Instance.PauseStopwatch("Tester");
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F3))
		// {
		// 	StopwatchManager.Instance.ResumeStopwatch("Tester");
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F4))
		// {
		// 	StopwatchManager.Instance.StopStopwatch("Tester").ToString().ToLog(LogPriority.Debug);
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F5))
		// {
		// 	StopwatchManager.Instance.CurrentTime("Tester").ToString().ToLog(LogPriority.Debug);
		// }
		
		if (Input.GetKeyDown(KeyCode.F1))
		{
			SceneManager.Instance.LoadScene("1_LoadTestScene 1", LoadSceneMode.Additive);
			SceneManager.Instance.LoadScene("2_LoadTestScene 2", LoadSceneMode.Additive);
			SceneManager.Instance.LoadScene("3_LoadTestScene 3", LoadSceneMode.Additive);
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			SceneManager.Instance.UnloadScene();
		}
		if (Input.GetKeyDown(KeyCode.F3))
		{
			SceneManager.Instance.LoadScene("2_LoadTestScene 2", LoadSceneMode.Single);
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			SceneManager.Instance.LoadScene("0_SampleTestScene", LoadSceneMode.Single);
		}

		// if (Input.GetKeyDown(KeyCode.F2))
		// {
		// 	SceneManager.Instance.UnloadScene();
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F3))
		// {
		// 	SceneManager.Instance.LoadScene("2_LoadTestScene 2", LoadSceneMode.Single);
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F4))
		// {
		// 	SceneManager.Instance.LoadScene("0_SampleTestScene", LoadSceneMode.Single);
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F5))
		// {
		// 	// SoundManager.Instance.Play("EG_LT_DE01 2", f =>
		// 	// {
		// 	// 	Logger.Log(LogPriority.Debug, $"Tester:Update -> {f}");
		// 	// });
		//
		// 	SoundManager.Instance.fadeInSpeedMultiplier = 0.1f;
		// 	SoundManager.Instance.Play("EG_LT_DE01 2");
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F6))
		// {
		// 	SoundManager.Instance.Pause("EG_LT_DE01 2");
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F7))
		// {
		// 	SoundManager.Instance.Resume("EG_LT_DE01 2");
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F8))
		// {
		// 	SoundManager.Instance.fadeOutSpeedMultiplier = 0.1f;
		// 	SoundManager.Instance.Stop("EG_LT_DE01 2");
		// }
		//
		// if (Input.GetKeyDown(KeyCode.F12))
		// {
		// 	fsmTestFacade.ChangeState();
		// }
	}

	private async UniTask GetSprite1()
	{
		var s = await LoadImage.LoadFromSpecificPath(Application.streamingAssetsPath + "/illust_bottom.png");

		s.rect.width.ToString().ToLog(LogPriority.Debug);
		s.rect.height.ToString().ToLog(LogPriority.Debug);
	}
	
	private async UniTask GetSprite2()
	{
		var s = await LoadImage.LoadFromResources("illust_bottom");

		s.rect.width.ToString().ToLog(LogPriority.Debug);
		s.rect.height.ToString().ToLog(LogPriority.Debug);
	}
	
	private async UniTask GetSprite3()
	{
		List<string> paths = new();
		
		paths.Add("Sprites/illust_bottom");
		paths.Add("Sprites/bg_aispeaking_ln_r");
		paths.Add("Sprites/book_inside");
		paths.Add("Sprites/image_mask");

		var s = await LoadImage.LoadManyFromResources(paths);
		
		foreach (var ss in s)
		{
			ss.rect.width.ToString().ToLog(LogPriority.Debug);
			ss.rect.height.ToString().ToLog(LogPriority.Debug);
		}
	}
}