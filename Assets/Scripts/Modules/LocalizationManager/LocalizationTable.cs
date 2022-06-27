using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.LocalizationManager
{
	public static class LocalizationTable
	{
		private static readonly Dictionary<SystemLanguage, Dictionary<int, string>> textTable = new();

		private static SystemLanguage keepUseTableIndex;
		public static SystemLanguage KeepUseTableIndex
		{
			set => keepUseTableIndex = value;
		}

		/// <summary>
		/// 언어 파일 로드 (이미 등록되어 있는 언어면 기존 언어 삭제 후, 새로 등록)
		/// </summary>
		/// <param name="language">불러올 언어 파일이 제공하는 언어</param>
		/// <param name="path">불러올 언어 파일 경로</param>
		/// <param name="isFileInResourcesFolder">파일이 Resources 폴더에 있는지 여부 체크용</param>
		/// <exception cref="FileLoadException">파일을 불러오지 못했을 때</exception>
		/// <exception cref="FormatException">언어 파일의 포멧이 잘못되어 있을 때</exception>
		public static async UniTaskVoid LoadLocalization(SystemLanguage language, string path, bool isFileInResourcesFolder = false)
		{
			var table = new Dictionary<int, string>();
			var readLines = await LoadLocalizationFile(path, isFileInResourcesFolder);
			var separateLines = readLines.Split('\n');

			var debugIndex = 0;

			foreach (var line in separateLines)
			{
				var processLine = line.Trim();
				var keyValue = processLine.Split("=");

				if (keyValue.Length != 2)
				{
					Logger.Log(LogPriority.Exception, $"{path} 파일의 행 읽기 중, 정상적이지 않은 라인을 만났습니다. 해당 파일 수정 후 재실행해주세요. / Line Position: {debugIndex}");

					throw new FileLoadException();
				}

				keyValue[0] = keyValue[0].Trim();
				keyValue[1] = keyValue[1].Trim();

				if (string.IsNullOrEmpty(keyValue[0]))
				{
					Logger.Log(LogPriority.Exception, $"{path} 파일의 키가 비어있습니다. 해당 파일 수정 후 재실행해주세요. / Line Position: {debugIndex}");

					throw new FormatException();
				}
				
				if (string.IsNullOrEmpty(keyValue[1]))
				{
					Logger.Log(LogPriority.Exception, $"{path} 파일의 값이 비어있습니다. 해당 파일 수정 후 재실행해주세요. / Line Position: {debugIndex}");
					
					throw new FormatException();
				}

				var key = keyValue[0].GetHashCode();
				var value = keyValue[1];

				if (table.ContainsKey(key))
				{
					Logger.Log(LogPriority.Warning, $"키가 {keyValue[0]} 인 값이 존재합니다.");
					
					debugIndex++;

					continue;
				}
				
				table.Add(key, value);
				
				debugIndex++;
			}

			if (textTable.ContainsKey(language))
			{
				Logger.Log(LogPriority.Warning, $"{language} 언어는 이미 존재합니다.");

				textTable.Remove(language);
			}
			
			textTable.Add(language, table);
		}

		private static async UniTask<string> LoadLocalizationFile(string path, bool isFileInResourcesFolder = false)
		{
			if (isFileInResourcesFolder)
			{
				var textAssetObject = await Resources.LoadAsync<TextAsset>(path);

				switch (textAssetObject)
				{
					case null:
						Logger.Log(LogPriority.Exception, $"{path} 경로에 파일이 없습니다.");

						throw new FileNotFoundException();
					case TextAsset textAsset:
						return textAsset.text;
					default:
						Logger.Log(LogPriority.Exception, $"{path} 에서 로드한 파일을 TextAsset으로 변환할 수 없습니다.");

						throw new InvalidCastException();
				}
			}
			
			var fileInfo = new FileInfo(path);

			if (fileInfo.Exists is false)
			{
				Logger.Log(LogPriority.Exception, $"{path} 경로에 파일이 없습니다.");

				throw new FileNotFoundException();
			}
			
			var reader = new StreamReader(path);
			var value = await reader.ReadToEndAsync();
			
			reader.Close();
			reader.Dispose();

			return value; 
		}
	}
}