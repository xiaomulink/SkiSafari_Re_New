public class Item_Gift : Item_Product
{
	public string giftURL;

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
