public class Item_Booster : Item
{
	public Booster booster;

	public int count = 5;

	public string commonIconTextureName = "gui_item_booster_";

	public override bool Unlocked
	{
		get
		{
			return false;
		}
		set
		{
		}
	}
}
