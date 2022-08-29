using System;

public class Comment_SnowballRecover : Comment
{
	public string mountCategory = "pandarolling";

	private void OnPlayerRecover(PlayerSnowball previousPlayer, Player player)
	{
		if (previousPlayer.Category == mountCategory)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		PlayerSnowball.OnPlayerRecover = (PlayerSnowball.OnPlayerRecoverDelegate)Delegate.Combine(PlayerSnowball.OnPlayerRecover, new PlayerSnowball.OnPlayerRecoverDelegate(OnPlayerRecover));
	}

	private void OnDisable()
	{
		PlayerSnowball.OnPlayerRecover = (PlayerSnowball.OnPlayerRecoverDelegate)Delegate.Remove(PlayerSnowball.OnPlayerRecover, new PlayerSnowball.OnPlayerRecoverDelegate(OnPlayerRecover));
	}
}
