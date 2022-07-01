using System.Collections.Generic;
using JetBrains.Annotations;
using Structural;
using UniRx;
using UnityEngine;
using Utilities;
using Utilities.Debug;
using Logger = Utilities.Debug.Logger;

namespace Modules.UIManager
{
	public class UIManager : MonoBehaviour
	{
		[System.Serializable]
		public struct UIDataType
		{
			public string uiName;
			public bool useFirstUI;
			public UIBase uiBase;
		}
		
		[Header("Inspector")]
		[SerializeField] private List<UIDataType> uiBases;

		private readonly Dictionary<int, UIBase> uiBaseDictionary = new();
		private readonly Stack<(int, UIBase)> uiBaseUseHistory = new();
		private bool firstUIActiveFlag;

		public readonly BoolReactiveProperty isDataProcessingComplete = new();

		/// <summary>
		/// BaseUI를 상속받은 UI Object 반환
		/// </summary>
		/// <param name="uiName">가져올 UI 이름 (인스펙터에 등록된 이름)</param>
		/// <typeparam name="T">캐스팅 할 UI 타입</typeparam>
		/// <returns>찾은 UI 반환, 찾지 못하면 null 반환</returns>
		[CanBeNull]
		public UIBase GetUIBase<T>(string uiName) where T : UIBase
		{
			var key = uiName.GetHashCode();

			if (!IsKeyValid(uiName, out var ui))
			{
				return null;
			}

			if (ui.IsVisible is false)
			{
				uiBaseUseHistory.Push((key, ui));
			}
			else
			{
				Logger.Log(LogPriority.Warning, $"{uiName} UI는 이미 켜져있습니다.");
			}

			return ui as T;
		}

		/// <summary>
		/// UI가 켜진 히스토리에 따라 꺼짐
		/// </summary>
		public void PutBackUIBase()
		{
			if (uiBaseUseHistory.Count < 1)
			{
				Logger.Log(LogPriority.Warning, "현재 켜져있는 UI가 없습니다.");
				
				return;
			}

			var data = uiBaseUseHistory.Pop();
			
			data.Item2.Hide();
		}

		private void Awake()
		{
			DataPostProcessing();
		}

		private void DataPostProcessing()
		{
			foreach (var ui in uiBases)
			{
				var key = ui.uiName.GetHashCode();
				
				ui.uiBase.Hide();

				if (ui.useFirstUI)
				{
					if (firstUIActiveFlag is not true)
					{
						ui.uiBase.Show();
						
						uiBaseUseHistory.Push((key, ui.uiBase));

						firstUIActiveFlag = true;
					}
					else
					{
						Logger.Log(LogPriority.Error, "첫번째로 보여줄 UI는 2개 이상이 될 수 없습니다. 인스펙터를 확인해주세요.");
					}
				}
				
				uiBaseDictionary.Add(key, ui.uiBase);
			}

			isDataProcessingComplete.Value = true;
		}

		private bool IsKeyValid(string key, out UIBase uiBase)
		{
			var hashKey = key.GetHashCode();

			if (uiBaseDictionary.ContainsKey(hashKey))
			{
				uiBase = uiBaseDictionary[hashKey];
				
				return true;
			}
			
			Logger.Log(LogPriority.Error, $"{key}인 데이터가 존재하지 않습니다. 인스펙터를 확인해주세요.");

			uiBase = null;

			return false;
		}
	}
}

// Code Author: ChangsooPark