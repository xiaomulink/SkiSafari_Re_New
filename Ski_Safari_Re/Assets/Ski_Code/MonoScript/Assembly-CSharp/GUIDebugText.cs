using UnityEngine;
using UnityEngine.UI;

public class GUIDebugText : MonoBehaviour
{
	private void Update()
	{
		if (Time.timeScale > 0f)
		{
			float num = 1f / Time.smoothDeltaTime;
			if (num < 100f)
			{
				GetComponent<Text>().text = string.Format("{0:0} fps", num);
			}
		}
		else
		{
			GetComponent<Text>().text = "paused";
		}
	}
}
