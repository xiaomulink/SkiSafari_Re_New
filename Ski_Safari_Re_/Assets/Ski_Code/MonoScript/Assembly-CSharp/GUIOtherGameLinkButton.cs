using UnityEngine;

public class GUIOtherGameLinkButton : GUIButton
{
	public string url = "itms-apps://itunes.com/apps/rocketbunnies";

	public override void Click(Vector3 position)
	{
		base.Click(position);
		Application.OpenURL(url);
	}
}
