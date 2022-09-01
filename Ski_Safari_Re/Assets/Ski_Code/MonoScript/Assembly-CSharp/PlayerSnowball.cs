using System.Collections.Generic;
using UnityEngine;

public class PlayerSnowball : Player
{
	public delegate void OnPlayerRecoverDelegate(PlayerSnowball previousPlayer, Player player);

	public string category = "snowball";

	public PlayerSkier skierPrefab;

	public GameObject detachedPrefab;

	public float minSpeedToBreak = 10f;

	public Player recoverPrefab;

	public float recoverRotationSpeed;

	public Sound recoverSound;

	public float lifeTime = 10f;

	public GameObject destroyEffect;

	public AnimatedSprite sprite;

	public FXTrail ballTrail;

	public float maxTimeOffGroundForTrail = 0.5f;

	public FXCameraShake cameraShake;

	public static OnPlayerRecoverDelegate OnPlayerRecover;

	private Snowball m_snowball;

	private float m_ballTrailStartWidth;

	private float m_cameraShakeDefaultMagnitude;

	private float m_airTimer;

	private float m_groundTimer;

	private float m_lifeTimer;

	public override string Category
	{
		get
		{
			return category;
		}
	}

	public override float LiftInput
	{
		get
		{
			return m_snowball.LiftInput;
		}
		set
		{
			m_snowball.LiftInput = value;
		}
	}

	public void BreakSnowball()
	{
		TransformUtils.Instantiate(destroyEffect, base.transform, false, true);
		Vector3 position = base.transform.position;
		Vector3 velocity = m_snowball.Collider.velocity;
		FlamePowerup.Level ignitionLevel = IgnitionLevel;
		List<Attachment> attachments = PopAttachments();
		Pool.Despawn(base.gameObject);
		Player player = PlayerManager.SpawnReplacement(this, skierPrefab, position, Quaternion.identity);
		player.Collider.velocity = velocity;
		if (ignitionLevel > FlamePowerup.Level.None)
		{
			player.Ignite(ignitionLevel);
		}
		player = player.TryMount(attachments, false);
		player.PushAttachments(attachments);
		if ((bool)detachedPrefab)
		{
			Pool.Spawn(detachedPrefab, base.transform.position, m_snowball.rotationNode.rotation);
		}
	}

	public void Recover()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = m_snowball.rotationNode.rotation;
		Vector3 velocity = m_snowball.Collider.velocity;
		FlamePowerup.Level ignitionLevel = IgnitionLevel;
		List<Attachment> attachments = PopAttachments();
		Pool.Despawn(base.gameObject);
		Player player = PlayerManager.SpawnReplacement(this, recoverPrefab, position, rotation);
		player.Collider.velocity = velocity;
		if (ignitionLevel > FlamePowerup.Level.None)
		{
			player.Ignite(ignitionLevel);
		}
		player = player.TryMount(attachments, false);
		if ((bool)player.VOSource)
		{
			player.VOSource.PlayOneShotVO(recoverSound, true);
		}
		player.PushAttachments(attachments);
		if (OnPlayerRecover != null)
		{
			OnPlayerRecover(this, player);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_snowball = GetComponent<Snowball>();
		m_ballTrailStartWidth = ballTrail.startWidth;
		m_cameraShakeDefaultMagnitude = cameraShake.magnitude;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_airTimer = 0f;
		m_groundTimer = 0f;
		m_lifeTimer = 0f;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.IsDisabling)
		{
			return;
		}
		if (!m_snowball.Collider.OnGround)
		{
			m_airTimer += Time.fixedDeltaTime;
			m_groundTimer = 0f;
		}
		else
		{
			m_groundTimer += Time.fixedDeltaTime;
			m_airTimer = 0f;
		}
		ballTrail.StartWidth = m_ballTrailStartWidth * Collider.radius;
		cameraShake.magnitude = Collider.velocity.magnitude / Collider.maxSpeed * m_cameraShakeDefaultMagnitude;
		m_lifeTimer += Time.deltaTime;
		if (m_lifeTimer >= lifeTime)
		{
			BreakSnowball();
		}
		else if (base.InvulnerabilityTimer <= 0f)
		{
			if (Collider.velocity.magnitude <= minSpeedToBreak)
			{
				BreakSnowball();
			}
			else if ((bool)recoverPrefab && m_snowball.RotationSpeed <= recoverRotationSpeed)
			{
				Recover();
			}
		}
	}
}
