using System;

public class Achievement_BreakLine : Achievement_Count
{
	public string lineColliderCategory;

	public bool chain;

	public string requiredMountCategory;

	public FlamePowerup.Level requiredIgnitionLevel;

	private void OnBreakLine(Player player, GeometryUtils.ContactInfo contactInfo)
	{
		if (lineColliderCategory == contactInfo.collider.category && Achievement.CheckMountCategory(player, requiredMountCategory) && player.IgnitionLevel >= requiredIgnitionLevel)
		{
			IncrementCount(1);
		}
	}

	private void OnContact(Player player, GeometryUtils.ContactInfo contactInfo)
	{
		if (chain)
		{
			ResetCount();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerBreakLine = (Player.OnCollisionDelegate)Delegate.Combine(Player.OnPlayerBreakLine, new Player.OnCollisionDelegate(OnBreakLine));
		Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Combine(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerBreakLine = (Player.OnCollisionDelegate)Delegate.Remove(Player.OnPlayerBreakLine, new Player.OnCollisionDelegate(OnBreakLine));
		Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Remove(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
	}
}
