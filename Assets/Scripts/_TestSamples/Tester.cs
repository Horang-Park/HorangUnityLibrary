using _TestSamples;
using Modules.InputManager;
using Modules.SoundManager;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Logger = Utilities.Logger;
using SceneManager = Modules.SceneManager.SceneManager;

public class Tester : MonoBehaviour
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
		
		InputManager.Instance.AddMouseInput(this, () =>
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				SceneManager.Instance.LoadScene("1_LoadTestScene 1".Log(LogPriority.Exception).Log(LogPriority.Error), LoadSceneMode.Additive);
				SceneManager.Instance.LoadScene("2_LoadTestScene 2", LoadSceneMode.Additive);
				SceneManager.Instance.LoadScene("3_LoadTestScene 3", LoadSceneMode.Additive);
			
				"f1 누름".Log(LogPriority.Debug);
			}
		});
		
		InputManager.Instance.AddMouseInput(this, null);
		InputManager.Instance.AddMouseInput(null, () => {});
		
		InputManager.Instance.AddKeyboardInput(this, () =>
		{
			if (Input.GetMouseButtonDown(0))
			{
				"마우스 좌클릭".Log(LogPriority.Debug);
			}
		});
	}

	private void Update()
	{
		// if (Input.GetKeyDown(KeyCode.F1))
		// {
		// 	SceneManager.Instance.LoadScene("1_LoadTestScene 1".Log(LogPriority.Exception).Log(LogPriority.Error), LoadSceneMode.Additive);
		// 	SceneManager.Instance.LoadScene("2_LoadTestScene 2", LoadSceneMode.Additive);
		// 	SceneManager.Instance.LoadScene("3_LoadTestScene 3", LoadSceneMode.Additive);
		// 	
		// 	"f1 누름".Log(LogPriority.Debug);
		// }

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

		if (Input.GetKeyDown(KeyCode.F5))
		{
			SoundManager.Instance.Play("EG_LT_DE01 2");
		}
		
		if (Input.GetKeyDown(KeyCode.F6))
		{
			SoundManager.Instance.Pause("EG_LT_DE01 2");
		}
		
		if (Input.GetKeyDown(KeyCode.F7))
		{
			SoundManager.Instance.Resume("EG_LT_DE01 2");
		}
		
		if (Input.GetKeyDown(KeyCode.F8))
		{
			SoundManager.Instance.Stop("EG_LT_DE01 2");
		}

		if (Input.GetKeyDown(KeyCode.F12))
		{
			fsmTestFacade.ChangeState();
		}
	}

	private void OnLoadDone(bool b)
	{
		if (b is true)
		{
			Logger.Log(LogPriority.Debug, $"{b}");
		}
	}

	private void OnUnloadDone(bool b)
	{
		if (b is true)
		{
			Logger.Log(LogPriority.Debug, $"{b}");
		}
	}
}
