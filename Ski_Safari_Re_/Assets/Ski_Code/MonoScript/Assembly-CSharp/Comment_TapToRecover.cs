using System;

public class Comment_TapToRecover : Comment
{
	private void OnPlayerTakeDamage(Player previousPlayer, Player player)
	{
		if (player is PlayerSkierBellyslide)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Combine(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnPlayerTakeDamage));
	}

	private void OnDisable()
	{
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Remove(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnPlayerTakeDamage));
	}
}
