using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

namespace Utilities
{
	public static class LoadImage
	{
		/// <summary>
		/// 특정 경로에서 이미지 로드 (비용 큼)
		/// </summary>
		/// <param name="path">로드 할 경로</param>
		/// <returns>로드한 이미지 파일을 스프라이트로 반환</returns>
		/// <exception cref="FileNotFoundException">파일을 찾지 못 함</exception>
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

		/// <summary>
		/// Resources 폴더에서 이미지 로드
		/// </summary>
		/// <param name="path">로드 경로 (상대 경로, 확장자 제외)</param>
		/// <returns>로드한 스프라이트</returns>
		/// <exception cref="FileLoadException">이미지가 없거나, 로드하려는 파일이 스프라이가 아닐 경우</exception>
		public static async UniTask<Sprite> LoadFromResources(string path)
		{
			var loadObject = await Resources.LoadAsync<Sprite>(path);

			if (loadObject is not null)
			{
				return loadObject as Sprite;
			}
			
			Logger.Log(LogPriority.Exception, $"{path} 경로에 해당하는 파일이 존재하지 않거나 해당 파일이 스프라이트가 아닙니다.");

			throw new FileLoadException();

		}

		/// <summary>
		/// Resources 폴더에서 여러 이미지 로드
		/// </summary>
		/// <param name="paths">이미지를 로드할 경로들 (상대 경로, 확장자 제외)</param>
		/// <returns>로드한 스프라이트 묶음</returns>
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