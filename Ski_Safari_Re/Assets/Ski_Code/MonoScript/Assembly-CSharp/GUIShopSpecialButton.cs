using UnityEngine;

public class GUIShopSpecialButton : GUIButton
{
	public Renderer sprite;

	public GameObject newObject;

	public GUITransitionAnimator transitionAnimator;

	public Vector3 normalOffset = new Vector3(1.5f, -1.5f, 0f);

	public Vector3 pulseOffset = new Vector3(2f, -2f, 0f);

	private Item m_item;

	private bool m_canBuyItem;

	public bool CanBuyItem
	{
		get
		{
			return m_canBuyItem;
		}
	}

	public override void Click(Vector3 position)
	{
		Deactivate();
		SkiGameManager.Instance.ShowShop = true;
		GUIShop.Instance.SelectItem(m_item);
		base.Click(position);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		UpdateItem();
		if (!m_item)
		{
			base.gameObject.SetActive(false);
			return;
		}
		AssetManager.UpdateTexture(sprite.material, m_item.iconTextureName);
		m_canBuyItem = m_item.CurrentCost <= GameState.CoinCount && !m_item.Unlocked;
		scaleNode.localPosition = ((!m_canBuyItem) ? normalOffset : pulseOffset);
		newObject.SetActive(m_item.IsNew);
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
	}

	private void UpdateItem()
	{
		m_item = null;
		ItemSet itemSet = ItemManager.Instance.GetItemSet("slope");
		Item[] items = itemSet.items;
		foreach (Item item in items)
		{
			if (item.IsDiscounted && !item.Unlocked && (!m_item || m_item.CurrentCost < item.CurrentCost))
			{
				m_item = item;
			}
		}
	}
}
