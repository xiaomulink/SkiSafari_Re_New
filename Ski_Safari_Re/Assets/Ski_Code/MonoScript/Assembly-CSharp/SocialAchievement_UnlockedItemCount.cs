using System;

public class SocialAchievement_UnlockedItemCount : SocialAchievement
{
	public string itemSetName;

	public override void Load()
	{
		GUIShop instance = GUIShop.Instance;
		instance.OnItemChanged = (Action)Delegate.Combine(instance.OnItemChanged, new Action(CheckCount));
		CheckCount();
	}

	public override void Unload()
	{
		GUIShop instance = GUIShop.Instance;
		instance.OnItemChanged = (Action)Delegate.Remove(instance.OnItemChanged, new Action(CheckCount));
	}

	protected void CheckCount()
	{
		ItemSet itemSet = ItemManager.Instance.GetItemSet(itemSetName);
		int num = 0;
		Item[] items = itemSet.items;
		foreach (Item item in items)
		{
			if ((item.cost > 0 || item.requiredItem != null) && item.Unlocked)
			{
				num++;
			}
		}
		SocialManager.Instance.SetAchievementProgress(this, num);
	}
}
