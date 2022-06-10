using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Structural;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.SceneManager
{
	public class SceneManager : MonoSingleton<SceneManager>
	{
		private readonly List<string> sceneNames = new();
		private readonly Stack<Scene> sceneStack = new();

		private Scene currentActiveScene;

		public ReactiveProperty<Scene> CurrentActiveScene;
		public BoolReactiveProperty isLoadDone;
		public BoolReactiveProperty isUnloadDone;

		/// <summary>
		/// 씬 로드
		/// </summary>
		/// <param name="sceneName">로드할 씬 이름</param>
		/// <param name="loadSceneMode">로드 모드</param>
		public void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
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

			Observable.FromMicroCoroutine(() => SceneLoad(loadSceneMode, sceneName)).Subscribe();
		}

		/// <summary>
		/// 씬 언로드 (씬 로드한 순서대로 언로드 함)
		/// </summary>
		public void UnloadScene()
		{
			if (sceneStack.Count == 1)
			{
				Logger.Log(LogPriority.Error, "마지막 씬은 언로드 할 수 없습니다.");

				return;
			}

			var scene = sceneStack.Pop();

			Observable.FromMicroCoroutine(() => SceneUnload(scene)).Subscribe();
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

		private void Start()
		{
			currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

			sceneStack.Push(currentActiveScene);
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

			currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

			sceneStack.Push(scene);

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

				currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			}

			isUnloadDone.Value = false;
		}

		private bool IsValidSceneName(string sceneName) => sceneNames.Contains(sceneName);

		private static bool IsAlreadyLoadedScene(string sceneName) => UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded;
	}
}

// Code Author: ChangsooPark