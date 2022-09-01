using System.Collections.Generic;
using UnityEngine;

public class PlayerSkierMounted : PlayerSkier
{
	public delegate void OnMountDetachDelegate(PlayerSkierMounted player);

	public string mountCategory = "unknown";

	public PlayerSkier skierPrefab;

	public FlamePowerup.Level ignitionLevelOnRelease;

	public GameObject mounteePrefab;

	public AnimatedSprite mounteeSprite;

	public float minLandSpeedForAnim = 30f;

	public float detachDelay = -1f;

	public int maxJumpsUntilDetach = -1;

	public Sound detachWarningSound;

	public Sound[] jumpFeedbackSounds;

	public float maxReverseDetachSpeed = -1f;

	public GameObject detachEffectPrefab;

	public bool destroyOnDetach;

	public float speedKickOnDetach;

	public Attachment[] mounteeAttachments;

	public static OnMountDetachDelegate OnMountDetach;

	private float m_detachTimer;

	private int m_jumpsUntilDetach;

	private bool m_detachWarningSoundPlayed;

	public override string Category
	{
		get
		{
			return mountCategory;
		}
	}

	protected override void OnJump()
	{
		base.OnJump();
		if (m_jumpsUntilDetach <= 0)
		{
			return;
		}
		m_jumpsUntilDetach--;
		if (m_jumpsUntilDetach == 0)
		{
			m_detachTimer = detachWarningSound.clip.length;
			VOSource.PlayVO(detachWarningSound, true);
			m_detachWarningSoundPlayed = true;
			if (OnMountDetach != null)
			{
				OnMountDetach(this);
			}
		}
		else
		{
			float num = 1f - (float)m_jumpsUntilDetach / (float)maxJumpsUntilDetach;
			int num2 = Mathf.FloorToInt(num * (float)jumpFeedbackSounds.Length);
			VOSource.PlayOneShotVO(jumpFeedbackSounds[num2], true);
		}
	}

	protected override void OnTakeDamage(float speedImpact)
	{
		Player player = Dismount(null, false, false);
		if ((bool)player.VOSource)
		{
			player.VOSource.PlayRandomVO(collisionSounds, true);
		}
		if (Player.OnPlayerTakeDamage != null && PlayerManager.IsHumanPlayer(player))
		{
			Player.OnPlayerTakeDamage(this, player);
		}
	}

	protected override void OnSoftLanding()
	{
		base.OnSoftLanding();
		if ((bool)mounteeSprite)
		{
			mounteeSprite.PlayAnimation("land");
		}
	}

	public override bool PlayAnimation(string animation, bool forceReset)
	{
		bool result = base.PlayAnimation(animation, forceReset);
		if ((bool)mounteeSprite && mounteeSprite.PlayAnimation(animation, forceReset))
		{
			result = true;
		}
		return result;
	}

	protected override Player OnMount(Player player)
	{
		if (destroyOnDetach)
		{
			return player;
		}
		if ((bool)mounteePrefab)
		{
			PlayerInteractionTrigger component = mounteePrefab.GetComponent<PlayerInteractionTrigger>();
			if ((bool)component)
			{
				Player player2 = player.TryFollow(component, base.transform);
				if ((bool)player2)
				{
					return player2;
				}
			}
		}
		if (mounteeAttachments.Length > 0)
		{
			Attachment[] array = mounteeAttachments;
			foreach (Attachment attachment in array)
			{
				if (!player.TryAttach(attachment, false))
				{
					Pool.Spawn(attachment.detachedPrefab, base.transform.position, base.transform.rotation);
				}
			}
		}
		else if ((bool)mounteePrefab)
		{
			Pool.Spawn(mounteePrefab, base.transform.position, base.transform.rotation);
		}
		return player;
	}

	public Player Dismount(PlayerInteractionTrigger trigger, bool allowTransferToAttachments, bool transferIgnition)
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 velocity = base.Skier.Collider.velocity + base.transform.right * speedKickOnDetach;
		FlamePowerup.Level ignitionLevel = ignitionLevelOnRelease;
		if (transferIgnition && IgnitionLevel > ignitionLevel)
		{
			ignitionLevel = IgnitionLevel;
		}
		List<Attachment> attachments = PopAttachments();
		DropAllFollowers();
		if ((bool)detachEffectPrefab)
		{
			Pool.Spawn(detachEffectPrefab, base.transform.position, base.transform.rotation);
		}
		Pool.Despawn(base.gameObject);
		Player player = PlayerManager.SpawnReplacement(this, skierPrefab, position, Quaternion.identity);
		player.Collider.velocity = velocity;
		if (ignitionLevel > FlamePowerup.Level.None)
		{
			player.Ignite(ignitionLevel);
		}
		if ((bool)mounteePrefab && !destroyOnDetach)
		{
			Pool.Spawn(mounteePrefab, position, rotation);
		}
		Player.OnPlayerDismount(this, player);
		if (allowTransferToAttachments)
		{
			player = player.TryMount(attachments, true);
		}
		player.PushAttachments(attachments);
		return player;
	}

	protected override void OnDespawnAsLeader()
	{
		base.OnDespawnAsLeader();
		if ((bool)detachEffectPrefab)
		{
			Pool.Spawn(detachEffectPrefab, base.transform.position, base.transform.rotation);
		}
		if ((bool)mounteePrefab && !destroyOnDetach)
		{
			Pool.Spawn(mounteePrefab, base.transform.position, base.transform.rotation);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_detachTimer = detachDelay;
		m_jumpsUntilDetach = maxJumpsUntilDetach;
		m_detachWarningSoundPlayed = false;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsDisabling)
		{
			return;
		}
		if (maxReverseDetachSpeed >= 0f && Collider.velocity.x < 0f - maxReverseDetachSpeed)
		{
			Dismount(null, true, false);
			return;
		}
		if (maxJumpsUntilDetach >= 0 && base.Skier.Collider.OnGround)
		{
			m_jumpsUntilDetach = maxJumpsUntilDetach;
			m_detachTimer = 0f;
		}
		if (m_detachTimer > 0f)
		{
			m_detachTimer -= Time.deltaTime;
			if (m_detachTimer <= 0f)
			{
				Dismount(null, true, true);
			}
			else if ((bool)detachWarningSound && (bool)detachWarningSound.clip && !m_detachWarningSoundPlayed && (bool)VOSource && m_detachTimer <= detachWarningSound.clip.length)
			{
				VOSource.PlayVO(detachWarningSound, true);
				m_detachWarningSoundPlayed = true;
			}
		}
	}
}
