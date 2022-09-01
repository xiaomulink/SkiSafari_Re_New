using System;

public class Achievement_GetUp : Achievement_Count
{
	public bool airOnly;

	private void OnGetUp(PlayerSkierBellyslide previousPlayer, Player player)
	{
		if (!airOnly || ((bool)player.Collider && !player.Collider.OnGround))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayerSkierBellyslide.OnPlayerGetUp = (PlayerSkierBellyslide.OnPlayerGetUpDelegate)Delegate.Combine(PlayerSkierBellyslide.OnPlayerGetUp, new PlayerSkierBellyslide.OnPlayerGetUpDelegate(OnGetUp));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		PlayerSkierBellyslide.OnPlayerGetUp = (PlayerSkierBellyslide.OnPlayerGetUpDelegate)Delegate.Remove(PlayerSkierBellyslide.OnPlayerGetUp, new PlayerSkierBellyslide.OnPlayerGetUpDelegate(OnGetUp));
	}
}
