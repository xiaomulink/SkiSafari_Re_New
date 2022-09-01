public class GUIShopBonusPopup : GUIShopPopup
{
	public Item_Booster superBlueBoosts;

	public int bonusSuperBlueBoostCount = 5;

	public Item_Booster pocketRockets;

	public int bonusPocketRocketCount = 5;

	protected override void Awake()
	{
		base.Awake();
		superBlueBoosts.booster.CurrentCount += bonusSuperBlueBoostCount;
		pocketRockets.booster.CurrentCount += bonusPocketRocketCount;
	}
}
