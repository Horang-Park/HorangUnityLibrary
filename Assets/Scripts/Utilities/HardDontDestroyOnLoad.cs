using UnityEngine;

namespace Utilities
{
	public class HardDontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void Start(){}
	}
}
