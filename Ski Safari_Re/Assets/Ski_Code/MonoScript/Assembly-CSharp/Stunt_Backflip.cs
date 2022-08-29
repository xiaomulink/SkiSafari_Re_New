using System;

public class Stunt_Backflip : Stunt
{
	[Serializable]
	public class MountDescriptor
	{
		public string mountCategory;

		public string[] descriptions;

		public string multiDescription = "Backflips!";

		public float score = 50f;
	}

	public delegate void OnBackflipLandedDelegate(Player player, int consecutiveCount);

	public MountDescriptor[] mountDescriptors;

	public MountDescriptor defaultDescriptor;

	public static OnBackflipLandedDelegate OnBackflipLanded;

	public static Player.SimplePlayerDelegate OnBackflipCancelled;

	private int m_currentCount;

	protected void OnLand(Player player)
	{
		CheckBackflip(player);
	}

	protected void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		CheckBackflip(previousPlayer);
	}

	protected void OnTakeDamage(Player previousPlayer, Player player)
	{
		if (OnBackflipCancelled != null)
		{
			OnBackflipCancelled(player);
		}
		m_currentCount = 0;
	}

	protected void OnBackflip(Player player)
	{
		if (!(player is PlayerSkierFrozen))
		{
			m_currentCount++;
		}
	}

	private void CheckBackflip(Player player)
	{
		if (m_currentCount > 0)
		{
			MountDescriptor mountDescriptor = defaultDescriptor;
			PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
			if ((bool)playerSkierMounted)
			{
				MountDescriptor[] array = mountDescriptors;
				foreach (MountDescriptor mountDescriptor2 in array)
				{
					if (playerSkierMounted.mountCategory == mountDescriptor2.mountCategory)
					{
						mountDescriptor = mountDescriptor2;
						break;
					}
				}
			}
			int num = m_currentCount - 1;
			string description = ((num >= mountDescriptor.descriptions.Length) ? string.Format("{0} {1}", m_currentCount, mountDescriptor.multiDescription) : mountDescriptor.descriptions[num]);
			base.Manager.AddScore(mountDescriptor.score * (float)m_currentCount, description, m_currentCount);
			FlamePowerup.Level level = FlamePowerup.Level.None;
			switch (m_currentCount)
			{
			case 1:
				level = FlamePowerup.Level.Yellow;
				break;
			case 2:
				level = FlamePowerup.Level.Red;
				break;
			default:
				level = FlamePowerup.Level.Blue;
				break;
			}
			player.Ignite(level);
			if (OnBackflipLanded != null)
			{
				OnBackflipLanded(player, m_currentCount);
			}
		}
		else if (OnBackflipCancelled != null)
		{
			OnBackflipCancelled(player);
		}
		m_currentCount = 0;
	}

	protected override void OnEnable()
	{
		Player.OnPlayerLand = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerLand, new Player.SimplePlayerDelegate(OnLand));
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Combine(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnTakeDamage));
		Player.OnPlayerBackflip = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerBackflip, new Player.SimplePlayerDelegate(OnBackflip));
	}

	protected override void OnDisable()
	{
		Player.OnPlayerLand = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerLand, new Player.SimplePlayerDelegate(OnLand));
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Remove(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnTakeDamage));
		Player.OnPlayerBackflip = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerBackflip, new Player.SimplePlayerDelegate(OnBackflip));
	}

	public override void OnReset()
	{
		m_currentCount = 0;
	}
}
