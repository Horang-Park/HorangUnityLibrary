using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Modules.InputManager.Interfaces.KeyboardInput;
using Modules.InputManager.Interfaces.MouseInput;
using Structural;
using UniRx;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.InputManager
{
	public class InputManager : MonoSingleton<InputManager>
	{
		private readonly List<(Component, MethodInfo)> inputMouseActions = new();
		private readonly List<(Component, MethodInfo)> inputKeyboardActions = new();

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
			var mouseInputImplementationTypes = AppDomain.CurrentDomain.GetAssemblies()
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
					
					var implementationClassInstance = GameObject.Find(item.FullName).GetComponent(item.FullName);
						
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
			var keyboardInputImplementationTypes = AppDomain.CurrentDomain.GetAssemblies()
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
						
					var implementationClassInstance = GameObject.Find(item.FullName).GetComponent(item.FullName);
							
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