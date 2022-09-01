using System;

public class Achievement_Transfer : Achievement_Count
{
	public string[] mountCategories;

	public bool airOnly;

	private bool m_isMounted;

	private void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		if ((!airOnly || mountedPlayer.Collider.OnGround) && (string.IsNullOrEmpty(mountCategories[base.Count]) || mountedPlayer.Category == mountCategories[base.Count]))
		{
			IncrementCount(1);
		}
		else
		{
			ResetCount();
		}
		m_isMounted = true;
	}

	private void OnDismount(PlayerSkierMounted mountedPlayer, Player newPlayer)
	{
		m_isMounted = false;
		if (mountedPlayer.Collider.OnGround)
		{
			ResetCount();
		}
	}

	private void OnPlayerLand(Player player)
	{
		if (!m_isMounted)
		{
			ResetCount();
		}
	}

	private void OnPlayerSpawn(Player player)
	{
		if (Achievement.CheckMountCategory(player, mountCategories[0]))
		{
			IncrementCount(1);
			m_isMounted = true;
		}
	}

	protected override void OnLoad()
	{
		base.OnLoad();
		m_isMounted = false;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerDismount = (Player.OnDisountDelegate)Delegate.Combine(Player.OnPlayerDismount, new Player.OnDisountDelegate(OnDismount));
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Combine(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerDismount = (Player.OnDisountDelegate)Delegate.Remove(Player.OnPlayerDismount, new Player.OnDisountDelegate(OnDismount));
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Remove(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
	}
}
