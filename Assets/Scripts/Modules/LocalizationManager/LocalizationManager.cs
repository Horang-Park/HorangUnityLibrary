using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Structural;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

namespace Modules.LocalizationManager
{
	public class LocalizationManager : MonoSingleton<LocalizationManager>
	{
		private readonly Dictionary<SystemLanguage, Dictionary<int, string>> textTable = new();

		private SystemLanguage languageLock;
		/// <summary>
		/// 이 값으로 해당 언어 테이블의 값을 계속 불러옴 (설정 필수)
		/// </summary>
		public SystemLanguage LanguageLock
		{
			set => languageLock = value;
		}

		/// <summary>
		/// 언어 파일 로드 (이미 등록되어 있는 언어면 기존 언어 삭제 후, 새로 등록)
		/// </summary>
		/// <param name="language">불러올 언어 파일이 제공하는 언어</param>
		/// <param name="path">불러올 언어 파일 경로</param>
		/// <param name="isFileInResourcesFolder">파일이 Resources 폴더에 있는지 여부 체크용</param>
		/// <exception cref="FileLoadException">파일을 불러오지 못했을 때</exception>
		/// <exception cref="FormatException">언어 파일의 포멧이 잘못되어 있을 때</exception>
		public async UniTaskVoid LoadLocalization(SystemLanguage language, string path, bool isFileInResourcesFolder = false)
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

		/// <summary>
		/// 언어 값 갖고옴
		/// </summary>
		/// <param name="key">갖고올 언어 값 키</param>
		/// <returns>languageLock에 해당하는 언어 값 반환</returns>
		/// <exception cref="InvalidDataException">언어 값을 찾을 수 없을 때</exception>
		public string Get(string key)
		{
			var hashKey = key.GetHashCode();

			if (textTable.ContainsKey(languageLock))
			{
				var table = textTable[languageLock];

				if (table.ContainsKey(hashKey))
				{
					return table[hashKey];
				}
				
				Logger.Log(LogPriority.Exception, $"{languageLock} 언어 테이블에 {key} 값이 없습니다.");

				throw new InvalidDataException("언어 파일의 키 값 혹은 코드 상의 키 값을 확인해주세요.");
			}
			
			Logger.Log(LogPriority.Exception, $"{languageLock} 언어에 해당하는 현지화 테이블이 없습니다.");

			throw new InvalidDataException("LanguageLock에 적절한 언어를 세팅해주세요.");

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