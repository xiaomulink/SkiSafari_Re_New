using System;

public class Achievement_PassThroughBoost : Achievement_Count
{
	public string requiredMountCategory;

	private void OnPlayerBoost(Player player)
	{
		if (!(SkiGameManager.Instance.CurrentDistance < 10f) && Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		BoostTrigger.OnPlayerBoost = (BoostTrigger.OnPlayerBoostDelegate)Delegate.Combine(BoostTrigger.OnPlayerBoost, new BoostTrigger.OnPlayerBoostDelegate(OnPlayerBoost));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		BoostTrigger.OnPlayerBoost = (BoostTrigger.OnPlayerBoostDelegate)Delegate.Remove(BoostTrigger.OnPlayerBoost, new BoostTrigger.OnPlayerBoostDelegate(OnPlayerBoost));
	}
}
