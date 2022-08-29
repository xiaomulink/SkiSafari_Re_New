public class Item_Skin : Item_Key
{
	public string materialName;

	public string textureName;

	protected override void OnEnable()
	{
		base.OnEnable();
		if ((bool)ThemeManager.Instance)
		{
			ThemeManager.Instance.SetSkinTexture(materialName, textureName);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((bool)ThemeManager.Instance)
		{
			ThemeManager.Instance.SetSkinTexture(materialName, string.Empty);
		}
	}
}
