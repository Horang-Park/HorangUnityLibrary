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
	public UnityEvent onMoveNextScene;

	private void Awake()
	{
		Observable.FromCoroutine(MoveToNextScene)
			.DoOnCompleted(() =>
			{
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName);
				
				Logger.Log(LogPriority.Information, $"{nextSceneName} 씬으로 이동합니다.");
				
				onMoveNextScene?.Invoke();
			})
			.Subscribe();
	}

	private IEnumerator MoveToNextScene()
	{
		yield return new WaitForSeconds(delayTime);
	}
}
