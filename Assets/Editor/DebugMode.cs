using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Utilities;
using Utilities.Debug;

namespace Editor
{
	public class DebugMode : MonoBehaviour
	{
		private const string LogSymbol = "DEBUG_MODE_LOG";
		private const string PanelSymbol = "DEBUG_MODE_PANEL";
		private const char DefineSeparator = ';';

		#region Log

		[MenuItem("Debug Mode/Log/Enable")]
		private static void EnableLog()
		{
			EnableSymbol(LogSymbol, "로그가 활성화 되었어요.\n스크립트 처리 후 플레이 해주세요.");
		}

		[MenuItem("Debug Mode/Log/Enable", true)]
		private static bool ValidateEnableLog()
		{
			return IsAlreadyDefined(LogSymbol) == false;
		}

		[MenuItem("Debug Mode/Log/Disable")]
		private static void DisableLog()
		{
			DisableSymbol(LogSymbol, "로그가 비활성화 되었어요.\n스크립트 처리 후 플레이 해주세요.");
		}

		[MenuItem("Debug Mode/Log/Disable", true)]
		private static bool ValidateDisableLog()
		{
			return IsAlreadyDefined(LogSymbol);
		}
		
		#endregion

		#region Debug Panel

		[MenuItem("Debug Mode/Panel/Enable")]
		private static void EnablePanel()
		{
			var panelGameObject = GameObject.Find("Debug Panel");
			
			if (panelGameObject is null)
			{
				panelGameObject = new GameObject("Debug Panel");
				panelGameObject.AddComponent<Panel>();
				panelGameObject.AddComponent<HardDontDestroyOnLoad>();
				panelGameObject.gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable | HideFlags.HideInHierarchy;
			}
			
			EnableSymbol(PanelSymbol, "패널이 활성화 되었어요.");
		}

		[MenuItem("Debug Mode/Panel/Enable", true)]
		private static bool ValidateEnablePanel()
		{
			return IsAlreadyDefined(PanelSymbol) == false;
		}

		[MenuItem("Debug Mode/Panel/Disable")]
		private static void DisablePanel()
		{
			DisableSymbol(PanelSymbol, "패널이 비활성화 되었어요.");
		}

		[MenuItem("Debug Mode/Panel/Disable", true)]
		private static bool ValidateDisablePanel()
		{
			return IsAlreadyDefined(PanelSymbol) == true;
		}

		#endregion

		#region Internal use only

		private static List<string> GetEverySymbols()
		{
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
			
			return currentDefines.Split(DefineSeparator).ToList();
		}

		private static void EnableSymbol(string symbol, string informationText)
		{
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var allDefines = GetEverySymbols();

			allDefines.Add(symbol);

			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(DefineSeparator, allDefines.ToArray()));

			EditorUtility.DisplayDialog("Debug Mode", $"{informationText}", "확인");
		}

		private static void DisableSymbol(string symbol, string informationText)
		{
			var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var allDefines = GetEverySymbols();

			allDefines.Remove(symbol);

			PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(DefineSeparator, allDefines.ToArray()));
			
			EditorUtility.DisplayDialog("Debug Mode", $"{informationText}", "확인");
		}

		private static bool IsAlreadyDefined(string symbol)
		{
			var allDefines = GetEverySymbols();

			return allDefines.Contains(symbol);
		}
		
		#endregion
	}
}