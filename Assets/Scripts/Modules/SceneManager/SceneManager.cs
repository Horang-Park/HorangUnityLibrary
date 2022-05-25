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
				Logger.Log(LogPriority.Error, "씬이 하나밖에 남지 않아 언로드를 할 수 없습니다.");

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
			Logger.Log(LogPriority.Debug, $"{sceneName} 씬 로드 시작");

			var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

			while (ao.isDone is false)
			{
				yield return null;
			}

			var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);

			isLoadDone.Value = true;

			if (scene.name.Equals("Modules") is false)
			{
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);

				currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

				sceneStack.Push(scene);
			}

			Logger.Log(LogPriority.Debug, $"{scene.name} 씬 로드 끝");

			isLoadDone.Value = false;
		}

		private IEnumerator SceneUnload(Scene scene)
		{
			Logger.Log(LogPriority.Debug, $"{scene.name} 씬 언로드 시작");

			var ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);

			while (ao.isDone is false)
			{
				yield return null;
			}

			var lastScene = sceneStack.First();
			
			if (lastScene.isLoaded)
			{
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(lastScene);

				currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			}

			isUnloadDone.Value = true;

			Logger.Log(LogPriority.Debug, $"{scene.name} 씬 언로드 끝");

			isUnloadDone.Value = false;
		}

		private bool IsValidSceneName(string sceneName) => sceneNames.Contains(sceneName);

		private static bool IsAlreadyLoadedScene(string sceneName) => UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).isLoaded;
	}
}

// Code Author: ChangsooPark