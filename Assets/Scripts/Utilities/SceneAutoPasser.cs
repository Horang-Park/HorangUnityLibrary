using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Logger = Utilities.Logger;

public class SceneAutoPasser : MonoBehaviour
{
	public string nextSceneName;
	public float delayTime;
	public UnityEvent OnMoveNextScene;

	private void Awake()
	{
		Observable.FromCoroutine(MoveToNextScene)
			.DoOnCompleted(() =>
			{
				OnMoveNextScene?.Invoke();
				
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName);
				
				Logger.Log(LogPriority.Verbose, $"{nextSceneName} 씬으로 이동합니다.");
			})
			.Subscribe();
	}

	private IEnumerator MoveToNextScene()
	{
		yield return new WaitForSeconds(delayTime);
	}
}
