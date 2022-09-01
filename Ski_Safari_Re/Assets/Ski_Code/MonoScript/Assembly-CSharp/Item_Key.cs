using UnityEngine;

public class Item_Key : Item
{
	public string key;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayerPrefs.SetInt(key, 1);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		PlayerPrefs.DeleteKey(key);
	}
}
