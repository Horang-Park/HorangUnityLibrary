using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Logger = Utilities.Logger;
using SceneManager = Modules.SceneManager.SceneManager;

public class Tester : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		SceneManager.Instance.isLoadDone.Subscribe(OnLoadDone);
		SceneManager.Instance.isUnloadDone.Subscribe(OnUnloadDone);
	}

	private void Update()
	{
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
		
		if (Input.GetKeyDown(KeyCode.F5))
		{
			SceneManager.Instance.LoadScene("0_SampleTestScene", LoadSceneMode.Single);
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
