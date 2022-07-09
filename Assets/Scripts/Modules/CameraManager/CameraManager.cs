using System;
using System.Collections.Generic;
using Structural;
using UnityEngine;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

namespace Modules.CameraManager
{
	public class CameraManager : MonoSingleton<CameraManager>
	{
		private readonly Dictionary<int, CameraElement> cameraPropertiesMap = new();

		/// <summary>
		/// 카메라 등록. 이름이 동일하면 기존에 있는 카메라를 삭제하고 새로 등록한다.
		/// </summary>
		/// <param name="cameraName">카메라 이름</param>
		/// <param name="cameraElement">등록할 카메라 요소</param>
		public void AddCamera(string cameraName, CameraElement cameraElement)
		{
			var hashKey = cameraName.GetHashCode();

			if (IsExistCamera(hashKey))
			{
				Logger.Log(LogPriority.Warning, $"{cameraName} 카메라는 이미 등록되어 있습니다. 삭제하고 새로 등록합니다.");
				
				cameraPropertiesMap.Remove(hashKey);
			}
			
			cameraElement.Initialize();
			cameraElement.SetCameraName(cameraName.GetHashCode().ToString());
			
			cameraPropertiesMap.Add(hashKey, cameraElement);
		}

		/// <summary>
		/// 카메라 삭제
		/// </summary>
		/// <param name="cameraName">삭제할 카메라 이름</param>
		/// <param name="isDestroy">카메라 GameObject 까지 파괴할 것인지 선택할 플래그</param>
		public void RemoveCamera(string cameraName, bool isDestroy = false)
		{
			var hashKey = cameraName.GetHashCode();

			if (TryGetCamera(hashKey, out var cameraElement))
			{
				if (isDestroy)
				{
					Destroy(cameraElement.Camera.gameObject);
				}

				cameraPropertiesMap.Remove(hashKey);
			}
			else
			{
				Logger.Log(LogPriority.Error, $"{cameraName}으로 등록된 카메라가 없습니다.");
			}
		}

		/// <summary>
		/// 등록되어 있는 카메라 전체 삭제
		/// </summary>
		public void RemoveAllCamera()
		{
			cameraPropertiesMap.Clear();
		}

		/// <summary>
		/// 카메라 갖고오기
		/// </summary>
		/// <param name="cameraName">갖고올 카메라 이름</param>
		/// <returns>카메라</returns>
		/// <exception cref="ArgumentException">등록되어 있지 않은 카메라 이름 입력 시</exception>
		public Camera GetCamera(string cameraName)
		{
			var hashKey = cameraName.GetHashCode();

			if (TryGetCamera(hashKey, out var cameraElement))
			{
				return cameraElement.Camera;
			}
			
			Logger.Log(LogPriority.Exception, $"{cameraName} 이름을 가진 카메라는 등록되지 않았습니다.");

			throw new ArgumentException();
		}

		private bool IsExistCamera(int key)
		{
			return cameraPropertiesMap.ContainsKey(key);
		}

		private bool TryGetCamera(int key, out CameraElement cameraElement)
		{
			if (IsExistCamera(key))
			{
				cameraElement = cameraPropertiesMap[key];

				return true;
			}

			cameraElement = null;

			return false;
		}
	}
}