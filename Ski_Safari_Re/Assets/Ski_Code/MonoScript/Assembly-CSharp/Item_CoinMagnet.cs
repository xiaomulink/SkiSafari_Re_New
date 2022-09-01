using UnityEngine;

public class Item_CoinMagnet : Item
{
	public CoinMagnet prefab;

	protected override void OnEnable()
	{
		base.OnEnable();
		Object.Instantiate(prefab);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((bool)CoinMagnet.Instance)
		{
			Object.Destroy(CoinMagnet.Instance.gameObject);
		}
	}
}
