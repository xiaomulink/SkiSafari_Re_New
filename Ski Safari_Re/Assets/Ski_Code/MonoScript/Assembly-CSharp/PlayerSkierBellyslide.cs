using UnityEngine;

public class PlayerSkierBellyslide : PlayerSkier
{
	public delegate void OnPlayerGetUpDelegate(PlayerSkierBellyslide previousPlayer, Player player);

	public PlayerSkier skierPrefab;

	public float initialTangentKickSpeed = 20f;

	public float initialKickTimeout = 0.25f;

	public float minTumbleTime = 1f;

	public float minTumbleSpeed = 10f;

	public float detachDelay = -1f;

	public float minSpeedForDetach = -1f;

	public float detachKickForwardSpeed = 20f;

	public float detachKickUpSpeed = 10f;

	public float detachDecayPerTap = 0.1f;

	public float delayBetweenTaps = 0.2f;

	public float getUpDuration = 1f;

	public static OnPlayerGetUpDelegate OnPlayerGetUp;

	private bool m_initialKickGiven;

	private float m_initialKickTimer;

	private float m_tumbleTimer;

	private float m_detachTimer;

	private float m_tapTimer;

	private bool m_tapping;

	private float m_getUpTimer;

	public override string Category
	{
		get
		{
			return "bellyslide";
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
			if (!(m_getUpTimer <= 0f))
			{
				return;
			}
			if (value == 1f)
			{
				if (m_tapping)
				{
					return;
				}
				m_tapping = true;
				if (m_tapTimer <= 0f)
				{
					m_detachTimer -= detachDecayPerTap;
					m_tapTimer = delayBetweenTaps;
					if (m_detachTimer <= 0f)
					{
						GetUp();
					}
					else if (m_tumbleTimer <= 0f)
					{
						sprite.PlayAnimation("tap", true);
						sprite.QueueAnimation("bellyslide");
					}
				}
			}
			else
			{
				m_tapping = false;
			}
		}
	}

	public bool IsTumbling
	{
		get
		{
			return m_tumbleTimer > 0f;
		}
	}

	public override bool Ignite(FlamePowerup.Level minIgnitionLevel)
	{
		base.Ignite(minIgnitionLevel);
		FinishGettingUp();
		return true;
	}

	protected override void OnTakeDamage(float speedImpact)
	{
	}

	public override bool CanMount(PlayerInteractionTrigger trigger, bool forceInstantRemount)
	{
		return trigger.mountConfig.instantMount;
	}

	public override bool PlayAnimation(string animation, bool forceReset)
	{
		return true;
	}

	private void GetUp()
	{
		sprite.PlayAnimation("getup");
		m_getUpTimer = getUpDuration;
	}

	private void FinishGettingUp()
	{
		if (base.enabled)
		{
			Vector3 lookAtPos = LookAtPos;
			Vector3 velocity = detachKickForwardSpeed * base.transform.right + detachKickUpSpeed * base.transform.up;
			Vector3 normalized = velocity.normalized;
			Quaternion rotation = Quaternion.LookRotation(upwards: new Vector3(0f - normalized.y, normalized.x, 0f), forward: Vector3.forward);
			Vector3 position = base.transform.position + new Vector3(0f, 0.1f, 0f);
			velocity += Collider.velocity;
			FlamePowerup.Level ignitionLevel = IgnitionLevel;
			Pool.Despawn(base.gameObject);
			PlayerSkier playerSkier = PlayerManager.SpawnReplacement(this, skierPrefab, position, rotation);
			playerSkier.Skier.Collider.velocity = velocity;
			playerSkier.LiftInput = 1f;
			if (ignitionLevel > FlamePowerup.Level.None)
			{
				playerSkier.Ignite(ignitionLevel);
			}
			if (OnPlayerGetUp != null && PlayerManager.IsHumanPlayer(playerSkier))
			{
				OnPlayerGetUp(this, playerSkier);
			}
			FollowCamera.Instance.AddPlayerChangeOffset(playerSkier.LookAtPos - lookAtPos);
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_initialKickGiven = false;
		m_initialKickTimer = initialKickTimeout;
		m_tumbleTimer = minTumbleTime;
		m_detachTimer = detachDelay;
		m_tapTimer = 0f;
		m_tapping = false;
		m_getUpTimer = 0f;
	}

	protected override void FixedUpdate()
	{
        transform.Find("SkierSprite").gameObject.layer = 0;
		base.FixedUpdate();
		if (base.IsDisabling)
		{
			return;
		}
		if (!m_initialKickGiven && m_initialKickTimer > 0f)
		{
			if (Collider.OnGround)
			{
				Vector3 normal = Collider.GroundContactInfo.normal;
				Vector3 vector = new Vector3(normal.y, 0f - normal.x, 0f);
				Collider.velocity += initialTangentKickSpeed * vector;
				m_initialKickGiven = true;
			}
			else
			{
				m_initialKickTimer -= Time.deltaTime;
			}
		}
		if (m_tapTimer > 0f)
		{
			m_tapTimer -= Time.deltaTime;
		}
		if (m_getUpTimer > 0f)
		{
			m_getUpTimer -= Time.deltaTime;
			if (m_getUpTimer <= 0f)
			{
				FinishGettingUp();
			}
		}
		else
		{
			if (!Collider.OnGround)
			{
				return;
			}
			if (m_tumbleTimer > 0f)
			{
				m_tumbleTimer -= Time.deltaTime;
				if (m_tumbleTimer <= 0f || Collider.velocity.magnitude < minTumbleSpeed)
				{
					m_tumbleTimer = 0f;
					sprite.QueueAnimation("bellyslide");
				}
				else
				{
					m_tumbleTimer = Mathf.Max(m_tumbleTimer, Mathf.Epsilon);
				}
			}
			if (m_detachTimer > 0f)
			{
				m_detachTimer -= Time.deltaTime;
				if (m_detachTimer <= 0f)
				{
					GetUp();
				}
			}
			else if (minSpeedForDetach >= 0f && Collider.velocity.magnitude <= minSpeedForDetach)
			{
				GetUp();
			}
		}
	}
}
