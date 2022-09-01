using UnityEngine;

public class GUIShopTabButton : GUIButton
{
	public string[] itemSetNames;

	public GameObject activeObject;

	public GameObject inactiveObject;

	public GameObject newObject;

	public int TabIndex { get; set; }

	public void SetToggled(bool toggled)
	{
		activeObject.SetActive(toggled);
		inactiveObject.SetActive(!toggled);
	}

	public override void Click(Vector3 position)
	{
		if (GUIShop.Instance.CurrentTabIndex != TabIndex)
		{
			GUIShop.Instance.CurrentTabIndex = TabIndex;
			base.Click(position);
		}
	}
}
