using System;

public class SocialAchievement_UsedItemCount : SocialAchievement
{
	public string itemSetName;

	public override void Load()
	{
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Combine(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
		CheckCount();
	}

	public override void Unload()
	{
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Remove(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
	}

	protected void CheckCount()
	{
		ItemSet itemSet = ItemManager.Instance.GetItemSet(itemSetName);
		int num = 0;
		Item[] items = itemSet.items;
		foreach (Item item in items)
		{
			if (item.HasBeenUsed)
			{
				num++;
			}
		}
		SocialManager.Instance.SetAchievementProgress(this, num);
	}

	private void OnPlayerSpawn(Player player)
	{
		CheckCount();
	}
}
