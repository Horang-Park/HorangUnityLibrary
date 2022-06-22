using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Editor
{
	public class DebugMode : MonoBehaviour
	{
		private static readonly string[] Symbols =
		{
			"DEBUG_MODE_LOG",
		};

		private const char DefineSeparator = ';';

		[MenuItem("Debug Mode/Log/Enable Log")]
		private static void EnableLog()
		{
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
			var allDefines = currentDefines.Split(DefineSeparator).ToList();

			allDefines.AddRange(Symbols.Except(allDefines));

			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(DefineSeparator, allDefines.ToArray()));

			EditorUtility.DisplayDialog("Debug Mode", "로그가 활성화 되었어요.\n유니티의 스크립트 처리가 끝난 후 실행해주세요.", "확인");
		}

		[MenuItem("Debug Mode/Log/Enable Log", true)]
		private static bool ValidateEnableLog()
		{
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
			var allDefines = currentDefines.Split(DefineSeparator).ToList();

			return allDefines.All(define => !define.Equals(Symbols[0]));
		}

		[MenuItem("Debug Mode/Log/Disable Log")]
		private static void DisableLog()
		{
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
			var allDefines = currentDefines.Split(DefineSeparator).ToList();

			foreach (var item in Symbols)
			{
				if (allDefines.Remove(item) is false)
				{
					break;
				}
			}

			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(DefineSeparator, allDefines.ToArray()));
			
			EditorUtility.DisplayDialog("Debug Mode", "로그가 비활성화 되었어요.\n유니티의 스크립트 처리가 끝난 후 실행해주세요.", "확인");
		}

		[MenuItem("Debug Mode/Log/Disable Log", true)]
		private static bool ValidateDisableLog()
		{
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
			var allDefines = currentDefines.Split(DefineSeparator).ToList();

			return allDefines.Any(define => define.Equals(Symbols[0]));
		}
	}
}