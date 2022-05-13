using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Modules.API
{
	public class Requester
	{
		/// <summary>
		/// 리퀘스트 보내기
		/// </summary>
		/// <param name="unityWebRequest">리퀘스트 요청하기 위해 세팅해놓은 UnityWebRequester </param>
		/// <param name="onSuccess">성공했을 때 콜백</param>
		/// <param name="onFailure">실패했을 때 콜백(메시지 전달)</param>
		/// <returns>awaitable 메서드로써, 사용하는 async 메서드 안에서 await 으로 대기한 후, 데이터를 string 반환</returns>
		public async UniTask<string> Send(UnityWebRequest unityWebRequest, Action onSuccess, Action<string> onFailure = null)
		{
			var operation = await unityWebRequest.SendWebRequest();

			if (operation.result is UnityWebRequest.Result.Success)
			{
				onSuccess?.Invoke();
				
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