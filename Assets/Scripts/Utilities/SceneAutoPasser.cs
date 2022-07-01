using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

public class SceneAutoPasser : MonoBehaviour
{
	public string nextSceneName;
	public int delayMillisecond;
	public UnityEvent onMoveNextScene;

	private async void Awake()
	{
		await UniTask.Delay(delayMillisecond);

		await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName);

		Logger.Log(LogPriority.Information, $"{nextSceneName} 씬으로 이동합니다.");

		onMoveNextScene?.Invoke();
	}
}