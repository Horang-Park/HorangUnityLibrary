using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

namespace Utilities.UnityComponentExtension
{
	[RequireComponent(typeof(Animator))]
	public class AnimatorWrapper : MonoBehaviour
	{
		private Animator animator;

		private readonly Dictionary<int, AnimatorControllerParameterType> animatorParameterTypes = new();

		/// <summary>
		/// 애니메이터 상태 지정
		/// </summary>
		/// <param name="parameterName">상태 지정할 파라미터 이름</param>
		/// <param name="setValue">지정할 값</param>
		/// <typeparam name="T">int, float, bool 형식</typeparam>
		/// <exception cref="ArgumentException">parameterName 매개변수가 잘못되었을 경우</exception>
		public void SetAnimatorState<T>(string parameterName, T setValue = default) where T : IConvertible
		{
			var hashKey = Animator.StringToHash(parameterName);

			if (!IsParameterNameValid(hashKey))
			{
				Logger.Log(LogPriority.Exception, $"{gameObject.name}의 Animator 컴포넌트에 {parameterName} 이름을 가진 파라미터가 없습니다.");

				throw new ArgumentException();
			}

			var paramType = animatorParameterTypes[hashKey];

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (paramType)
			{
				case AnimatorControllerParameterType.Float:
					animator.SetFloat(hashKey, (float)(object)setValue);
					break;
				case AnimatorControllerParameterType.Int:
					animator.SetInteger(hashKey, (int)(object)setValue);
					break;
				case AnimatorControllerParameterType.Bool:
					animator.SetBool(hashKey, (bool)(object)setValue);
					break;
				case AnimatorControllerParameterType.Trigger:
					animator.SetTrigger(hashKey);
					break;
			}
		}

		public object GetAnimatorState(string parameterName)
		{
			var hashKey = Animator.StringToHash(parameterName);

			if (!IsParameterNameValid(hashKey))
			{
				Logger.Log(LogPriority.Exception, $"{gameObject.name}의 Animator 컴포넌트에 {parameterName} 이름을 가진 파라미터가 없습니다.");

				throw new ArgumentException();
			}

			var paramType = animatorParameterTypes[hashKey];

			switch (paramType)
			{
				case AnimatorControllerParameterType.Float:
					return animator.GetFloat(hashKey);
				case AnimatorControllerParameterType.Int:
					return animator.GetInteger(hashKey);
				case AnimatorControllerParameterType.Bool:
					return animator.GetBool(hashKey);
			}

			return null;
		}

		private void Awake()
		{
			animator = GetComponent(typeof(Animator)) as Animator;

			foreach (var param in animator.parameters)
			{
				var key = param.nameHash;
				var value = param.type;

				if (IsParameterNameValid(key))
				{
					Logger.Log(LogPriority.Warning, $"{gameObject.name}의 Animator 컴포넌트에 {param.name} 이름을 가진 파라미터가 2개 이상 있습니다.");

					continue;
				}

				animatorParameterTypes.Add(key, value);
			}
		}

		private bool IsParameterNameValid(int key) => animatorParameterTypes.ContainsKey(key);
	}
}