using System;

public class Achievement_Taxi : Achievement_Count
{
	public enum AttachmentMode
	{
		AllTogether = 0,
		CountEachAttachment = 1
	}

	public string requiredMountCategory;

	public string[] requiredAttachmentCategories;

	public AttachmentMode attachmentMode;

	private bool IsRequiredAttachment(string attachmentCategory)
	{
		string[] array = requiredAttachmentCategories;
		foreach (string text in array)
		{
			if (attachmentCategory == text)
			{
				return true;
			}
		}
		return false;
	}

	private void CountAttachments(AttachNode[] attachNodes)
	{
		foreach (AttachNode attachNode in attachNodes)
		{
			if ((bool)attachNode.Attachment && IsRequiredAttachment(attachNode.Attachment.attachmentCategory))
			{
				IncrementCount(1);
			}
		}
	}

	private void CheckPlayer(Player player)
	{
		if (!Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			return;
		}
		switch (attachmentMode)
		{
		case AttachmentMode.AllTogether:
		{
			string[] array = requiredAttachmentCategories;
			foreach (string requiredAttachmentCategory in array)
			{
				if (!Achievement.CheckAttachmentCategory(player, requiredAttachmentCategory))
				{
					return;
				}
			}
			IncrementCount(1);
			break;
		}
		case AttachmentMode.CountEachAttachment:
			CountAttachments(player.attachNodes);
			{
				foreach (Follower follower in player.Followers)
				{
					if (IsRequiredAttachment(follower.followerCategory))
					{
						IncrementCount(1);
					}
					CountAttachments(follower.attachNodes);
				}
				break;
			}
		}
	}

	private void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		if (mountedPlayer.Category == requiredMountCategory)
		{
			AchievementManager.Instance.OnIncrement(this);
		}
		CheckPlayer(mountedPlayer);
	}

	private void OnAttach(Player player, Attachment attachment)
	{
		if (!IsRequiredAttachment(attachment.attachmentCategory) || !Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			return;
		}
		switch (attachmentMode)
		{
		case AttachmentMode.AllTogether:
		{
			int num = 0;
			AttachNode[] attachNodes = player.attachNodes;
			foreach (AttachNode attachNode in attachNodes)
			{
				if ((bool)attachNode.Attachment && attachNode.Attachment.attachmentCategory == attachment.attachmentCategory)
				{
					num++;
				}
			}
			if (num == 1)
			{
				AchievementManager.Instance.OnIncrement(this);
			}
			CheckPlayer(player);
			break;
		}
		case AttachmentMode.CountEachAttachment:
			IncrementCount(1);
			break;
		}
	}

	private void OnFollow(Player leader)
	{
		if (leader.Category == requiredMountCategory)
		{
			AchievementManager.Instance.OnIncrement(this);
		}
		CheckPlayer(leader);
	}

	private void OnSpawn(Player player)
	{
		CheckPlayer(player);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Combine(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
		Player.OnPlayerFollow = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerFollow, new Player.SimplePlayerDelegate(OnFollow));
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Combine(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnSpawn));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Remove(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
		Player.OnPlayerFollow = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerFollow, new Player.SimplePlayerDelegate(OnFollow));
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Remove(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnSpawn));
	}
}
