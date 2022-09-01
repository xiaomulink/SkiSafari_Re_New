using System;

public class Achievement_Mount : Achievement_Count
{
	public enum Mode
	{
		Mount = 1,
		Attach = 2,
		All = 255
	}

	public string requiredMountCategory;

	public FlamePowerup.Level requiredIgnitionLevel;

	public Mode mode = Mode.Mount;

	public bool airOnly;

	public bool chain;

	private bool m_isMounted;

	private void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		if (Achievement.CheckMountCategory(mountedPlayer, requiredMountCategory) && (!airOnly || (!mountedPlayer.Collider.OnGround && !previousPlayer.Collider.OnGround)) && mountedPlayer.IgnitionLevel >= requiredIgnitionLevel)
		{
			IncrementCount(1);
			m_isMounted = true;
		}
		else if (chain)
		{
			ResetCount();
		}
	}

	private void OnDismount(PlayerSkierMounted mountedPlayer, Player newPlayer)
	{
		m_isMounted = false;
	}

	private void OnPlayerLand(Player player)
	{
		if (chain && !m_isMounted)
		{
			ResetCount();
		}
	}

	private void OnAttach(Player player, Attachment attachment)
	{
		if ((string.IsNullOrEmpty(requiredMountCategory) || requiredMountCategory == attachment.attachmentCategory) && (!airOnly || ((bool)player.Collider && !player.Collider.OnGround)) && player.IgnitionLevel >= requiredIgnitionLevel)
		{
			IncrementCount(1);
		}
	}

	private void OnPlayerSpawn(Player player)
	{
		if (Achievement.CheckMountCategory(player, requiredMountCategory) && player.IgnitionLevel >= requiredIgnitionLevel)
		{
			IncrementCount(1);
			m_isMounted = true;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if ((mode & Mode.Mount) != 0)
		{
			Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
			if (!airOnly)
			{
				SkiGameManager instance = SkiGameManager.Instance;
				instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Combine(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
			}
		}
		Player.OnPlayerDismount = (Player.OnDisountDelegate)Delegate.Combine(Player.OnPlayerDismount, new Player.OnDisountDelegate(OnDismount));
		if ((mode & Mode.Attach) != 0)
		{
			Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Combine(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((mode & Mode.Mount) != 0)
		{
			Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
			if (!airOnly)
			{
				SkiGameManager instance = SkiGameManager.Instance;
				instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Remove(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
			}
		}
		Player.OnPlayerDismount = (Player.OnDisountDelegate)Delegate.Remove(Player.OnPlayerDismount, new Player.OnDisountDelegate(OnDismount));
		if ((mode & Mode.Attach) != 0)
		{
			Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Remove(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnAttach));
		}
	}
}
