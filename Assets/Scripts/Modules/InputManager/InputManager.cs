using System;
using System.Collections;
using System.Collections.Generic;
using Structural;
using UniRx;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.InputManager
{
	public enum InputActionType
	{
		Down,
		Up,
		Press,
	}
	
	public class InputManager : MonoSingleton<InputManager>
	{
		private readonly Dictionary<(int, InputActionType), List<Action>> inputMouse = new();
		private readonly Dictionary<(KeyCode, InputActionType), List<Action>> inputKeyboard = new();

		[HideInInspector] public bool blockMouseInput;
		[HideInInspector] public bool blockKeyboardInput;

		public void AddMouseInput(object sender, int mouseButtonIndex, InputActionType inputActionType, Action action)
		{
			if (sender is null)
			{
				Logger.Log(LogPriority.Exception, "sender는 null 일 수 없습니다. Mouse Input 등록에 실패했습니다. 로그를 참조해주세요.");

				throw new ArgumentNullException();
			}
			
			if (action is null)
			{
				Logger.Log(LogPriority.Exception, "action은 null 일 수 없습니다. Mouse Input 등록에 실패했습니다. 로그를 참조해주세요.");

				throw new ArgumentNullException();
			}

			var key = (mouseButtonIndex, inputActionType);

			List<Action> actions;

			if (inputMouse.ContainsKey(key))
			{
				actions = inputMouse[key];
				actions.Add(action);

				inputMouse[key] = actions;
			}
			else
			{
				actions = new List<Action> {action};

				inputMouse.Add((mouseButtonIndex, inputActionType), actions);
			}
			
			Logger.Log(LogPriority.Verbose, $"마우스 입력 액션이 등록되었습니다. / methodName: {action.Method.Name}, from: {sender.GetType().Name}");
		}
		
		public void AddKeyboardInput(object sender, KeyCode keyCode, InputActionType inputActionType, Action action)
		{
			if (sender is null)
			{
				Logger.Log(LogPriority.Exception, "sender는 null 일 수 없습니다. Keyboard Input 등록에 실패했습니다.");

				throw new ArgumentNullException();
			}
			
			if (action is null)
			{
				Logger.Log(LogPriority.Exception, "action은 null 일 수 없습니다. Keyboard Input 등록에 실패했습니다.");

				throw new ArgumentNullException();
			}
			
			var key = (keyCode, inputActionType);
			
			List<Action> actions;

			if (inputKeyboard.ContainsKey(key))
			{
				actions = inputKeyboard[key];
				actions.Add(action);

				inputKeyboard[key] = actions;
			}
			else
			{
				actions = new List<Action> {action};

				inputKeyboard.Add((keyCode, inputActionType), actions);
			}
			
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
				
				// 코드 추가
					
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
				
				// 코드 추가
					
				yield return null;
			}
		}
	}
}