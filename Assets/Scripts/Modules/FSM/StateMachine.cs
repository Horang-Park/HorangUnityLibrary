using System;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.FSM
{
	public class StateMachine : MonoBehaviour
	{
		public State CurrentState { private set; get; }

		private bool isInitialized;
		
		public void Initialize(State startState)
		{
			if (isInitialized)
			{
				Logger.Log(LogPriority.Warning, $"[{gameObject.name}]의 StateMachine은 초기화가 이미 되어있습니다.");

				return;
			}
			
			CurrentState = startState;
			CurrentState.Enter();

			Logger.Log(LogPriority.Verbose, $"[{CurrentState.StateName}] 상태로 초기화 및 FSM 시작되었습니다.");
			
			CurrentState.LateInitialize();
		}

		public void ChangeState(State newState)
		{
			CurrentState.BeforeExit();
			
			var beforeStateName = CurrentState.StateName;
			
			CurrentState.Exit();

			CurrentState = newState;
			CurrentState.Enter();
			
			Logger.Log(LogPriority.Verbose, $"[{beforeStateName}] 상태에서 [{CurrentState.StateName}] 상태로 변경되었습니다.");
			
			CurrentState.LateInitialize();
		}

		private void Update()
		{
			CurrentState.HandleInput();
			CurrentState.LogicUpdate();
		}

		private void FixedUpdate()
		{
			CurrentState.PhysicsUpdate();
		}

		private void LateUpdate()
		{
			CurrentState.LateUpdate();
		}
	}
}