using System;
using UnityEngine;

public class UnlockableObject : MonoBehaviour
{
	public Item requiredItem;

	public float chance = 0.5f;

	public GameObject unlockedObject;

	public GameObject lockedObject;

	private void Refresh()
	{
		bool flag = (bool)ItemManager.Instance && ItemManager.Instance.IsItemCurrent(requiredItem) && base.transform.position.x > 0f && UnityEngine.Random.value <= chance;
		unlockedObject.SetActive(flag);
		if ((bool)lockedObject)
		{
			lockedObject.SetActive(!flag);
		}
	}

	private void OnEnable()
	{
		Refresh();
		GUIShop instance = GUIShop.Instance;
		instance.OnItemChanged = (Action)Delegate.Combine(instance.OnItemChanged, new Action(Refresh));
	}

	private void OnDisable()
	{
		GUIShop instance = GUIShop.Instance;
		instance.OnItemChanged = (Action)Delegate.Remove(instance.OnItemChanged, new Action(Refresh));
	}
}
