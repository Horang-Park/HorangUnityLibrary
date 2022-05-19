using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modules.SoundManager;
using UnityEditor;
using UnityEngine;
using Utilities;
using Logger = Utilities.Logger;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerCustom : Editor
{
	private AudioClipType audioClipType;
	
	private float audioClipVolume = 1.0f;
	private float audioClipPanning;
	private string deleteAudioClipName;
	private bool dictionaryAlreadyUpdated;
	private string[] audioClipTypeOptions = new[]
	{
		"BGM",
		"LoopableSFX",
		"OneShotSFX",
	};
	
	private const string SelectedAudioClipNameSeparator = "\n";
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		var labelTextGuiStyle = new GUIStyle
		{
			richText = true,
			wordWrap = true,
		};

		EditorStyles.textField.wordWrap = true;

		base.OnInspectorGUI();
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		#region 오디오 클립 속성 지정
		
		GUILayout.BeginVertical();
		GUILayout.Label("<color=yellow><b><size=13>오디오 클립 임포트 세팅</size></b></color>", labelTextGuiStyle);
		
		// 타입 선택
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("<color=white>타입</color>", labelTextGuiStyle);
		audioClipType = (AudioClipType)EditorGUILayout.Popup(string.Empty, (int)audioClipType, audioClipTypeOptions);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		// 볼륨 선택
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("<color=white>볼륨</color>", labelTextGuiStyle);
		audioClipVolume = EditorGUILayout.Slider(audioClipVolume, 0.0f, 1.0f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		// 패닝
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("<color=white>패닝</color>", labelTextGuiStyle);
		audioClipPanning = EditorGUILayout.Slider(audioClipPanning, -1.0f, 1.0f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		// 선택한 오디오 클립 이름 표시
		GUILayout.Label("<color=white>현재 선택된 오디오 클립 이름</color>", labelTextGuiStyle);
		var selectAudioClipNameStringBuilder = new StringBuilder("<color=#00ff00><b><size=14>");
		var selectedAudioClips = GetSelectedAudioClips();
		var audioClips = selectedAudioClips as AudioClip[] ?? selectedAudioClips.ToArray();

		foreach (var audioClip in audioClips)
		{
			selectAudioClipNameStringBuilder.Append(audioClip.name);
			selectAudioClipNameStringBuilder.Append(SelectedAudioClipNameSeparator);
		}

		selectAudioClipNameStringBuilder.Append("</size></b></color>");
		
		EditorGUILayout.LabelField(selectAudioClipNameStringBuilder.ToString(), labelTextGuiStyle);
		GUILayout.EndVertical();
		
		#endregion

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		#region 오디오 클립 불러오기
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("<color=yellow><b><size=13>오디오 클립 등록</size></b></color>", labelTextGuiStyle);
		GUILayout.Label("<color=#ff00ff><b><size=13>※ 인스펙터 잠금을 사용해주세요. ※</size></b></color>", labelTextGuiStyle);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginVertical();

		var originalColor = GUI.backgroundColor;
		GUI.backgroundColor = Color.green;
		
		if (GUILayout.Button("선택한 오디오 클립 불러오기", GUILayout.Height(50.0f)))
		{
			foreach (var audioClip in audioClips)
			{
				var audioData = new AudioClipData
				{
					clip = audioClip,
					type = audioClipType,
					hashKey = audioClip.name.GetHashCode(),
					
					name = audioClip.name,
					volume = audioClipVolume,
					panning = audioClipPanning,
				};
				
				SoundManager.AddAudioClip(audioData);
			}
		}
		EditorGUILayout.EndVertical();
		
		#endregion
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		#region 오디오 클립 삭제
		
		GUILayout.Label("<color=red><b><size=13>오디오 클립 삭제</size></b></color>", labelTextGuiStyle);
		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal();
		
		GUI.backgroundColor = Color.red;
		
		deleteAudioClipName = EditorGUILayout.TextField(deleteAudioClipName, GUILayout.Height(30.0f));
			
		if (GUILayout.Button("입력한 오디오 클립 삭제", GUILayout.Height(30.0f)))
		{
			var key = deleteAudioClipName.GetHashCode();

			if (SoundManager.RemoveSpecificAudioClip(key))
			{
				Logger.Log(LogPriority.Verbose, $"{deleteAudioClipName} 오디오 클립이 삭제되었습니다.");

				deleteAudioClipName = string.Empty;
				
				return;
			}
		}
		EditorGUILayout.EndHorizontal();
		
		if (GUILayout.Button("오디오 클립 전체 삭제", GUILayout.Height(30.0f)))
		{
			SoundManager.RemoveAllAudioClip();
		}
		EditorGUILayout.EndVertical();
		GUI.color = originalColor;
		
		#endregion
	}

	private static IEnumerable<AudioClip> GetSelectedAudioClips()
	{
		var objList = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
		var clipList = new AudioClip[objList.Length];

		for (var i = 0; i < objList.Length; i++)
		{
			clipList[i] = objList[i] as AudioClip;
		}

		return clipList;
	}
}

// Code Author: ChangsooPark