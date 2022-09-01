using System;
using UnityEngine;

public class Stunt_Glide : Stunt
{
	[Serializable]
	public class GlideLevel
	{
		public float minGlideDuration = 3f;

		public string description = "Re-entry!";

		public float score = 200f;
	}

	[Serializable]
	public class MountDescriptor
	{
		public string mountCategory;

		public GlideLevel[] glideLevels;
	}

	public delegate void OnGlideIgniteDelegate(Player player, FlamePowerup.Level level);

	public MountDescriptor[] mountDescriptors;

	public MountDescriptor defaultDescriptor;

	public float glideMissToleranceDuration = 0.25f;

	public static OnGlideIgniteDelegate OnGlideIgnite;

	private float m_glideTimer;

	private float m_glideMissTimer;

	private MountDescriptor m_currentMountDescriptor;

	private Player m_currentPlayer;

	private MountDescriptor FindMountDescriptor(Player player)
	{
		PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
		if ((bool)playerSkierMounted)
		{
			MountDescriptor[] array = mountDescriptors;
			foreach (MountDescriptor mountDescriptor in array)
			{
				if (playerSkierMounted.mountCategory == mountDescriptor.mountCategory)
				{
					return mountDescriptor;
				}
			}
		}
		return defaultDescriptor;
	}

	private void ResetGlide(Player player)
	{
		m_glideMissTimer = glideMissToleranceDuration;
		m_glideTimer = 0f;
		player.PreIgnite(0f);
	}

	protected void OnLand(Player player)
	{
		ResetGlide(player);
	}

	protected void OnTakeDamage(Player previousPlayer, Player player)
	{
		ResetGlide(player);
	}

	protected void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		if (!(mountedPlayer.Category == previousPlayer.Category))
		{
			ResetGlide(mountedPlayer);
		}
	}

	protected void OnDismount(PlayerSkierMounted mountedPlayer, Player player)
	{
		ResetGlide(player);
	}

	protected void OnJump(Skier skier)
	{
		PlayerSkier playerSkier = Player.Instance as PlayerSkier;
		if ((bool)playerSkier && playerSkier.Skier == skier)
		{
			m_glideMissTimer = glideMissToleranceDuration;
			m_glideTimer = 0f;
			playerSkier.PreIgnite(0f);
		}
	}

	protected void OnIgnoreFreeze(Player player)
	{
		ResetGlide(player);
		player.Douse();
	}

	protected void FixedUpdate()
	{
		Player instance = Player.Instance;
		if ((bool)instance && instance.IsGliding)
		{
			m_glideTimer += Time.deltaTime;
			m_glideMissTimer = 0f;
			if (instance != m_currentPlayer)
			{
				m_currentPlayer = instance;
				m_currentMountDescriptor = FindMountDescriptor(m_currentPlayer);
			}
			FlamePowerup.Level ignitionLevel = m_currentPlayer.IgnitionLevel;
			if ((int)ignitionLevel >= m_currentMountDescriptor.glideLevels.Length || !m_currentPlayer.CanIgniteToLevel(ignitionLevel + 1))
			{
				return;
			}
			float minGlideDuration = m_currentMountDescriptor.glideLevels[(int)ignitionLevel].minGlideDuration;
			if (m_glideTimer >= minGlideDuration)
			{
				GlideLevel glideLevel = m_currentMountDescriptor.glideLevels[(int)ignitionLevel];
				if (glideLevel.score > 0f)
				{
					base.Manager.AddScore(glideLevel.score, glideLevel.description);
				}
				if (m_currentPlayer.Ignite())
				{
					m_glideTimer -= minGlideDuration;
					if (OnGlideIgnite != null)
					{
						OnGlideIgnite(instance, ignitionLevel);
					}
				}
			}
			else
			{
				instance.PreIgnite(Mathf.Clamp01(m_glideTimer / minGlideDuration));
			}
		}
		else
		{
			if (!(m_glideMissTimer < glideMissToleranceDuration))
			{
				return;
			}
			m_glideMissTimer += Time.deltaTime;
			if (m_glideMissTimer > glideMissToleranceDuration)
			{
				m_glideTimer = 0f;
				if ((bool)instance)
				{
					instance.PreIgnite(0f);
				}
			}
		}
	}

	protected override void OnEnable()
	{
		Player.OnPlayerLand = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerLand, new Player.SimplePlayerDelegate(OnLand));
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Combine(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnTakeDamage));
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerDismount = (Player.OnDisountDelegate)Delegate.Combine(Player.OnPlayerDismount, new Player.OnDisountDelegate(OnDismount));
		Player.OnPlayerIgnoreFreeze = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerIgnoreFreeze, new Player.SimplePlayerDelegate(OnIgnoreFreeze));
		Skier.OnSkierJump = (Skier.OnSkierJumpDelegate)Delegate.Combine(Skier.OnSkierJump, new Skier.OnSkierJumpDelegate(OnJump));
	}

	protected override void OnDisable()
	{
		Player.OnPlayerLand = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerLand, new Player.SimplePlayerDelegate(OnLand));
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Remove(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnTakeDamage));
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		Player.OnPlayerDismount = (Player.OnDisountDelegate)Delegate.Remove(Player.OnPlayerDismount, new Player.OnDisountDelegate(OnDismount));
		Player.OnPlayerIgnoreFreeze = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerIgnoreFreeze, new Player.SimplePlayerDelegate(OnIgnoreFreeze));
		Skier.OnSkierJump = (Skier.OnSkierJumpDelegate)Delegate.Remove(Skier.OnSkierJump, new Skier.OnSkierJumpDelegate(OnJump));
	}
}
