using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Modules.InputManager.Interfaces.KeyboardInput;
using Modules.InputManager.Interfaces.MouseInput;
using Structural;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.InputManager
{
	public class InputManager : MonoSingleton<InputManager>
	{
		private readonly List<(object, MethodInfo)> inputMouseActions = new();
		private readonly List<(object, MethodInfo)> inputKeyboardActions = new();

		/*[HideInInspector]*/ public bool blockMouseInput;
		/*[HideInInspector]*/ public bool blockKeyboardInput;

		protected override async void Awake()
		{
			base.Awake();

			await UniTask.RunOnThreadPool(FindMouseInputImplementations);
			await UniTask.RunOnThreadPool(FindKeyboardInputImplementations);
		}

		private void Start()
		{
			UniTask.RunOnThreadPool(MouseUpdate);
			UniTask.RunOnThreadPool(KeyboardUpdate);
		}

		private async UniTask MouseUpdate()
		{
			while (true)
			{
				if (blockMouseInput)
				{
					await UniTask.Yield();

					continue;
				}

				foreach (var item in inputMouseActions)
				{
					item.Item2.Invoke(item.Item1, null);
				}

				await UniTask.Yield();
			}
		}

		private async UniTask KeyboardUpdate()
		{
			while (true)
			{
				if (blockKeyboardInput)
				{
					await UniTask.Yield();

					continue;
				}

				foreach (var item in inputKeyboardActions)
				{
					item.Item2.Invoke(item.Item1, null);
				}

				await UniTask.Yield();
			}
		}

		private async UniTask FindMouseInputImplementations()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var mouseInputType = typeof(IMouseInput);
			var mouseInputImplementationTypes = await GetTypes();
			
			async UniTask<IEnumerable<Type>> GetTypes()
			{
				var types = await UniTask.RunOnThreadPool(() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()));
					
				return types.Where(p => mouseInputType.IsAssignableFrom(p) && !p.IsInterface && p.IsClass && !p.IsAbstract);
			}

			foreach (var item in mouseInputImplementationTypes)
			{
				foreach (var implementationMethod in item.GetMethods())
				{
					if (!implementationMethod.Name.Contains("Mouse"))
					{
						continue;
					}

					// 예외처리
					if (item.FullName is null)
					{
						Logger.Log(LogPriority.Exception, "메서드의 정보를 가져올 수 없습니다.");

						throw new NullReferenceException();
					}

					var instanceType = Type.GetType(item.FullName);

					if (instanceType is null)
					{
						Logger.Log(LogPriority.Exception, $"{item.FullName}의 타입을 가져올 수 없습니다.");

						throw new NullReferenceException();
					}

					// 인스턴스 생성
					var implementationClassInstance = (object)GameObject.Find(item.FullName)?.GetComponent(item.FullName) ?? Activator.CreateInstance(instanceType);

					inputMouseActions.Add((implementationClassInstance, implementationMethod));
				}
			}

			stopwatch.Stop();
			Logger.Log(LogPriority.Information, $"Mouse input 초기화 완료까지 걸린 시간: {stopwatch.ElapsedMilliseconds}ms");
			stopwatch.Reset();

			await UniTask.CompletedTask;
		}

		private async UniTask FindKeyboardInputImplementations()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var keyboardInputType = typeof(IKeyboardInput);
			var keyboardInputImplementationTypes = await GetTypes();
			
			async UniTask<IEnumerable<Type>> GetTypes()
			{
				var types = await UniTask.RunOnThreadPool(() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()));
					
				return types.Where(p => keyboardInputType.IsAssignableFrom(p) && !p.IsInterface && p.IsClass && !p.IsAbstract);
			}

			foreach (var item in keyboardInputImplementationTypes)
			{
				foreach (var implementationMethod in item.GetMethods())
				{
					if (!implementationMethod.Name.Contains("Keyboard"))
					{
						continue;
					}

					// 예외처리
					if (item.FullName is null)
					{
						Logger.Log(LogPriority.Exception, "메서드의 정보를 가져올 수 없습니다.");

						throw new NullReferenceException();
					}

					var instanceType = Type.GetType(item.FullName);

					if (instanceType is null)
					{
						Logger.Log(LogPriority.Exception, $"{item.FullName}의 타입을 가져올 수 없습니다.");

						throw new NullReferenceException();
					}

					// 인스턴스 생성
					var implementationClassInstance = (object)GameObject.Find(item.FullName)?.GetComponent(item.FullName) ?? Activator.CreateInstance(instanceType);

					inputKeyboardActions.Add((implementationClassInstance, implementationMethod));
				}
			}

			stopwatch.Stop();
			Logger.Log(LogPriority.Information, $"Keyboard input 초기화 완료까지 걸린 시간: {stopwatch.ElapsedMilliseconds}ms");
			stopwatch.Reset();
			
			await UniTask.CompletedTask;
		}
	}
}