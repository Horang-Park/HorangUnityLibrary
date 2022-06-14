using _TestSamples;
using Modules.InputManager.Interfaces.KeyboardInput;
using Modules.InputManager.Interfaces.MouseInput;
using Modules.SoundManager;
using Modules.StopwatchManager;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Logger = Utilities.Logger;
using SceneManager = Modules.SceneManager.SceneManager;

public class Tester : MonoBehaviour, IMouseButtonDown, IKeyboardKeyDown
{
	private FSMTestFacade fsmTestFacade;

	private void Awake()
	{
		fsmTestFacade = GetComponent(typeof(FSMTestFacade)) as FSMTestFacade;
	}

	private void Start()
	{
		SceneManager.Instance.isLoadDone.Subscribe(OnLoadDone);
		SceneManager.Instance.isUnloadDone.Subscribe(OnUnloadDone);
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

	public void OnKeyboardKeyDown()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			StopwatchManager.Instance.StartStopwatch("Tester");
			StopwatchManager.Instance.SubscribeStopwatchTimeUpdate("Tester", StopwatchSubscription);
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			StopwatchManager.Instance.PauseStopwatch("Tester");
		}
		
		if (Input.GetKeyDown(KeyCode.F3))
		{
			StopwatchManager.Instance.ResumeStopwatch("Tester");
		}
		
		if (Input.GetKeyDown(KeyCode.F4))
		{
			StopwatchManager.Instance.StopStopwatch("Tester").ToString().ToLog(LogPriority.Debug);
		}

		if (Input.GetKeyDown(KeyCode.F5))
		{
			StopwatchManager.Instance.CurrentTime("Tester").ToString().ToLog(LogPriority.Debug);
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
}