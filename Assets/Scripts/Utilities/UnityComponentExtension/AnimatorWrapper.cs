using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Utilities.UnityComponentExtension
{
	[RequireComponent(typeof(Animator))]
	public class AnimatorWrapper : MonoBehaviour
	{
		private Animator animator;

		private readonly Dictionary<int, AnimatorControllerParameterType> animatorParameterTypes = new();
		
		public void StartAnimation<T>(string parameterName, T setValue = default)
		{
			if (typeof(T).IsPrimitive is false)
			{
				Logger.Log(LogPriority.Exception, $"T는 float, int, bool 형식만 지원합니다.");

				throw new ArgumentException();
			}

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
				default:
					throw new ArgumentOutOfRangeException();
			}

			HashCode.Combine(0);
		}

		private void Awake()
		{
			animator = GetComponent(typeof(Animator)) as Animator;
		}

		private void Start()
		{
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