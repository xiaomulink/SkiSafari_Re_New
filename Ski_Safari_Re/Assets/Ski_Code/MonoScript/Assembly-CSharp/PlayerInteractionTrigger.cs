using System;
using UnityEngine;

public class PlayerInteractionTrigger : PlayerTrigger
{
	[Serializable]
	public class MountConfig
	{
		public Player prefab;

		public float initialDelay = 1f;

		public Vector3 spawnDirectionOverride;

		public Vector3 velocityOverride;

		public bool usePlayerPosition = true;

		public bool usePlayerRotation = true;

		public bool startIgnited;

		public bool disableOnMount;

		public bool instantMount;

		public Attachment playerAttachment;

		public int numInitialRandomAttachments;

		public Sound[] sounds;
	}

	[Serializable]
	public class FollowConfig
	{
		public Player[] leaderPrefabs;

		public Sound followSound;
	}

	[Serializable]
	public class AttachConfig
	{
		public Attachment prefab;
	}

	[Serializable]
	public class TumbleConfig
	{
		public GameObject prefab;

		public string[] categories;

		public float initialDelay = 1f;

		public Vector3 kickVelocity;

		public Sound sound;

		public GameObject effect;
	}

	[Serializable]
	public class PassConfig
	{
		public float initialDelay = 1f;

		public GameObject effect;

		public bool disableOnPass;
	}

	public delegate void OnTumbleDelegate(PlayerInteractionTrigger trigger, Player sourcePlayer);

	public string category = "unknown";

	public MountConfig mountConfig = new MountConfig();

	public FollowConfig followConfig = new FollowConfig();

	public AttachConfig attachConfig = new AttachConfig();

	public TumbleConfig tumbleConfig = new TumbleConfig();

	public PassConfig passConfig = new PassConfig();

	public bool disableOnTrigger;

	public AvalancheDestroyable destroyableOwner;

	public static OnTumbleDelegate OnTumble;

	private CircleCollider m_collider;

	private float m_startTime;

	protected override PlayerManager.PlayerGroup PlayerGroup
	{
		get
		{
			return PlayerManager.PlayerGroup.Human;
		}
	}

	public CircleCollider Collider
	{
		get
		{
			return m_collider;
		}
	}

	public bool IsMountable(Player current)
	{
		if (!base.enabled)
		{
			return false;
		}
		if (Time.time - m_startTime < mountConfig.initialDelay)
		{
			return false;
		}
		return true;
	}

	public Player Mount(Player current, bool sendEvents)
	{
		Player player = current.Mount(rotation: (mountConfig.spawnDirectionOverride != Vector3.zero) ? Quaternion.LookRotation(Vector3.forward, Vector3.Cross(mountConfig.spawnDirectionOverride, Vector3.back).normalized) : ((!mountConfig.usePlayerRotation) ? base.transform.rotation : current.transform.rotation), skierPrefab: mountConfig.prefab, position: (!mountConfig.usePlayerPosition) ? base.transform.position : current.transform.position, startIgnited: mountConfig.startIgnited, velocityOverride: mountConfig.velocityOverride, sourceCollider: Collider, usePlayerPosition: mountConfig.usePlayerPosition, sendEvents: sendEvents);
		Pool.Despawn(current.gameObject);
		Disable();
		if ((bool)player.VOSource)
		{
			player.VOSource.PlayRandomVO(mountConfig.sounds);
		}
		if ((bool)mountConfig.playerAttachment)
		{
			player.TryAttach(mountConfig.playerAttachment, true);
			PlayerManager.ReplacePlayer(GetComponent<Player>(), mountConfig.playerAttachment);
		}
		if (mountConfig.numInitialRandomAttachments > 0 && Slope.Instance.randomAttachments.Length > 0)
		{
			for (int i = 0; i < mountConfig.numInitialRandomAttachments; i++)
			{
				Attachment attachment = Slope.Instance.randomAttachments[UnityEngine.Random.Range(0, Slope.Instance.randomAttachments.Length)];
				player.TryAttach(attachment, sendEvents);
			}
		}
		return player;
	}

	private bool CausesTumble(Player player)
	{
		if (Time.time - m_startTime < tumbleConfig.initialDelay)
		{
			return false;
		}
		string[] categories = tumbleConfig.categories;
		foreach (string text in categories)
		{
			if (player.Category == text)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnPlayerTrigger(Player player)
	{
		if ((bool)player.TryMount(this, true) || !base.enabled)
		{
			return;
		}
		if ((bool)attachConfig.prefab && (bool)player.TryAttach(attachConfig.prefab, true))
		{
			if (attachConfig.prefab.isPlayer)
			{
				PlayerManager.ReplacePlayer(GetComponent<Player>(), attachConfig.prefab);
			}
			Disable();
		}
		else if ((bool)player.TryFollow(this, base.transform))
		{
			Disable();
		}
		else if (CausesTumble(player))
		{
			GameObject gameObject = base.gameObject;
			if ((bool)tumbleConfig.prefab)
			{
				gameObject = Pool.Spawn(tumbleConfig.prefab, base.transform.position, base.transform.rotation);
				Pool.Despawn(base.gameObject);
			}
			CircleCollider component = gameObject.GetComponent<CircleCollider>();
			if ((bool)component)
			{
				component.velocity = tumbleConfig.kickVelocity;
			}
			if ((bool)tumbleConfig.sound)
			{
				VOSource component2 = gameObject.GetComponent<VOSource>();
				if ((bool)component2)
				{
					component2.PlayOneShotVO(tumbleConfig.sound, true);
				}
			}
			if ((bool)tumbleConfig.effect)
			{
				Pool.Spawn(tumbleConfig.effect, base.transform.position, base.transform.rotation);
			}
			if (OnTumble != null)
			{
				OnTumble(this, player);
			}
		}
		else if (Time.time - m_startTime >= passConfig.initialDelay)
		{
			if ((bool)passConfig.effect)
			{
				Pool.Spawn(passConfig.effect, base.transform.position, base.transform.rotation);
			}
			if (passConfig.disableOnPass)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	private void Disable()
	{
		if (disableOnTrigger)
		{
			base.gameObject.SetActive(false);
		}
		else if ((bool)destroyableOwner)
		{
			destroyableOwner.Destroy();
		}
		else
		{
			Pool.Despawn(base.gameObject);
		}
	}

	private void Start()
	{
		m_collider = GetComponent<CircleCollider>();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_startTime = Time.time;
	}
}
