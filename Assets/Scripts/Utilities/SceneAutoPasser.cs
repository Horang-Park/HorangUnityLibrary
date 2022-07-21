using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;
using SceneManager = Modules.SceneManager.SceneManager;

public class SceneAutoPasser : MonoBehaviour
{
	public string nextSceneName;
	public int delayMillisecond;
	public UnityEvent onMoveNextScene;

	private async void Awake()
	{
		await UniTask.Delay(delayMillisecond);

		SceneManager.Instance.LoadScene(nextSceneName, LoadSceneMode.Single);

		Logger.Log(LogPriority.Information, $"{nextSceneName} 씬으로 이동합니다.");

		onMoveNextScene?.Invoke();
	}
}