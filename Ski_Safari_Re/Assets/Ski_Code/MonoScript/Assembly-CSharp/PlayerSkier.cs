using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkier : Player
{
	private class State
	{
		public float airTimer;

		public float groundTimer;

		public float glideTrailTimer;

		public float attachTimer;

		public bool wasOnGround = true;

		public bool flippedInAir;

		public bool disabling;

		public List<Follower> followers = new List<Follower>();

		public int positionHistoryIndex;

		public int positionHistoryCount;
	}

	public new static PlayerSkier Instance;

	public AnimatedSprite sprite;

	public bool useAttachmentsAsShields = true;

	public FXAirEffect airEffectPrefab;

	public float glideTrailTransitionDuration = 0.25f;

	public float maxTimeOffGroundForTrail = 0.25f;

	public FXBackflipEffect backflipEffectPrefab;

	public ParticleSystem invulnerabilityGlowPrefab;

	public float invulnerabilityGlowFadeDuration = 0.5f;

	public float maxSpeedForPush = 20f;

	public float pushSpeed;

	public float maxSpeedForAirPush = 20f;

	public float airPushSpeed;

	public Sound pushSound;

	public Sound pushSound_new;

	public Player collisionSkierPrefab;

	public GameObject collisionEffectPrefab;

	public bool takeDamageOnCollision = true;

	public float collisionKickSpeed;

	public float collisionSkierKickAngle = 30f;

	public Sound[] collisionSounds;

	public GameObject hardLandingEffect;

	public float hardLandingAngle = 135f;

	public float hardLandingSpeed = 20f;

	public float hardLandingSpeedReduction = 20f;

	public bool alwaysDismountOnHardLanding;

	public PlayerPiledrive piledrivePlayerPrefab;

	public Sound piledriveSound;

	public float piledriveAngle = 150f;

	public float piledriveSpeed = 20f;

	public PlayerSkierFrozen frozenPlayerPrefab;

	public PlayerSnowball snowballPlayerPrefab;

	public float softLandingSpeed = 10f;

	public static float skierLateralDragRatio = 1f;

	public static float skierGroundSuctionRatio = 1f;

	private Skier m_skier;

	private VOSource m_voSource;

	private FXAirEffect m_airEffect;

	private FXBackflipEffect m_backflipEffect;

	private ParticleSystem m_invulnerabilityGlow;

	private State m_state;

	public Skier Skier
	{
		get
		{
			return m_skier;
		}
	}

	public override string Category
	{
		get
		{
			return "skier";
		}
	}

	public override VOSource VOSource
	{
		get
		{
			return m_voSource;
		}
	}

	public override bool IsGliding
	{
		get
		{
			return m_skier.IsGliding;
		}
	}

	public override float LiftInput
	{
		get
		{
			return m_skier.LiftInput;
		}
		set
		{
			m_skier.LiftInput = value;
		}
	}

	protected override void OnCollision(GeometryUtils.ContactInfo contactInfo)
	{
		if (base.IsDisabling)
		{
			return;
		}
		if (base.InvulnerabilityTimer <= 0f)
		{
			float num = Vector3.Dot(contactInfo.normal, Vector3.up);
			if (num > 0f)
			{
				float num2 = Vector3.Angle(contactInfo.normal, base.transform.up);
				if (!contactInfo.collider && num2 > piledriveAngle && 0f - contactInfo.normalSpeed > piledriveSpeed)
				{
					if (!contactInfo.collider || !(contactInfo.collider.category == "cloud"))
					{
						Quaternion rotation = Quaternion.LookRotation(Vector3.forward, contactInfo.normal);
						Pool.Spawn(hardLandingEffect, base.transform.position, rotation);
						TakeDamage(hardLandingSpeedReduction, (DamageFlags)6);
					}
					return;
				}
				if (num2 > hardLandingAngle || (IgnitionLevel != FlamePowerup.Level.SuperBlue && 0f - contactInfo.normalSpeed > hardLandingSpeed))
				{
					if ((!contactInfo.collider || !(contactInfo.collider.category == "cloud")) && !TryLoseLeader())
					{
						if (Player.OnPlayerHardLanding != null && PlayerManager.IsHumanPlayer(this))
						{
							Player.OnPlayerHardLanding(this);
						}
						Quaternion rotation2 = Quaternion.LookRotation(Vector3.forward, contactInfo.normal);
						Pool.Spawn(hardLandingEffect, base.transform.position, rotation2);
						TakeDamage(hardLandingSpeedReduction, DamageFlags.IgnoreInvulnerability);
					}
					return;
				}
			}
		}
		if (0f - contactInfo.normalSpeed > softLandingSpeed)
		{
			OnSoftLanding();
		}
		if (m_state.flippedInAir)
		{
			m_state.flippedInAir = false;
			if (Player.OnPlayerBackflip != null && PlayerManager.IsHumanPlayer(this))
			{
				Player.OnPlayerBackflip(this);
			}
		}
		base.OnCollision(contactInfo);
	}

	protected virtual void OnJump()
	{
		if (!PlayAnimation("jump", true))
		{
			PlayAnimation("glide", false);
		}
	}

	public virtual bool PlayAnimation(string animation, bool forceReset)
	{
		bool result = sprite.PlayAnimation(animation, forceReset);
		foreach (Follower follower in base.Followers)
		{
			if (follower.sprite.PlayAnimation(animation, forceReset))
			{
				result = true;
			}
		}
		AttachNode[] array = attachNodes;
		foreach (AttachNode attachNode in array)
		{
			if ((bool)attachNode.Attachment && attachNode.Attachment.sprite.PlayAnimation(animation, forceReset))
			{
				result = true;
			}
		}
		return result;
	}

	public override void TakeDamage(float speedImpact, DamageFlags damageFlags)
	{
		if (!base.IsDisabling && (!IsInvulnerable || (damageFlags & DamageFlags.IgnoreInvulnerability) != 0) && ((damageFlags & DamageFlags.HitHazard) == 0 || !useAttachmentsAsShields || (!TryLoseLeader() && !DetachFirstPassenger())) && ((damageFlags & DamageFlags.Piledrive) == 0 || !HandlePiledriveCollision()) && !HandleDamageCollision(speedImpact, damageFlags))
		{
			OnTakeDamage(speedImpact);
		}
	}

	protected override bool OnFreeze()
	{
		if (IsInvulnerable || !frozenPlayerPrefab)
		{
			if (Player.OnPlayerIgnoreFreeze != null && PlayerManager.IsHumanPlayer(this))
			{
				Player.OnPlayerIgnoreFreeze(this);
			}
			return false;
		}
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 velocity = Collider.velocity;
		float radius = Collider.radius;
		Vector3 vector = ((!Collider.OnGround) ? Vector3.up : Collider.GroundContactInfo.normal);
		Vector3 vector2 = vector * (frozenPlayerPrefab.GetComponent<CircleCollider>().radius - radius);
		position += vector2;
		List<Attachment> attachments = PopAttachments();
		Pool.Despawn(base.gameObject);
		PlayerSkierFrozen playerSkierFrozen = PlayerManager.SpawnReplacement(this, frozenPlayerPrefab, position, rotation);
		playerSkierFrozen.Collider.velocity = velocity;
		playerSkierFrozen.PushAttachments(attachments);
		if (PlayerManager.IsHumanPlayer(playerSkierFrozen))
		{
			if (Player.OnPlayerTakeDamage != null)
			{
				Player.OnPlayerTakeDamage(this, playerSkierFrozen);
			}
			if (Player.OnPlayerFreeze != null)
			{
				Player.OnPlayerFreeze(this, playerSkierFrozen);
			}
		}
		return true;
	}

	protected override bool OnSnowball(SnowballTrigger snowballTrigger)
	{
		if (IsInvulnerable || !snowballPlayerPrefab)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		Vector3 velocity = Collider.velocity;
		float radius = Collider.radius;
		Vector3 vector = ((!Collider.OnGround) ? Vector3.up : Collider.GroundContactInfo.normal);
		Vector3 vector2 = vector * (snowballPlayerPrefab.GetComponent<CircleCollider>().radius - radius);
		position += vector2;
		DetachAll();
		Pool.Despawn(base.gameObject);
		PlayerSnowball playerSnowball = PlayerManager.SpawnReplacement(this, snowballPlayerPrefab, position, rotation);
		playerSnowball.Collider.velocity = velocity;
		snowballTrigger.OnHitByPlayer(playerSnowball);
		if (PlayerManager.IsHumanPlayer(playerSnowball))
		{
			if (Player.OnPlayerTakeDamage != null)
			{
				Player.OnPlayerTakeDamage(this, playerSnowball);
			}
			if (Player.OnPlayerSnowball != null)
			{
				Player.OnPlayerSnowball(this, playerSnowball);
			}
		}
		return true;
	}

	protected virtual void OnTakeDamage(float speedImpact)
	{
		float magnitude = m_skier.Collider.velocity.magnitude;
		if (magnitude > Mathf.Epsilon)
		{
			Vector3 vector = m_skier.Collider.velocity / magnitude;
			m_skier.Collider.velocity -= Mathf.Min(magnitude, speedImpact) * vector;
		}
		if ((bool)VOSource)
		{
			VOSource.PlayRandomVO(collisionSounds, true);
		}
		if (Player.OnPlayerTakeDamage != null && PlayerManager.IsHumanPlayer(this))
		{
			Player.OnPlayerTakeDamage(this, this);
		}
	}

	protected virtual void OnSoftLanding()
	{
	}

	protected virtual bool HandleDamageCollision(float speedImpact, DamageFlags damageFlags)
	{
		if ((bool)collisionSkierPrefab && (!alwaysDismountOnHardLanding || (damageFlags & DamageFlags.HitHazard) != 0))
		{
			float num = m_skier.Collider.velocity.magnitude + collisionKickSpeed;
			Vector3 vector = ((!(num > 0f)) ? base.transform.right : (m_skier.Collider.velocity / num));
			Vector3 upwards = new Vector3(0f - vector.y, vector.x, 0f);
			Quaternion quaternion = Quaternion.AngleAxis(collisionSkierKickAngle, Vector3.forward);
			vector = quaternion * vector;
			num = Mathf.Max(0f, num - speedImpact);
			Vector3 position = base.transform.position;
			Quaternion rotation = Quaternion.LookRotation(Vector3.forward, upwards);
			List<Attachment> attachments = PopAttachments();
			Pool.Despawn(base.gameObject);
			Player player = PlayerManager.SpawnReplacement(this, collisionSkierPrefab, position, rotation);
			player.Collider.velocity = vector * num;
			player.PushAttachments(attachments);
			if ((bool)player.VOSource)
			{
				player.VOSource.PlayRandomVO(collisionSounds, true);
			}
			if ((bool)collisionEffectPrefab)
			{
				Pool.Spawn(collisionEffectPrefab, position, rotation);
			}
			if (takeDamageOnCollision && Player.OnPlayerTakeDamage != null)
			{
				Player.OnPlayerTakeDamage(this, player);
			}
			return true;
		}
		return false;
	}

	protected virtual bool HandlePiledriveCollision()
	{
		if ((bool)piledrivePlayerPrefab)
		{
			Vector3 position = base.transform.position;
			Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Collider.GroundContactInfo.normal);
			DetachAll();
			Pool.Despawn(base.gameObject);
			PlayerPiledrive playerPiledrive = PlayerManager.SpawnReplacement(this, piledrivePlayerPrefab, position, rotation);
			if ((bool)piledriveSound && SoundManager.Instance.SFXEnabled)
			{
				playerPiledrive.GetComponent<AudioSource>().loop = false;
				playerPiledrive.GetComponent<AudioSource>().PlayOneShot(piledriveSound.clip);
			}
			if (Player.OnPlayerTakeDamage != null && PlayerManager.IsHumanPlayer(playerPiledrive))
			{
				Player.OnPlayerTakeDamage(this, playerPiledrive);
			}
			return true;
		}
		return false;
	}

	private void UpdateInAir()
	{
		if (sprite.CurrentAnimation != "tumble")
		{
			if (m_skier.IsRotatingFromInput)
			{
				PlayAnimation("tuck", false);
			}
			else if (LiftInput < 1f)
			{
				PlayAnimation("glide", false);
			}
		}
		if (airPushSpeed > 0f)
		{
			CircleCollider collider = m_skier.Collider;
			if (collider.velocity.magnitude < maxSpeedForAirPush && (sprite.CurrentAnimation == "idle" || string.IsNullOrEmpty(sprite.CurrentAnimation)))
			{
				PlayAnimation("push", false);
				collider.velocity += base.transform.right * airPushSpeed;
			}
		}
		float z = base.transform.eulerAngles.z;
		if (z > 180f && z < 225f)
		{
			m_state.flippedInAir = true;
		}
		else if (m_state.flippedInAir && z >= 270f && z < 360f)
		{
			m_state.flippedInAir = false;
			if (Player.OnPlayerBackflip != null && PlayerManager.IsHumanPlayer(this))
			{
				Player.OnPlayerBackflip(this);
			}
		}
		if (m_state.wasOnGround)
		{
			if (Player.OnPlayerTakeoff != null && PlayerManager.IsHumanPlayer(this))
			{
				Player.OnPlayerTakeoff(this);
			}
			m_state.wasOnGround = false;
		}
		m_state.airTimer += Time.fixedDeltaTime;
		m_state.groundTimer = 0f;
	}

	private void UpdateOnGround()
	{
		if (sprite.CurrentAnimation == "glide" || sprite.CurrentAnimation == "tuck")
		{
			PlayAnimation("idle", false);
		}
		if (pushSpeed > 0f && sprite.CurrentAnimation == "idle")
		{
			CircleCollider collider = m_skier.Collider;
			if (Vector3.Dot(collider.velocity, base.transform.right) < maxSpeedForPush && Vector3.Dot(base.transform.up, Vector3.up) > 0f)
			{
				PlayAnimation("push", false);
				collider.velocity += base.transform.right * pushSpeed;
			}
		}
		m_state.groundTimer += Time.fixedDeltaTime;
		if (!m_state.wasOnGround)
		{
			m_state.flippedInAir = false;
			m_state.airTimer = 0f;
			if (Player.OnPlayerLand != null && PlayerManager.IsHumanPlayer(this))
			{
				Player.OnPlayerLand(this);
			}
			m_state.wasOnGround = true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_skier = GetComponent<Skier>();
		m_voSource = GetComponent<VOSource>();
		if ((bool)airEffectPrefab)
		{
			m_airEffect = TransformUtils.Instantiate(airEffectPrefab, base.transform);
		}
		if ((bool)backflipEffectPrefab)
		{
			m_backflipEffect = TransformUtils.Instantiate(backflipEffectPrefab, base.transform);
		}
		if ((bool)base.FlamePowerup)
		{
			base.FlamePowerup.Skier = m_skier;
		}
		if ((bool)invulnerabilityGlowPrefab)
		{
			m_invulnerabilityGlow = TransformUtils.Instantiate(invulnerabilityGlowPrefab, base.transform);
			m_invulnerabilityGlow.enableEmission = false;
		}
	}

	protected override void Start()
	{
		Skier skier = Skier;
		skier.OnJump = (Skier.OnJumpDelegate)Delegate.Combine(skier.OnJump, new Skier.OnJumpDelegate(OnJump));
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_state = new State();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.IsDisabling)
		{
			if (m_skier.Collider.OnGround)
			{
				UpdateOnGround();
			}
			else
			{
				UpdateInAir();
			}
			if (m_skier.IsGliding)
			{
				m_state.glideTrailTimer = Mathf.Min(glideTrailTransitionDuration, m_state.glideTrailTimer + Time.fixedDeltaTime);
			}
			else
			{
				m_state.glideTrailTimer = Mathf.Max(0f, m_state.glideTrailTimer - Time.fixedDeltaTime);
			}
			if ((bool)m_airEffect)
			{
				m_airEffect.SetGlidingRatio(m_state.glideTrailTimer / glideTrailTransitionDuration);
			}
			if ((bool)m_backflipEffect)
			{
				m_backflipEffect.SetBackflipRatio(Skier.AirInputRatio);
			}
			if ((bool)m_invulnerabilityGlow)
			{
				m_invulnerabilityGlow.enableEmission = base.InvulnerabilityTimer > invulnerabilityGlowFadeDuration && IgnitionLevel == FlamePowerup.Level.None;
			}
		}
	}
}
