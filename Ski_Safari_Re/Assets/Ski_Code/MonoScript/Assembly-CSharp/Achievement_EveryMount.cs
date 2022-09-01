using System;

public class Achievement_EveryMount : Achievement_Count
{
	public string requiredMountCategory;

	private void CheckPlayer(Player player)
	{
		PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
		if (!playerSkierMounted || !(playerSkierMounted.mountCategory == requiredMountCategory))
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		AttachNode[] attachNodes = playerSkierMounted.attachNodes;
		foreach (AttachNode attachNode in attachNodes)
		{
			if ((bool)attachNode.Attachment)
			{
				switch (attachNode.Attachment.attachmentCategory)
				{
				case "penguin":
					flag = true;
					break;
				case "yeti":
					flag2 = true;
					break;
				case "eagle":
					flag3 = true;
					break;
				}
			}
		}
		if (flag && flag2 && flag3)
		{
			Complete();
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
		PlayerSkier playerSkier = player as PlayerSkier;
		if ((bool)playerSkier)
		{
			int num = 0;
			AttachNode[] attachNodes = playerSkier.attachNodes;
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
		}
		CheckPlayer(player);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Combine(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Remove(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
	}
}
