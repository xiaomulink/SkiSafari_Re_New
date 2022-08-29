using System.Collections.Generic;
using UnityEngine;

public class PlayerSkierFrozen : PlayerSkier
{
	public string mountCategory = "unknown";

	public PlayerSkier skierPrefab;

	public float freezeTime = 10f;

	public float freezeDecayPerTap = 0.1f;

	public float delayBetweenTaps = 0.2f;

	public GameObject breakOutEffect;

	public float breakOutKickForwardSpeed = 20f;

	public float breakOutKickUpSpeed = 10f;

	public FlamePowerup.Level breakOutIgnitionLevel = FlamePowerup.Level.Yellow;

	public Renderer cracksSprite;

	public Sound[] crackSounds;

	public ParticleSystem crackParticles;

	public static SimplePlayerDelegate OnPlayerBreakOut;

	private float m_freezeTimer;

	private float m_tapTimer;

	private bool m_tapping;

	public override string Category
	{
		get
		{
			return mountCategory;
		}
	}

	public override float LiftInput
	{
		get
		{
			return base.LiftInput;
		}
		set
		{
			base.LiftInput = value;
			if (value == 1f)
			{
				if (m_tapping)
				{
					return;
				}
				m_tapping = true;
				if (m_tapTimer <= 0f)
				{
					m_freezeTimer -= freezeDecayPerTap;
					m_tapTimer = delayBetweenTaps;
					if (m_freezeTimer <= 0f)
					{
						BreakOut();
						return;
					}
					sprite.PlayAnimation("tap");
					sprite.QueueAnimation("frozen");
					SoundManager.Instance.PlayRandomSound(crackSounds);
					crackParticles.Emit(1000);
				}
			}
			else
			{
				m_tapping = false;
			}
		}
	}

	public override bool Ignite(FlamePowerup.Level minIgnitionLevel)
	{
		base.Ignite(minIgnitionLevel);
		BreakOut();
		return true;
	}

	protected override void OnTakeDamage(float speedImpact)
	{
		BreakOut();
	}

	public override bool CanMount(PlayerInteractionTrigger trigger, bool forceInstantRemount)
	{
		return false;
	}

	public override bool PlayAnimation(string animation, bool forceReset)
	{
		return true;
	}

	private void BreakOut()
	{
		if (base.enabled)
		{
			List<Attachment> attachments = PopAttachments();
			Vector3 velocity = breakOutKickForwardSpeed * base.transform.right + breakOutKickUpSpeed * base.transform.up;
			Vector3 normalized = velocity.normalized;
			Quaternion rotation = Quaternion.LookRotation(upwards: new Vector3(0f - normalized.y, normalized.x, 0f), forward: Vector3.forward);
			Vector3 position = base.transform.position;
			velocity += Collider.velocity;
			Pool.Despawn(base.gameObject);
			PlayerSkier playerSkier = PlayerManager.SpawnReplacement(this, skierPrefab, position, rotation);
			playerSkier.Skier.Collider.velocity = velocity;
			playerSkier.LiftInput = 1f;
			if (breakOutIgnitionLevel > FlamePowerup.Level.None)
			{
				playerSkier.Ignite(breakOutIgnitionLevel);
			}
			playerSkier.PushAttachments(attachments);
			Object.Instantiate(breakOutEffect, position, rotation);
			if (OnPlayerBreakOut != null && PlayerManager.IsHumanPlayer(playerSkier))
			{
				OnPlayerBreakOut(playerSkier);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_freezeTimer = freezeTime;
		m_tapTimer = 0f;
		m_tapping = false;
		cracksSprite.material.SetFloat("_Cutoff", 1f);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsDisabling)
		{
			return;
		}
		if (m_tapTimer > 0f)
		{
			m_tapTimer -= Time.deltaTime;
		}
		cracksSprite.material.SetFloat("_Cutoff", m_freezeTimer / freezeTime);
		if (m_freezeTimer > 0f)
		{
			m_freezeTimer -= Time.deltaTime;
			if (m_freezeTimer <= 0f)
			{
				BreakOut();
			}
		}
	}
}
