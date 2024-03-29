﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Utilities.Debug
{
	public enum LogPriority
	{
		Debug,
		Verbose,
		Warning,
		Error,
		Exception,
		Information,
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
			"<color=#a6a6a6>",
		};
		private const string LoggerPriorityColorPostfix = "</color>";

		private const string FontSizePrefix = "<size=13>";
		private const string FontSizePostfix = "</size>";
		private const string FontBoldPrefix = "<b>";
		private const string FontBoldPostfix = "</b>";

		private static readonly char[] PathSeparator = { '\\', '/' };
		
		private const string Separator = " ▶ ";
		private const char LineNumberSeparator = ':';

		private const char OpenBracket = '[';
		private const string CloseBracket = "] ";

		private static bool _isInternalLogCall;

		/// <summary>
		/// Log 띄우기
		/// <br></br>
		/// StackTrace를 이용하므로 Update, FixedUpdate 혹은 지속적으로 반복되는 Coroutine에서는 성능 저하가 발생할 수 있다.
		/// <br></br>
		/// 테스트 혹은 디버깅 용도로 사용하고 위와 같은 메서드에서는 제거해주어야 한다.
		/// </summary>
		/// <param name="priority">중요도</param>
		/// <param name="message">메세지</param>
		public static void Log(LogPriority priority, string message)
		{
#if DEBUG_MODE_LOG
			var st = new StackTrace(true);
			var sf = st.GetFrame(_isInternalLogCall ? 2 : 1);
			var fn = Path.GetFileNameWithoutExtension(sf.GetFileName()?.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries)[^1]);
			var sb = new StringBuilder(FontBoldPrefix);
			
			sb.Append(FontSizePrefix);
			sb.Append(LoggerPriorityColorPrefix[(int)priority]);
			sb.Append(OpenBracket);
			sb.Append(fn);
			sb.Append(LineNumberSeparator);
			sb.Append(sf.GetFileLineNumber());
			sb.Append(CloseBracket);
			sb.Append(sf.GetMethod().Name);
			sb.Append(Separator);
			sb.Append(message);
			sb.Append(LoggerPriorityColorPostfix);
			sb.Append(FontSizePostfix);
			sb.Append(FontBoldPostfix);

			switch (priority)
			{
				case LogPriority.Debug:
				case LogPriority.Verbose:
				case LogPriority.Information:
					UnityEngine.Debug.Log(sb.ToString());
					break;
				case LogPriority.Warning:
					UnityEngine.Debug.LogWarning(sb.ToString());
					break;
				case LogPriority.Error:
				case LogPriority.Exception:
					UnityEngine.Debug.LogError(sb.ToString());
					break;
			}
			
			sb.Clear();

			_isInternalLogCall = false;
#endif
		}

		/// <summary>
		/// Log 띄우기 [string을 바로 로그로 띄울 수 있는 기능]
		/// <br></br>
		/// StackTrace를 이용하므로 Update, FixedUpdate 혹은 지속적으로 반복되는 Coroutine에서는 성능 저하가 발생할 수 있다.
		/// <br></br>
		/// 테스트 혹은 디버깅 용도로 사용하고 위와 같은 메서드에서는 제거해주어야 한다.
		/// /// </summary>
		/// <param name="message"></param>
		/// <param name="logPriority"></param>
		public static string ToLog(this string message, LogPriority logPriority)
		{
#if DEBUG_MODE_LOG
			_isInternalLogCall = true;
			
			Log(logPriority, message);
#endif
			return message;

		}
	}
}

// Code Author: ChangsooPark