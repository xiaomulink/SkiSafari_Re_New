using System;

public class Achievement_SnowballRecover : Achievement_Count
{
	public string requiredMountCategory;

	private void OnPlayerRecover(PlayerSnowball previousPlayer, Player player)
	{
		if (Achievement.CheckMountCategory(previousPlayer, requiredMountCategory))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayerSnowball.OnPlayerRecover = (PlayerSnowball.OnPlayerRecoverDelegate)Delegate.Combine(PlayerSnowball.OnPlayerRecover, new PlayerSnowball.OnPlayerRecoverDelegate(OnPlayerRecover));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		PlayerSnowball.OnPlayerRecover = (PlayerSnowball.OnPlayerRecoverDelegate)Delegate.Remove(PlayerSnowball.OnPlayerRecover, new PlayerSnowball.OnPlayerRecoverDelegate(OnPlayerRecover));
	}
}
