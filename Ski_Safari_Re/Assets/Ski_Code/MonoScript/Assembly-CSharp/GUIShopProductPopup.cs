using System;
using UnityEngine;

public class GUIShopProductPopup : GUIShopPopup
{
	[Serializable]
	public class PlatformText
	{
		public string[] lines;
	}

	public TextMesh[] lineMeshes;

	public PlatformText appStoreText;

	public PlatformText googlePlayText;

	public PlatformText amazonText;

	private void SetupPlatform(PlatformText platformText)
	{
		for (int i = 0; i < lineMeshes.Length; i++)
		{
			if (i < platformText.lines.Length)
			{
				lineMeshes[i].text = platformText.lines[i];
			}
			else
			{
				lineMeshes[i].text = string.Empty;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			SetupPlatform(appStoreText);
			break;
		case RuntimePlatform.Android:
			SetupPlatform(googlePlayText);
			break;
		case RuntimePlatform.PS3:
		case RuntimePlatform.XBOX360:
			break;
		}
	}
}
