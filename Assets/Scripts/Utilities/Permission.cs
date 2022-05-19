using System.Collections;
using UnityEngine;

namespace Utilities
{
	public static class Permission
	{
		public static bool HasCameraPermission()
		{
#if UNITY_ANDROID
			return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
			return Application.HasUserAuthorization(UserAuthorization.WebCam);
#else
			return Application.HasUserAuthorization(UserAuthorization.WebCam);
#endif
		}

		public static bool HasMicrophonePermission()
		{
#if UNITY_ANDROID
			return Permission.HasUserAuthorizedPermission(Permission.Microphone);
#elif UNITY_IPHONE
			return Application.HasUserAuthorization(UserAuthorization.Microphone);
#else
			return Application.HasUserAuthorization(UserAuthorization.Microphone);
#endif
		}

		public static IEnumerator RequestCameraPermission()
		{
#if UNITY_ANDROID
			Permission.RequestUserPermission(Permission.Camera);
			yield return null;
#elif UNITY_IOS
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
#else
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
#endif
		}


		public static IEnumerator RequestMicrophonePermission()
		{
#if UNITY_ANDROID
			Permission.RequestUserPermission(Permission.Microphone);
			yield return null;
#elif UNITY_IPHONE
			yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
#else
			yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
#endif
		}
	}
}

// Code Author: ChangsooPark