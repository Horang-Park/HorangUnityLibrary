using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.SceneManager
{
	public class SceneHandler : MonoBehaviour
	{
		[ReadOnly] public int sceneId;
		
		[ReadOnly] public FloatReactiveProperty loadPercentage = new();
		[ReadOnly] public BoolReactiveProperty loadDone = new();

		private (string, int)? sceneName;
		private Scene thisScene;

		public virtual void Initialize()
		{
			if (sceneId.Equals(0) && sceneName is null)
			{
				Logger.Log(LogPriority.Error, "Scene ID와 Scene Name이 세팅되지 않았습니다. 초기화를 진행할 수 없습니다.");

				return;
			}
			
			Logger.Log(LogPriority.Verbose, $"{sceneName?.Item1} 씬이 초기화되었습니다.");
		}

		public virtual void SceneLoad(LoadSceneMode loadSceneMode)
		{
			thisScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName?.Item1);
			
			Observable.FromMicroCoroutine(() => SceneLoading(loadSceneMode)).Subscribe();
		}

		public virtual void SceneUnload()
		{
			UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(thisScene);

			loadDone.Value = false;
			
			thisScene = default;
		}

		private IEnumerator SceneLoading(LoadSceneMode loadSceneMode)
		{
			Logger.Log(LogPriority.Verbose, $"{sceneName?.Item1} 씬 로드를 시작합니다.");

			var asyncSceneLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName?.Item1, loadSceneMode);

			asyncSceneLoad.allowSceneActivation = false;

			while (asyncSceneLoad.isDone is false)
			{
				loadPercentage.Value = asyncSceneLoad.progress;
				
				yield return null;
			}

			asyncSceneLoad.allowSceneActivation = true;

			UnityEngine.SceneManagement.SceneManager.SetActiveScene(thisScene);

			loadDone.Value = true;
		}
	}
}