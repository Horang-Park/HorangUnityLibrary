using UnityEngine;
using Modules.FSM;
using UniRx;

namespace _TestSamples
{
	public class FSMTestFacade : MonoBehaviour
	{
		public int a;
		
		private TestState testState;
		private ChangedState changedState;
		private IdleState idleState;
		
		private StateMachine stateMachine;

		private bool b;

		private void Awake()
		{
			testState = new TestState();
			changedState = new ChangedState();
			idleState = new IdleState();
			
			stateMachine = gameObject.AddComponent<StateMachine>();

			TestState.a.Subscribe(i => { a = i; });
		}

		private void Start()
		{
			stateMachine.Initialize(testState);
		}

		public void ChangeState()
		{
			stateMachine.ChangeState(b ? testState : changedState);

			b = !b;
		}
	}
}