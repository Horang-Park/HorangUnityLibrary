using UnityEngine;

namespace Modules.CameraManager
{
	[RequireComponent(typeof(Camera))]
	public class CameraElement : MonoBehaviour
	{
		public Camera Camera { get; private set; }

		public void Initialize()
		{
			Camera = GetComponent(typeof(Camera)) as Camera;
		}

		public void SetCameraName(string cameraName)
		{
			Camera.name = cameraName;
		}
	}
}