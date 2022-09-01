using System;

public class Comment_Freeze : Comment
{
	protected void OnFreeze(Player previousPlayer, PlayerSkierFrozen frozenPlayer)
	{
		ShowMessagesAndComplete();
	}

	private void OnEnable()
	{
		Player.OnPlayerFreeze = (Player.OnFreezeDelegate)Delegate.Combine(Player.OnPlayerFreeze, new Player.OnFreezeDelegate(OnFreeze));
	}

	private void OnDisable()
	{
		Player.OnPlayerFreeze = (Player.OnFreezeDelegate)Delegate.Remove(Player.OnPlayerFreeze, new Player.OnFreezeDelegate(OnFreeze));
	}
}
