public class Item_CoinDoubler : Item_NonConsumableProduct
{
	public override void Init()
	{
		base.Init();
		if ((bool)SkiGameManager.Instance)
		{
			if (Unlocked)
			{
				SkiGameManager.Instance.coinMultiplier = 2;
			}
			else
			{
				SkiGameManager.Instance.coinMultiplier = 1;
			}
		}
	}
}
