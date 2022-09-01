using System;

public class Achievement_NoInputDistance : Achievement_Distance
{
	private void OnPlayerInputPressed(Player player)
	{
		ClearDistance();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerInputPressed = (Player.SimplePlayerDelegate)Delegate.Combine(instance.OnPlayerInputPressed, new Player.SimplePlayerDelegate(OnPlayerInputPressed));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerInputPressed = (Player.SimplePlayerDelegate)Delegate.Remove(instance.OnPlayerInputPressed, new Player.SimplePlayerDelegate(OnPlayerInputPressed));
	}
}
