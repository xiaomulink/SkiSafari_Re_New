using System;

public class Comment_Follow : Comment
{
	public string category = "reindeer";

	public int minFollowers;

	public int maxFollowers = 1;

	protected void OnFollow(Player leader)
	{
		if (leader.Category == category && leader.Followers.Count <= maxFollowers && leader.Followers.Count >= minFollowers)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		Player.OnPlayerFollow = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerFollow, new Player.SimplePlayerDelegate(OnFollow));
	}

	private void OnDisable()
	{
		Player.OnPlayerFollow = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerFollow, new Player.SimplePlayerDelegate(OnFollow));
	}
}
