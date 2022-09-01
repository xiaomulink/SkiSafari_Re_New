using UnityEngine;

public class GUIVersion : MonoBehaviour
{
	public GUIDropShadowText text;

	protected void Start()
	{
		string version = AppInfo.Version;
		if (version != string.Empty)
		{
			text.Text = "v" + version;
		}
		else
		{
			text.Text = string.Empty;
		}
	}
}
