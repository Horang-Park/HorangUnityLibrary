using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities;
using Utilities.Debug;

namespace Modules.Parser
{
	public static class JsonParser<T>
	{
		#region External interfaces
		
		/// <summary>
		/// 일반 Json object parsing
		/// </summary>
		/// <param name="json">파싱할 Json string</param>
		/// <typeparam name="T">파싱할 전체 템플릿</typeparam>
		/// <returns>템플릿 형 반환</returns>
		public static T JsonParsing(string json)
		{
			Logger.Log(LogPriority.Information, $"Original Json: {json}");
			
			try
			{
				return JsonConvert.DeserializeObject<T>(json, JsonParserInternalUseOnly.JsonSettings);
			}
			catch (JsonException exception)
			{
				Logger.Log(LogPriority.Exception, exception.ToString());

				throw;
			}
		}

		/// <summary>
		/// 배열형 Json object parsing
		/// </summary>
		/// <param name="json">파싱할 Json string</param>
		/// <typeparam name="T">배열의 요소로 파싱 될 템플릿</typeparam>
		/// <returns>템플릿 형 반환</returns>
		/// <exception cref="NullReferenceException">파라미터로 넘겨준 Json을 JArray로 파싱에 실패했을 때</exception>
		public static List<T> JsonArrayParsing(string json)
		{
			Logger.Log(LogPriority.Information, $"Original Json: {json}");
			
			JArray arrayData;

			try
			{
				arrayData = JsonConvert.DeserializeObject(json, JsonParserInternalUseOnly.JsonSettings) as JArray;
			}
			catch (JsonException exception)
			{
				Logger.Log(LogPriority.Exception, exception.ToString());

				throw;
			}

			return (arrayData ?? throw new NullReferenceException()).Select(jToken => ((JObject)jToken).ToString()).Select(InternalArrayElementParsing).ToList();
		}
		
		#endregion

		#region Module internal use only
		
		private static T InternalArrayElementParsing(string element)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(element, JsonParserInternalUseOnly.JsonSettings);
			}
			catch (JsonException exception)
			{
				Logger.Log(LogPriority.Exception, exception.ToString());
			}

			return default;
		}
		
		#endregion
	}
	
	public static class JsonParserInternalUseOnly
	{
		public static readonly JsonSerializerSettings JsonSettings = new()
		{
			NullValueHandling = NullValueHandling.Include,
			MissingMemberHandling = MissingMemberHandling.Error
		};
	}
}

// Code Author: ChangsooPark