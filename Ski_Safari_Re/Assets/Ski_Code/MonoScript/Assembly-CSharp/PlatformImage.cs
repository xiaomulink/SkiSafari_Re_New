using UnityEngine;

public class PlatformImage : MonoBehaviour
{
	public Material material;

	public string testImageName;

	public string iOSImageName;

	public string googlePlayImageName;

	public string amazonImageName;

	public string nookImageName;

	private string GetImageName()
	{
		return googlePlayImageName;
	}

	private void OnEnable()
	{
		if ((bool)ThemeManager.Instance)
		{
			string imageName = GetImageName();
			if (!string.IsNullOrEmpty(imageName))
			{
				material.mainTexture = AssetManager.LoadAsset<Texture>(imageName);
			}
		}
	}

	private void OnDisable()
	{
		AssetManager.UnloadTexture(material);
	}
}
