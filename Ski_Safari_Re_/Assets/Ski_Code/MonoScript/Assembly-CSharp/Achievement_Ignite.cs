using System;

public class Achievement_Ignite : Achievement_Count
{
	public string requiredMountCategory;

	public FlamePowerup.Level requiredIgnitionLevel;

	private void OnPlayerIgnite(Player player, FlamePowerup.Level ignitionLevel)
	{
		if (ignitionLevel >= requiredIgnitionLevel && Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerIgnite = (Player.OnIgniteDelegate)Delegate.Combine(Player.OnPlayerIgnite, new Player.OnIgniteDelegate(OnPlayerIgnite));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerIgnite = (Player.OnIgniteDelegate)Delegate.Remove(Player.OnPlayerIgnite, new Player.OnIgniteDelegate(OnPlayerIgnite));
	}
}
