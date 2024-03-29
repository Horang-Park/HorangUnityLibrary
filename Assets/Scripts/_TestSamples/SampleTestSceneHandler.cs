using System;
using System.Collections;
using System.Collections.Generic;
using Modules.CameraManager;
using Modules.SceneManager;
using UnityEngine;

public class SampleTestSceneHandler : MonoBehaviour
{
	public CameraElement cameraElement;

	private void Awake()
	{
		CameraManager.Instance.AddCamera("Main Camera", cameraElement);
		SceneManager.Instance.SetCurrentScene();
	}
}