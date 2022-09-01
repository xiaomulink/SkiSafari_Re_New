using System;
using System.Collections.Generic;

public class Comment_Attach : Comment
{
	public List<string> categories = new List<string>();

	public int requiredAttachmentCount = 1;

	public string requiredMountCategory;

	protected void OnAttach(Player player, Attachment attachment)
	{
		if (categories.Contains(attachment.attachmentCategory))
		{
			CheckPlayer(player);
		}
	}

	private void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		CheckPlayer(mountedPlayer);
	}

	private void CheckPlayer(Player player)
	{
		if (!string.IsNullOrEmpty(requiredMountCategory) && !(player.Category == requiredMountCategory))
		{
			return;
		}
		int num = 0;
		AttachNode[] attachNodes = player.attachNodes;
		foreach (AttachNode attachNode in attachNodes)
		{
			if ((bool)attachNode.Attachment && categories.Contains(attachNode.Attachment.attachmentCategory))
			{
				num++;
			}
		}
		if (num >= requiredAttachmentCount)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Combine(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
	}

	private void OnDisable()
	{
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Remove(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
	}
}
