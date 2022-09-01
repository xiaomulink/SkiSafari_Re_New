using System;

public class Achievement_HitHazard : Achievement_Count
{
	public string requiredHazardCategory;

	public string requiredMountCategory;

	public FlamePowerup.Level requiredIgnitionLevel;

	private void OnHitHazard(Player previousPlayer, Player player, Hazard hazard)
	{
		if ((string.IsNullOrEmpty(requiredHazardCategory) || requiredHazardCategory == hazard.category) && Achievement.CheckMountCategory(previousPlayer, requiredMountCategory) && player.IgnitionLevel >= requiredIgnitionLevel)
		{
			IncrementCount(hazard.count);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerHitHazard = (Player.OnHitHazardDelegate)Delegate.Combine(Player.OnPlayerHitHazard, new Player.OnHitHazardDelegate(OnHitHazard));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerHitHazard = (Player.OnHitHazardDelegate)Delegate.Remove(Player.OnPlayerHitHazard, new Player.OnHitHazardDelegate(OnHitHazard));
	}
}
