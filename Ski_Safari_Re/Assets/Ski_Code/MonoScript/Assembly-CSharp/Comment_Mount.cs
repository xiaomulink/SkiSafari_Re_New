using System;

public class Comment_Mount : Comment
{
	public string category = "penguin";

	public bool groundOnly;

	public bool airOnly;

	protected void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		bool flag = previousPlayer.Collider.OnGround || mountedPlayer.Collider.OnGround;
		if (mountedPlayer.Category == category && (!groundOnly || flag) && (!airOnly || !flag))
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
	}

	private void OnDisable()
	{
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
	}
}
