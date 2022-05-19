using System.Collections;
using System.Collections.Generic;
using Structural;
using UniRx;
using UnityEngine.SceneManagement;
using Utilities;

namespace Modules.SceneManager
{
	public class SceneManager : MonoSingleton<SceneManager>
	{
		private readonly Dictionary<int, Scene> sceneAssets = new();
		
		private Scene currentActiveScene;
		public Scene CurrentActiveScene => currentActiveScene;

		public BoolReactiveProperty isLoadDone;
		
		public void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
		{
			var key = sceneName.GetHashCode();

			if (IsValidSceneName(key, out var scene) is false)
			{
				Logger.Log(LogPriority.Error, $"{sceneName} 씬을 찾을 수 없습니다.");

				return;
			}

			Observable.FromMicroCoroutine(() => SceneLoad(loadSceneMode, scene));
		}

		private void Awake()
		{
			if (UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings < 1)
			{
				Logger.Log(LogPriority.Error, "BuildSetting에 등록된 Scene이 없습니다.");
				
				return;
			}

			for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
			{
				var value = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				var key = value.name.GetHashCode();
				
				sceneAssets.Add(key, value);
			}
		}

		private void Start()
		{
			currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		}
		

		private IEnumerator SceneLoad(LoadSceneMode loadSceneMode, Scene scene)
		{
			Logger.Log(LogPriority.Debug, $"{scene.name} 씬 로드 시작");

			var asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene.name, loadSceneMode);

			while (asyncLoad.isDone is false)
			{
				yield return null;
			}

			UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
			
			Logger.Log(LogPriority.Debug, $"{scene.name} 씬 로드 끝");
		}

		private bool IsValidSceneName(int key, out Scene scene)
		{
			if (sceneAssets.ContainsKey(key))
			{
				scene = sceneAssets[key];

				return true;
			}

			scene = default;

			return false;
		}

		private bool IsAlreadyLoadedScene(Scene scene)
		{
			return false;
		}
	}
}

// Code Author: ChangsooPark