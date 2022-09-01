public abstract class Item_ConsumableProduct : Item_Product
{
	public override bool Unlocked
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public override bool Purchasable
	{
		get
		{
			return true;
		}
	}
}
