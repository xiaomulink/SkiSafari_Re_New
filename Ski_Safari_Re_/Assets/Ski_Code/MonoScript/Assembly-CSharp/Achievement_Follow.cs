using System;

public class Achievement_Follow : Achievement_Count
{
	public string requiredLeaderCategory;

	public string[] requiredFollowerCategories;

	private void OnFollow(Player leader)
	{
		if (!Achievement.CheckMountCategory(leader, requiredLeaderCategory) || leader.Followers.Count != requiredFollowerCategories.Length)
		{
			return;
		}
		for (int i = 0; i < requiredFollowerCategories.Length; i++)
		{
			if (leader.Followers[i].followerCategory != requiredFollowerCategories[i])
			{
				return;
			}
		}
		IncrementCount(1);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerFollow = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerFollow, new Player.SimplePlayerDelegate(OnFollow));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerFollow = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerFollow, new Player.SimplePlayerDelegate(OnFollow));
	}
}
