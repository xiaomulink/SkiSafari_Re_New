using System;

public class Achievement_Collect : Achievement_Count
{
	public string requiredCollectableCategory;

	public string requiredMountCategory;

	public string requiredAttachmentCategory;

	public bool eitherMountOrAttachment = true;

	private bool CheckMountAndAttachment(Player player)
	{
		if (eitherMountOrAttachment)
		{
			return Achievement.CheckMountCategory(player, requiredMountCategory) || Achievement.CheckAttachmentCategory(player, requiredAttachmentCategory);
		}
		return Achievement.CheckMountCategory(player, requiredMountCategory) && Achievement.CheckAttachmentCategory(player, requiredAttachmentCategory);
	}

	private void OnPlayerCollect(Player player, Collectable collectable)
	{
		if ((string.IsNullOrEmpty(requiredCollectableCategory) || collectable.category == requiredCollectableCategory) && CheckMountAndAttachment(player))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Collectable.OnPlayerCollect = (Collectable.OnPlayerCollectDelegate)Delegate.Combine(Collectable.OnPlayerCollect, new Collectable.OnPlayerCollectDelegate(OnPlayerCollect));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Collectable.OnPlayerCollect = (Collectable.OnPlayerCollectDelegate)Delegate.Remove(Collectable.OnPlayerCollect, new Collectable.OnPlayerCollectDelegate(OnPlayerCollect));
	}
}
