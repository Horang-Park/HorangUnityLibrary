using System;
using System.Diagnostics;
using System.Text;

namespace Utilities
{
	public enum LoggerPriority
	{
		Debug,
		Verbose,
		Warning,
		Error,
		Exception,
	}
	
	public static class Logger
	{
		private static string[] LoggerPriorityColorPrefix =
		{
			"<color=#2d75eb>",
			"<color=#ebebeb>",
			"<color=#fc960f>",
			"<color=#fc460f>",
			"<color=#f725a3>",
		};
		private const string LoggerPriorityColorPostfix = "</color>";

		private const string FontSizePrefix = "<size=16>";
		private const string FontSizePostfix = "</size>";

		private const string FontBoldPrefix = "<b>";
		private const string FontBoldPostfix = "</b>";

		private const string Separator = " ▶ ";

		/// <summary>
		/// Log 띄우기
		/// <br></br>
		/// StackTrace를 이용하므로 Update, FixedUpdate 혹은 지속적으로 반복되는 Coroutine에서는 성능 저하가 발생할 수 있다.
		/// <br></br>
		/// 테스트 혹은 디버깅 용도로 사용하고 위와 같은 메서드에서는 제거해주어야 한다.
		/// </summary>
		/// <param name="priority">중요도</param>
		/// <param name="message">메세지</param>
		public static void Log(LoggerPriority priority, string message)
		{
			var st = new StackTrace();
			var sf = st.GetFrame(1);
			var mn = sf.GetMethod().Name;
			var fc = LoggerPriorityColorPrefix[(int)priority];
			
			var sb = new StringBuilder(FontBoldPrefix);
			sb.Append(FontSizePrefix);
			sb.Append(fc);
			sb.Append(mn);
			sb.Append(Separator);
			sb.Append(message);
			sb.Append(LoggerPriorityColorPostfix);
			sb.Append(FontSizePostfix);
			sb.Append(FontBoldPostfix);

			switch (priority)
			{
				case LoggerPriority.Debug:
					UnityEngine.Debug.Log(sb.ToString());
					sb.Clear();
					break;
				case LoggerPriority.Verbose:
					UnityEngine.Debug.Log(sb.ToString());
					sb.Clear();
					break;
				case LoggerPriority.Warning:
					UnityEngine.Debug.LogWarning(sb.ToString());
					sb.Clear();
					break;
				case LoggerPriority.Error:
					UnityEngine.Debug.LogError(sb.ToString());
					sb.Clear();
					break;
				case LoggerPriority.Exception:
					UnityEngine.Debug.LogError(sb.ToString());
					sb.Clear();
					break;
			}
		}
	}
}