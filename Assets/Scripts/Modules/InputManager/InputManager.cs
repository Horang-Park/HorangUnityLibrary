using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Modules.InputManager.Interfaces.KeyboardInput;
using Modules.InputManager.Interfaces.MouseInput;
using Structural;
using UniRx;
using UnityEditor;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.InputManager
{
	public class InputManager : MonoSingleton<InputManager>
	{
		private readonly List<(object, MethodInfo)> inputMouseActions = new();
		private readonly List<(object, MethodInfo)> inputKeyboardActions = new();

		[HideInInspector] public bool blockMouseInput;
		[HideInInspector] public bool blockKeyboardInput;

		protected override void Awake()
		{
			base.Awake();

			Observable.FromMicroCoroutine(FindMouseInputImplementations).Subscribe().AddTo(this);
			Observable.FromMicroCoroutine(FindKeyboardInputImplementations).Subscribe().AddTo(this);
		}

		private void Start()
		{
			Observable.FromMicroCoroutine(MouseUpdate).Subscribe().AddTo(this);
			Observable.FromMicroCoroutine(KeyboardUpdate).Subscribe().AddTo(this);
		}

		private IEnumerator MouseUpdate()
		{
			while (true)
			{
				if (blockMouseInput)
				{
					yield return null;

					continue;
				}

				foreach (var item in inputMouseActions)
				{
					item.Item2.Invoke(item.Item1, null);
				}

				yield return null;
			}
		}

		private IEnumerator KeyboardUpdate()
		{
			while (true)
			{
				if (blockKeyboardInput)
				{
					yield return null;

					continue;
				}

				foreach (var item in inputKeyboardActions)
				{
					item.Item2.Invoke(item.Item1, null);
				}

				yield return null;
			}
		}

		private IEnumerator FindMouseInputImplementations()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var mouseInputType = typeof(IMouseInput);
			var mouseInputImplementationTypes = System.AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => mouseInputType.IsAssignableFrom(p) && !p.IsInterface && p.IsClass && !p.IsAbstract);

			foreach (var item in mouseInputImplementationTypes)
			{
				foreach (var implementationMethod in item.GetMethods())
				{
					if (!implementationMethod.Name.Contains("Mouse"))
					{
						yield return null;

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

					yield return null;
				}
			}

			stopwatch.Stop();
			Logger.Log(LogPriority.Verbose, $"Mouse input 초기화 완료까지 걸린 시간: {stopwatch.ElapsedMilliseconds}ms");
			stopwatch.Reset();
		}

		private IEnumerator FindKeyboardInputImplementations()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var keyboardInputType = typeof(IKeyboardInput);
			var keyboardInputImplementationTypes = System.AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => keyboardInputType.IsAssignableFrom(p) && !p.IsInterface && p.IsClass && !p.IsAbstract);

			foreach (var item in keyboardInputImplementationTypes)
			{
				foreach (var implementationMethod in item.GetMethods())
				{
					if (!implementationMethod.Name.Contains("Keyboard"))
					{
						yield return null;

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

					yield return null;
				}
			}

			stopwatch.Stop();
			Logger.Log(LogPriority.Verbose, $"Keyboard input 초기화 완료까지 걸린 시간: {stopwatch.ElapsedMilliseconds}ms");
			stopwatch.Reset();
		}
	}
}