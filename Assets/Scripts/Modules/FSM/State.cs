using UnityEngine;

namespace Modules.FSM
{
	public abstract class State
	{
		public string StateName { protected set; get; }

		/// <summary>
		/// 상태 들어갈 때 실행
		/// </summary>
		public abstract void Enter();
		
		/// <summary>
		/// 상태 전환 완료한 후 실행
		/// </summary>
		public virtual void LateInitialize() {}
		
		/// <summary>
		/// Input 관련 처리할 때 사용 (Update)
		/// </summary>
		public virtual void HandleInput() {}
		
		/// <summary>
		/// 로직관련 처리할 때 사용 (Update)
		/// </summary>
		public virtual void LogicUpdate() {}
		
		/// <summary>
		/// 물리처리가 필요할 때 사용 (FixedUpdate)
		/// </summary>
		public virtual void PhysicsUpdate() {}
		
		/// <summary>
		/// Unity의 LateUpdate에 해당 (LateUpdate)
		/// </summary>
		public virtual void LateUpdate() {}
		
		/// <summary>
		/// 상태 나오기 전 처리할 때 사용
		/// </summary>
		public virtual void BeforeExit() {}

		/// <summary>
		/// 상태 나올 때 사용
		/// </summary>
		public abstract void Exit();
	}
}

// Code Author: ChangsooPark