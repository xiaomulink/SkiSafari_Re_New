using System;

public class Achievement_TriggerTumble : Achievement_Count
{
	public string requiredTargetCategory;

	public string requiredMountCategory;

	protected void OnTumble(PlayerInteractionTrigger trigger, Player sourcePlayer)
	{
		if ((string.IsNullOrEmpty(requiredTargetCategory) || trigger.category == requiredTargetCategory) && Achievement.CheckMountCategory(sourcePlayer, requiredMountCategory))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayerInteractionTrigger.OnTumble = (PlayerInteractionTrigger.OnTumbleDelegate)Delegate.Combine(PlayerInteractionTrigger.OnTumble, new PlayerInteractionTrigger.OnTumbleDelegate(OnTumble));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		PlayerInteractionTrigger.OnTumble = (PlayerInteractionTrigger.OnTumbleDelegate)Delegate.Remove(PlayerInteractionTrigger.OnTumble, new PlayerInteractionTrigger.OnTumbleDelegate(OnTumble));
	}
}
