using System;

public class Achievement_Freeze : Achievement_Count
{
	public string requiredMountCategory;

	public string[] requiredAttachmentCategories;

	public bool allowMountOrAttachment;

	private void OnPlayerFreeze(Player previousPlayer, PlayerSkierFrozen frozenPlayer)
	{
		bool flag = Achievement.CheckMountCategory(previousPlayer, requiredMountCategory);
		bool flag2 = true;
		string[] array = requiredAttachmentCategories;
		foreach (string requiredAttachmentCategory in array)
		{
			if (!Achievement.CheckAttachmentCategory(frozenPlayer, requiredAttachmentCategory))
			{
				flag2 = false;
				break;
			}
		}
		if (allowMountOrAttachment)
		{
			if (flag || flag2)
			{
				IncrementCount(1);
			}
		}
		else if (flag && flag2)
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerFreeze = (Player.OnFreezeDelegate)Delegate.Combine(Player.OnPlayerFreeze, new Player.OnFreezeDelegate(OnPlayerFreeze));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerFreeze = (Player.OnFreezeDelegate)Delegate.Remove(Player.OnPlayerFreeze, new Player.OnFreezeDelegate(OnPlayerFreeze));
	}
}
