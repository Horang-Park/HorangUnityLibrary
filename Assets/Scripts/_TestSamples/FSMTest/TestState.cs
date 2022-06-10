using Modules.FSM;
using UniRx;
using UnityEngine;
using Utilities;

namespace _TestSamples
{
	public class TestState : State
	{
		public static IntReactiveProperty a = new ();
		
		public override void Enter()
		{
			StateName = "TestState";
		}

		public override void LogicUpdate()
		{
			a.Value += 1;
		}

		public override void Exit()
		{
			$"{StateName} Exit".Log(LogPriority.Debug);
		}
	}
	
	public class ChangedState : State
	{
		public override void Enter()
		{
			StateName = "ChangedState";
		}

		public override void LogicUpdate()
		{
			TestState.a.Value -= 1;
		}

		public override void Exit()
		{
			$"{StateName} Exit".Log(LogPriority.Debug);
		}
	}
}