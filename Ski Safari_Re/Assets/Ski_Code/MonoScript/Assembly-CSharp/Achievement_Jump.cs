using System;

public class Achievement_Jump : Achievement_Count
{
	public string requiredMountCategory;

	private void OnSkierJump(Skier skier)
	{
		Player instance = Player.Instance;
		if ((bool)instance && instance.Collider == skier.Collider && !(instance is PlayerSkierBellyslide) && !(instance is PlayerPiledrive) && !(instance is PlayerSkierFrozen) && Achievement.CheckMountCategory(instance, requiredMountCategory))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Skier.OnSkierJump = (Skier.OnSkierJumpDelegate)Delegate.Combine(Skier.OnSkierJump, new Skier.OnSkierJumpDelegate(OnSkierJump));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Skier.OnSkierJump = (Skier.OnSkierJumpDelegate)Delegate.Remove(Skier.OnSkierJump, new Skier.OnSkierJumpDelegate(OnSkierJump));
	}
}
