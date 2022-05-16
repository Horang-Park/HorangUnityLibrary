﻿using System.Text;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Structural
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;

		private static readonly object Lock = new object();
		private static bool applicationIsQuitting = false;

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					return null;
				}

				lock (Lock)
				{
					if (instance != null)
					{
						return instance;
					}
					
					instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						Logger.Log(LoggerPriority.Error, $"{typeof(T)}의 Instance가 중복되었습니다. 정상동작을 보장하지 못하므로, Scene을 다시 열어주세요.");
						
						return instance;
					}

					if (instance != null)
					{
						return instance;
					}
					
					var singleton = new GameObject();
					instance = singleton.AddComponent<T>();
					singleton.name = new StringBuilder("[Singleton] ").Append(typeof(T)).ToString();

					DontDestroyOnLoad(singleton);

					return instance;
				}
			}
		}
		
		public void OnDestroy()
		{
			applicationIsQuitting = true;
		}
	}
}