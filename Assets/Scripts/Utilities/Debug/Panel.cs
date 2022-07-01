using UnityEngine;

namespace Utilities.Debug
{
	public class Panel : MonoBehaviour
	{
#if DEBUG_MODE_PANEL
		private float deltaTime;

		private void Update()
		{
			deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		}

		private void OnGUI()
		{
			var width = Screen.width;
			var height = Screen.height;
			var guiStyle = new GUIStyle();
			var rect = new Rect(0.0f, 0.0f, width, height * 0.02f);

			guiStyle.alignment = TextAnchor.UpperLeft;
			guiStyle.fontSize = height * 2 / 30;
			guiStyle.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			var deltaMillisecond = deltaTime * 1000.0f;
			var frame = 1.0f / deltaTime;
			var text = $"{deltaMillisecond:0.0}ms → {frame:0}fps";
			
			GUI.Label(rect, text, guiStyle);
		}
#endif
	}
}