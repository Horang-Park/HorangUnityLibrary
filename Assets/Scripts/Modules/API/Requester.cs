using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.API
{
	public static class Requester
	{
		public static async UniTask<string> Send(UnityWebRequest unityWebRequest, Action<string> onFailure = null)
		{
			var sb = new StringBuilder();
			sb.Append("API Method: ");
			sb.Append(unityWebRequest.method);
			sb.Append(" / ");
			sb.Append("URL: ");
			sb.Append(unityWebRequest.url);

			Logger.Log(LogPriority.Information, $"API Request Send Info: {sb}");
			
			var operation = await unityWebRequest.SendWebRequest();

			if (operation.result is UnityWebRequest.Result.Success)
			{
				return operation.downloadHandler.text;
			}

			sb.Clear();

			var code = operation.responseCode;
			var message = operation.error;
			
			sb.Append("Response Code: ");
			sb.Append(code);
			sb.Append(" / Message: ");
			sb.Append(message);
				
			onFailure?.Invoke(sb.ToString());
			
			sb.Clear();

			return string.Empty;
		}
	}
}

// Code Author: ChangsooPark