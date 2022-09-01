using UnityEngine;

public class GUIShopCoinButton : GUIButton
{
	public GUIShopTabButton productTabButton;

	public int productItemIndex;

	public GameObject clickEffectPrefab;

	public GameObject coinCountText;

	private GameObject m_clickEffect;

	public void OnBuyFail()
	{
		coinCountText.transform.localScale = Vector3.one;
		Go.killAllTweensWithTarget(coinCountText.transform);
		Go.to(coinCountText.transform, 0.5f, new GoTweenConfig().scale(0.2f, true).setEaseType(GoEaseType.ElasticPunch));
	}

	private void ShowClickEffect()
	{
		if ((bool)m_clickEffect)
		{
			Object.Destroy(m_clickEffect);
		}
		m_clickEffect = TransformUtils.Instantiate(clickEffectPrefab, base.transform);
	}

	public override void Click(Vector3 position)
	{
		if (GUIShop.Instance.CurrentTabIndex != productTabButton.TabIndex)
		{
			productTabButton.Click(position);
			GUIShop.Instance.CurrentTab.SnapToItemIndex(productItemIndex);
			ShowClickEffect();
			base.Click(position);
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		coinCountText.transform.localScale = Vector3.one;
	}
}
