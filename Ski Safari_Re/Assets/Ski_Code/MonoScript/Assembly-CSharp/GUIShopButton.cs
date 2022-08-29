using UnityEngine;

public class GUIShopButton : GUIButton
{
	public GUITransitionAnimator transitionAnimator;

	public Vector3 normalOffset = new Vector3(-1.5f, -1.5f, 0f);

	public Vector3 pulseOffset = new Vector3(-2f, -2f, 0f);

	public GameObject newObject;

	private bool m_canBuyAnyItem;

	private float m_scaleTimer;

	public bool CanBuyAnyItem
	{
		get
		{
			return m_canBuyAnyItem;
		}
	}

    void Update()
    {
        if(SkiGameManager.Instance.isOnline)
        {
            transitionAnimator.Hide();
        }
    }

    public override void Click(Vector3 position)
	{
		Deactivate();
		SkiGameManager.Instance.ShowShop = true;
		base.Click(position);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		bool hasNewSelectableItem = ItemManager.Instance.HasNewSelectableItem;
		m_canBuyAnyItem = ItemManager.Instance.CanBuyAnyItem;
		newObject.SetActive(hasNewSelectableItem);
		scaleNode.localPosition = ((!m_canBuyAnyItem) ? normalOffset : pulseOffset);
	}
}
