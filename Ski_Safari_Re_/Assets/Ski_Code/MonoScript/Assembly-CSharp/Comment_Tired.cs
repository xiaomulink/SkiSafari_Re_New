using System;

public class Comment_Tired : Comment
{
	public string category = "penguin";

	protected void OnMountDetach(PlayerSkierMounted player)
	{
		if (player.mountCategory == category)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		PlayerSkierMounted.OnMountDetach = (PlayerSkierMounted.OnMountDetachDelegate)Delegate.Combine(PlayerSkierMounted.OnMountDetach, new PlayerSkierMounted.OnMountDetachDelegate(OnMountDetach));
	}

	private void OnDisable()
	{
		PlayerSkierMounted.OnMountDetach = (PlayerSkierMounted.OnMountDetachDelegate)Delegate.Remove(PlayerSkierMounted.OnMountDetach, new PlayerSkierMounted.OnMountDetachDelegate(OnMountDetach));
	}
}
