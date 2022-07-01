using System;
using System.Text;
using UnityEngine;
using Utilities;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

namespace Structural
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;
		private static readonly object Lock = new object();

		public static T Instance
		{
			get
			{
				lock (Lock)
				{
					if (instance != null)
					{
						return instance;
					}
					
					instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						Logger.Log(LogPriority.Error, $"{typeof(T)}의 Instance가 중복되었습니다. 정상동작을 보장하지 못하므로, Scene을 다시 열어주세요.");
						
						return instance;
					}

					if (instance != null)
					{
						return instance;
					}
					
					var singleton = new GameObject();
					instance = singleton.AddComponent<T>();
					singleton.name = new StringBuilder("[Singleton] ").Append(typeof(T)).ToString();

					

					return instance;
				}
			}
		}

		protected virtual void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}

// Code Author: ChangsooPark