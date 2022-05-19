using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Modules.API
{
	public class Requester
	{
		private async UniTask<string> Send(UnityWebRequest unityWebRequest, Action<string> onFailure = null)
		{
			var operation = await unityWebRequest.SendWebRequest();

			if (operation.result is UnityWebRequest.Result.Success)
			{
				return operation.downloadHandler.text;
			}

			var code = operation.responseCode;
			var message = operation.error;
			var sb = new StringBuilder("Response Code: ");
				
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