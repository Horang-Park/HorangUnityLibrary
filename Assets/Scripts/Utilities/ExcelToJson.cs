using System.IO;
using Aspose.Cells;

namespace Utilities
{
	public static class ExcelToJson
	{
		public static void XlsxToJson(string sourcePath, string jsonSaveDestinationPath)
		{
			if (File.Exists(sourcePath) is false)
			{
				Logger.Log(LogPriority.Error, $"{sourcePath}에 해당하는 파일이 존재하지 않습니다.");
				
				return;
			}

			var workbook = new Workbook(sourcePath);

			workbook.Save(jsonSaveDestinationPath, SaveFormat.Json);
			
			workbook.Dispose();
		}
	}
}