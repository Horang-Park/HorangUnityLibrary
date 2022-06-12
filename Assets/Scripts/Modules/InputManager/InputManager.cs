using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Structural;
using UniRx;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.InputManager
{
	public class InputManager : MonoSingleton<InputManager>
	{
		private readonly List<Action> inputMouse = new();
		private readonly List<Action> inputKeyboard = new();

		[HideInInspector] public bool blockMouseInput;
		[HideInInspector] public bool blockKeyboardInput;

		public void AddMouseInput(object sender, Action action)
		{
			if (sender is null)
			{
				Logger.Log(LogPriority.Exception, $"sender는 null 일 수 없습니다. Mouse Input 등록에 실패했습니다. 로그를 참조해주세요.");

				throw new ArgumentNullException();
			}
			
			if (action is null)
			{
				Logger.Log(LogPriority.Exception, $"action은 null 일 수 없습니다. Mouse Input 등록에 실패했습니다. 로그를 참조해주세요.");

				throw new ArgumentNullException();
			}
			
			inputMouse.Add(action);
			
			Logger.Log(LogPriority.Verbose, $"마우스 입력 액션이 등록되었습니다. / methodName: {action.Method.Name}, from: {sender.GetType().Name}");
		}
		
		public void AddKeyboardInput(object sender, [NotNull] Action action)
		{
			if (sender is null)
			{
				Logger.Log(LogPriority.Exception, $"sender는 null 일 수 없습니다. Keyboard Input 등록에 실패했습니다.");

				throw new ArgumentNullException();
			}
			
			if (action is null)
			{
				Logger.Log(LogPriority.Exception, $"action은 null 일 수 없습니다. Keyboard Input 등록에 실패했습니다.");

				throw new ArgumentNullException();
			}
			
			inputKeyboard.Add(action);
			
			Logger.Log(LogPriority.Verbose, $"키보드 입력 액션이 등록되었습니다. / methodName: {action.Method.Name}, from: {sender.GetType().Name}");
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

				foreach (var item in inputMouse)
				{
					item?.Invoke();
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

				foreach (var item in inputKeyboard)
				{
					item?.Invoke();
				}
					
				yield return null;
			}
		}
	}
}