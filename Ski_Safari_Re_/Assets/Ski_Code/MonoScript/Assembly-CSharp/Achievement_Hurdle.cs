using System;

public class Achievement_Hurdle : Achievement_Count
{
	public string requiredHurdleCategory;

	public string requiredMountCategory;

	public bool requiresCaves;

	private void OnJumpHurdle(Player player, Hurdle hurdle, bool insideCave)
	{
		if (Achievement.CheckMountCategory(player, requiredMountCategory) && (string.IsNullOrEmpty(requiredHurdleCategory) || hurdle.category == requiredHurdleCategory) && (!requiresCaves || insideCave))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Hurdle.OnJump = (Hurdle.OnJumpDelegate)Delegate.Combine(Hurdle.OnJump, new Hurdle.OnJumpDelegate(OnJumpHurdle));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Hurdle.OnJump = (Hurdle.OnJumpDelegate)Delegate.Remove(Hurdle.OnJump, new Hurdle.OnJumpDelegate(OnJumpHurdle));
	}
}
