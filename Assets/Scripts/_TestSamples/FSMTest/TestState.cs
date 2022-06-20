using Modules.FSM;
using Modules.InputManager.Interfaces.KeyboardInput;
using UniRx;
using UnityEngine;
using Utilities;

namespace _TestSamples
{
	public class TestState : State
	{
		public static readonly IntReactiveProperty a = new ();
		
		public override void Enter()
		{
			StateName = "TestState";
		}

		public override void LogicUpdate()
		{
			a.Value += 1;
		}
		
		public override void BeforeExit()
		{
			a.Value = 0;
		}

		public override void Exit()
		{
			$"{StateName} Exit".ToLog(LogPriority.Debug);
		}
	}
	
	public class ChangedState : State
	{
		public override void Enter()
		{
			StateName = "ChangedState";
		}

		public override void Exit()
		{
			$"{StateName} Exit".ToLog(LogPriority.Debug);
		}
	}

	public class IdleState : State
	{
		public override void Enter()
		{
		}

		public override void Exit()
		{
		}
	}
}