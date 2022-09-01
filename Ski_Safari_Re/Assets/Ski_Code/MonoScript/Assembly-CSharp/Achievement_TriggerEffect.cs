using System;

public class Achievement_TriggerEffect : Achievement_Count
{
	public string requiredEffectCategory;

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

	private void OnPlayerTriggerEffect(Player player, EffectTrigger effectTrigger)
	{
		if ((string.IsNullOrEmpty(requiredEffectCategory) || effectTrigger.effectCategory == requiredEffectCategory) && CheckMountAndAttachment(player))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		EffectTrigger.OnPlayerTriggerEffect = (EffectTrigger.OnPlayerTriggerEffectDelegate)Delegate.Combine(EffectTrigger.OnPlayerTriggerEffect, new EffectTrigger.OnPlayerTriggerEffectDelegate(OnPlayerTriggerEffect));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		EffectTrigger.OnPlayerTriggerEffect = (EffectTrigger.OnPlayerTriggerEffectDelegate)Delegate.Remove(EffectTrigger.OnPlayerTriggerEffect, new EffectTrigger.OnPlayerTriggerEffectDelegate(OnPlayerTriggerEffect));
	}
}
