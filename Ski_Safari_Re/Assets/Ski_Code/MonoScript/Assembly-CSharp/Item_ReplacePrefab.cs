using UnityEngine;

public class Item_ReplacePrefab : Item
{
	public GameObject oldPrefab;

	public GameObject newPrefab;

	public string materialName;

	public string oldTextureName;

	public string newTextureName;

	protected override void OnEnable()
	{
		base.OnEnable();
		Pool.Replace(oldPrefab, newPrefab);
		if ((bool)ThemeManager.Instance && !string.IsNullOrEmpty(materialName))
		{
			ThemeManager.Instance.SetSkinTexture(materialName, newTextureName);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Pool.Revert(oldPrefab);
		if ((bool)ThemeManager.Instance && !string.IsNullOrEmpty(materialName))
		{
			ThemeManager.Instance.SetSkinTexture(materialName, oldTextureName);
		}
	}
}
