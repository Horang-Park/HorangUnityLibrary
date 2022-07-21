using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Structural;
using UniRx;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

namespace Modules.SceneManager
{
	public class SceneManager : MonoSingleton<SceneManager>
	{
		private readonly List<string> sceneNames = new();
		private readonly Stack<Scene> sceneStack = new();

		public ReactiveProperty<Scene> currentActiveScene = new();
		public BoolReactiveProperty isLoadDone;
		public BoolReactiveProperty isUnloadDone;

		/// <summary>
		/// 씬 로드
		/// </summary>
		/// <param name="sceneName">로드할 씬 이름</param>
		/// <param name="loadSceneMode">로드 모드</param>
		public async void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
		{
			if (IsValidSceneName(sceneName) is false)
			{
				Logger.Log(LogPriority.Error, $"{sceneName} 씬을 찾을 수 없습니다.");

				return;
			}

			if (IsAlreadyLoadedScene(sceneName))
			{
				Logger.Log(LogPriority.Error, $"{sceneName} 씬은 이미 로딩되어 있습니다.");

				return;
			}

			if (loadSceneMode == LoadSceneMode.Single)
			{
				sceneStack.Clear();
			}

			await SceneLoad(loadSceneMode, sceneName);
		}

		/// <summary>
		/// 씬 언로드 (씬 로드한 순서대로 언로드 함)
		/// </summary>
		public async void UnloadScene()
		{
			if (sceneStack.Count == 1)
			{
				Logger.Log(LogPriority.Error, "마지막 씬은 언로드 할 수 없습니다.");
				
				currentActiveScene.Value = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

				return;
			}

			var scene = sceneStack.Pop();

			await SceneUnload(scene);
		}

		/// <summary>
		/// 현재 활성화 된 씬을 현재 씬으로 세팅
		/// </summary>
		public void SetCurrentScene()
		{
			currentActiveScene.Value = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			
			sceneStack.Push(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}

		protected override void Awake()
		{
			base.Awake();
			
			if (UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings < 1)
			{
				Logger.Log(LogPriority.Error, "BuildSetting에 등록된 Scene이 없습니다.");

				return;
			}

			for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
			{
				var value = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));

				sceneNames.Add(value);
			}
		}

		private IEnumerator SceneLoad(LoadSceneMode loadSceneMode, string sceneName)
		{
			var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

			while (ao.isDone is false)
			{
				yield return null;
			}

			var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);

			isLoadDone.Value = true;
			
			UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);

			currentActiveScene.Value = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			
			if (IsAlreadyContainScene(currentActiveScene.Value) is false)
			{
				sceneStack.Push(scene);
			}

			isLoadDone.Value = false;
		}

		private IEnumerator SceneUnload(Scene scene)
		{
			var ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);

			while (ao.isDone is false)
			{
				yield return null;
			}
			
			isUnloadDone.Value = true;

			var lastScene = sceneStack.First();
			
			if (lastScene.isLoaded)
			{
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(lastScene);

				currentActiveScene.Value = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			}

			isUnloadDone.Value = false;
		}

		private bool IsValidSceneName(string sceneName) => sceneNames.Contains(sceneName);

		private static bool IsAlreadyLoadedScene(string sceneName) => UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded;

		private bool IsAlreadyContainScene(Scene scene) => sceneStack.Contains(scene);
	}
}

// Code Author: ChangsooPark