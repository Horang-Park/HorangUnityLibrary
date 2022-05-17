using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modules.SoundManager;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerCustom : Editor
{
	public AudioClipType audioClipType;
	
	private string[] audioClipTypeOptions = new[]
	{
		"BGM",
		"StoppableSFX",
		"LoopableSFX",
		"OneShotSFX",
	};
	private const char SelectedAudioClipNameSeparator = '\n';
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		var labelTextGuiStyle = new GUIStyle
		{
			richText = true,
		};

		base.OnInspectorGUI();
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		GUILayout.BeginVertical();
		GUILayout.Label("<color=yellow><b><size=13>오디오 클립 임포트 세팅</size></b></color>", labelTextGuiStyle);

		// 타입 선택
		audioClipType = (AudioClipType)EditorGUILayout.Popup("AudioClipType", (int)audioClipType, audioClipTypeOptions);
		////////////////////////////////////////////////////////////
		
		// 선택한 오디오 클립 이름 표시
		GUILayout.Label("현재 선택된 오디오 클립 이름");
		var selectAudioClipNameStringBuilder = new StringBuilder("<color=#00ff00><b>");
		var selectedAudioClips = GetSelectedAudioClips();
		var audioClips = selectedAudioClips as AudioClip[] ?? selectedAudioClips.ToArray();

		foreach (var audioClip in audioClips)
		{
			selectAudioClipNameStringBuilder.Append(audioClip.name);
			selectAudioClipNameStringBuilder.Append(SelectedAudioClipNameSeparator);
		}

		selectAudioClipNameStringBuilder.Append("</b></color>");
		
		GUILayout.Label(selectAudioClipNameStringBuilder.ToString(), labelTextGuiStyle);
		GUILayout.EndVertical();
		////////////////////////////////////////////////////////////

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		// 버튼 (임포트, 전체 삭제)
		GUILayout.Label("<color=yellow><b><size=13>오디오 클립 임포트</size></b></color>", labelTextGuiStyle);
		EditorGUILayout.BeginVertical();
		GUILayout.Label("<color=red><b><size=15>※ 인스펙터 잠금을 사용해주세요. ※</size></b></color>", labelTextGuiStyle);

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("선택한 오디오 클립 임포트", GUILayout.Height(30.0f)))
		{
			foreach (var audioClip in audioClips)
			{
				var audioData = new AudioClipData
				{
					clip = audioClip,
					type = audioClipType,
					name = audioClip.name,
				};
				
				SoundManager.Instance.AddAudioClip(audioData);
			}
		}
		if (GUILayout.Button("오디오 클립 전체 삭제", GUILayout.Height(30.0f)))
		{
			SoundManager.Instance.RemoveAllAudioClip();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		////////////////////////////////////////////////////////////
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

	private void OnEnable()
	{
		SoundManager.UpdateDictionary();
	}
}