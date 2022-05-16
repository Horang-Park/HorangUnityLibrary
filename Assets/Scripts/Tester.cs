using System.Collections.Generic;
using Modules.PlayerPrefs;
using UnityEngine;

public class Tester : MonoBehaviour
{
	private int testIntegerValue = 19961021;
	private int[] testIntegerArrayValue = new int[]
	{
		2022,
		05,
		13,
		16,
		04,
	};
	
	private float testFloatValue = 3.14159f;
	private float[] testFloatArrayValue = new float[]
	{
		20.22f,
		0.5f,
		1.3f,
		1.6f,
		0.5f,
	};
	
	private string testStringValue = "This is test string";

	private int getIntegerValue;
	private List<int> getIntegerArrayValue;
	
	private float getFloatValue;
	private List<float> getFloatArrayValue;

	private string getStringValue;

	private void Awake()
	{
		PlayerPrefsManager.SetPlayerPrefs.SetInt("TestInteger", testIntegerValue);
		PlayerPrefsManager.SetPlayerPrefs.SetInt("TestIntegerArray", testIntegerArrayValue);
		PlayerPrefsManager.SetPlayerPrefs.SetFloat("TestFloat", testFloatValue);
		PlayerPrefsManager.SetPlayerPrefs.SetFloat("TestFloatArray", testFloatArrayValue);
		PlayerPrefsManager.SetPlayerPrefs.SetString("TestString", testStringValue);
	}

	private void Start()
	{
		getIntegerValue = PlayerPrefsManager.GetPlayerPrefs.GetInt("TestInteger");
		getIntegerArrayValue = PlayerPrefsManager.GetPlayerPrefs.GetIntArray("TestIntegerArray");
		getFloatValue = PlayerPrefsManager.GetPlayerPrefs.GetFloat("TestFloat");
		getFloatArrayValue = PlayerPrefsManager.GetPlayerPrefs.GetFloatArray("TestFloatArray");
		getStringValue = PlayerPrefsManager.GetPlayerPrefs.GetString("TestString");
	}
}
