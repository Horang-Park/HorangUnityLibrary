using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utilities
{
	public static class LoadImage
	{
		public static async UniTask<Sprite> LoadFromSpecificPath(string path)
		{
			var fileInfo = new FileInfo(path);

			if (fileInfo.Exists is false)
			{
				Logger.Log(LogPriority.Exception, $"{path} 경로에 해당하는 파일이 존재하지 않습니다.");

				throw new FileNotFoundException();
			}
		
			var fileStream = new FileStream(path, FileMode.Open);
			var fileReadBuffer = new byte[fileStream.Length];

			await UniTask.Create(() =>
			{
				// ReSharper disable once MustUseReturnValue
				fileStream.Read(fileReadBuffer, 0, fileReadBuffer.Length);
				
				return default;
			});
			
			fileStream.Close();
		
			return GetSpriteFromByteBuffer(fileReadBuffer);
		}

		public static async UniTask<Sprite> LoadFromResources(string path)
		{
			var loadObject = await Resources.LoadAsync<Sprite>(path);

			switch (loadObject)
			{
				case null:
					Logger.Log(LogPriority.Exception, $"{path} 경로에 해당하는 파일이 존재하지 않습니다.");

					throw new FileNotFoundException();
				case Sprite sprite:
					return sprite;
				default:
					Logger.Log(LogPriority.Exception, $"{path} 파일은 스프라이트 파일이 아닙니다.");

					throw new FileLoadException();
			}
		}

		public static async UniTask<List<Sprite>> LoadManyFromResources(IEnumerable<string> paths)
		{
			var tasks = Enumerable.Select(paths, LoadFromResources);
			var loadedSprites = await UniTask.WhenAll(tasks);

			return loadedSprites.ToList();
		}
		
		private static Sprite GetSpriteFromByteBuffer(byte[] buffer)
		{
			var texture = new Texture2D(2, 2);
			texture.LoadImage(buffer);

			var rect = new Rect(0, 0, texture.width, texture.height);

			return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
		}
	}
}