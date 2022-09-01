using System;

public class Achievement_RideAvalanche : Achievement_Count
{
	public string requiredMountCategory;

	private void OnRideAvalanche(Player player)
	{
		if (Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Stunt_RideAvalanche.OnRideAvalanche = (Player.SimplePlayerDelegate)Delegate.Combine(Stunt_RideAvalanche.OnRideAvalanche, new Player.SimplePlayerDelegate(OnRideAvalanche));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Stunt_RideAvalanche.OnRideAvalanche = (Player.SimplePlayerDelegate)Delegate.Remove(Stunt_RideAvalanche.OnRideAvalanche, new Player.SimplePlayerDelegate(OnRideAvalanche));
	}
}
