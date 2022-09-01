public abstract class Item_NonConsumableProduct : Item_Product
{
	public override bool Unlocked
	{
		get
		{
			return base.Unlocked;
		}
		set
		{
			if (value)
			{
				GameState.SetInt(base.name, 1);
			}
			else
			{
				GameState.DeleteKey(base.name);
			}
			Init();
		}
	}

	public override bool Purchasable
	{
		get
		{
			return !Unlocked;
		}
	}
}
