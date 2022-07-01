using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Utilities.Debug;

namespace Utilities
{
	public static class Compression
	{
		/// <summary>
		/// 압축하기
		/// </summary>
		/// <param name="sourceDirectoryPath">압축할 폴더 경로</param>
		/// <param name="destinationPath">압축파일 내보낼 경로</param>
		/// <returns>성공: true, 실패: false 및 예외 로그로 표시</returns>
		public static bool Zip(string sourceDirectoryPath, string destinationPath)
		{
			try
			{
				if (File.Exists(destinationPath))
				{
					File.Delete(destinationPath);
				}
				
				ZipFile.CreateFromDirectory(sourceDirectoryPath, destinationPath);
				
				Logger.Log(LogPriority.Information, $"{destinationPath} 경로에 {sourceDirectoryPath} 의 Zip 파일을 압축했습니다.");

				return true;
			}
			catch (Exception e)
			{
				Logger.Log(LogPriority.Exception, $"압축하기에 실패했습니다. Message: {e.Message} / Source: {e.Source}");

				return false;
			}
		}
		
		/// <summary>
		/// 압축풀기
		/// </summary>
		/// <param name="sourceDirectoryPath">압축 풀 파일 경로</param>
		/// <param name="destinationPath">압축 푼 파일을 내보낼 경로</param>
		/// <returns>성공: true, 실패: false 및 예외 로그로 표시</returns>
		public static bool Unzip(string sourceDirectoryPath, string destinationPath)
		{
			try
			{
				if (File.Exists(destinationPath))
				{
					File.Delete(destinationPath);
				}
				
				ZipFile.ExtractToDirectory(sourceDirectoryPath, destinationPath);
				
				Logger.Log(LogPriority.Information, $"{destinationPath} 경로에 {sourceDirectoryPath} 의 압축을 풀었습니다.");

				return true;
			}
			catch (Exception e)
			{
				Logger.Log(LogPriority.Exception, $"압축풀기에 실패했습니다. Message: {e.Message}");

				return false;
			}
		}

		/// <summary>
		/// Zip 파일 내부의 파일 목록을 가져옴
		/// </summary>
		/// <param name="sourceDirectoryPath">Zip파일 경로</param>
		/// <returns>파일 목록</returns>
		public static ReadOnlyCollection<string> GetFileListInZip(string sourceDirectoryPath)
		{
			if (File.Exists(sourceDirectoryPath) is false)
			{
				Logger.Log(LogPriority.Error, $"{sourceDirectoryPath} 파일은 정상적인 파일이 아닙니다.");
				
				return null;
			}

			var archive = ZipFile.Open(sourceDirectoryPath, ZipArchiveMode.Read);
			var fileNames = archive.Entries.Select(entry => entry.Name).ToList();

			return new ReadOnlyCollection<string>(fileNames);
		}
	}
}