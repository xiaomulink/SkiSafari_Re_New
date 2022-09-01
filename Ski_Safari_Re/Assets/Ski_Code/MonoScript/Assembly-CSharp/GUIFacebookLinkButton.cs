using UnityEngine;

public class GUIFacebookLinkButton : GUIButton
{
	public string appUrl = "fb://profile/100363946788083";

	public string webUrl = "http://www.facebook.com/skisafarigame";

	public override void Click(Vector3 position)
	{
		base.Click(position);
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");

			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass(Application.identifier + ".FacebookLink"))
			{
				androidJavaClass2.CallStatic("open", @static, appUrl, webUrl);
			}
		}
	}
}
