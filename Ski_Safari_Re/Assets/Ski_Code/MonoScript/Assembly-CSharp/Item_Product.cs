public abstract class Item_Product : Item
{
	public string productId;

	public abstract bool Purchasable { get; }

	public override bool Unlocked
	{
		get
		{
			return GameState.GetInt(base.name) != 0;
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
		}
	}
}
