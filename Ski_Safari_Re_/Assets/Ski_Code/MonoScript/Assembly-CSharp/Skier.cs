using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider))]
public class Skier : MonoBehaviour
{
	public delegate void OnSkierJumpDelegate(Skier skier);

	public delegate void OnJumpDelegate();

	public bool usePlayerSkierModifiers;

	public float minLateralDrag = 5f;

	public float maxLateralDrag = 50f;

	public float reverseDrag = 20f;

	public float minJumpImpulse = 50;

	public float maxJumpImpulse = 100;

	public float timeBetweenAirJumps = -1f;

	public bool rotateTowardsVelocity;

	public bool rotateTowardsVelocityWhenJumping;

	public bool rotateTowardsVelocityOnCollision;

	public bool applyJumpInLocalSpace;

	public float airRotationFilter = 20f;

	public bool alwaysApplyGravityRotation;

	public float minGravityRotationSpeed = 30f;

	public float maxGravityRotationSpeed = 90f;

	public float minInputRotationSpeed = 90f;

	public float maxInputRotationSpeed = 180f;

	public float maxInputRotation = 360f;

	public bool maxInputRotationIsRelativeToVelocity;

	public float inputRotationSpeedRampDuration = 0.5f;

	public float maxHeightAboveTerrainForInput = -1f;

	public float minHeightAboveTerrainForInput = -1f;

	public float jumpSnapRotation;

	public float initialSpeedLimit;

	public float initialSpeedLimitDuration;

	public float airDrag;

	public float groundRotationFilter = 5f;

	public float groundRotationOffset = 5f;

	public float groundAcceleration;

	public float minGroundSpeed;

	public float maxSpeedForGroundAcceleration = 100f;

	public float groundSuction;

	public float groundDrag;

	public float minForwardSpeedForGlide = 20f;

	public float maxLateralSpeedForGlide = 5f;

	public FXSurfaceEffect[] surfaceEffectPrefabs;

	public SurfaceType defaultSurfaceType;

	public float maxTimeOffGroundForEffects = 0.15f;

	public Sound[] jumpSounds;

	public bool forcePlayJumpSound;

	public Sound[] landSounds;

	public Sound[] badLandSounds;

	public float badLandSpeed = 20f;

	public static OnSkierJumpDelegate OnSkierJump;

	public OnJumpDelegate OnJump;

	private CircleCollider m_collider;

	private float m_maxSpeed;

	private VOSource m_voSource;

	private Dictionary<SurfaceType, FXSurfaceEffect> m_surfaceEffects = new Dictionary<SurfaceType, FXSurfaceEffect>();

	private LineCollider m_lastContactCollider;

	private FXSurfaceEffect m_currentSurfaceEffect;

	private bool m_isGliding;

	private float m_liftInput;

	private float m_jumpInput;

	private float m_groundTimer;

	private float m_airTimer;

	private float m_airLiftInputTimer;

	private float m_airJumpTimer;

	private float m_speedLimitTimer;

	private static List<Skier> s_allSkiers = new List<Skier>();

	public static List<Skier> AllSkiers
	{
		get
		{
			return s_allSkiers;
		}
	}

	public CircleCollider Collider
	{
		get
		{
			return m_collider;
		}
	}

	public float LiftInput
	{
		get
		{
			return m_liftInput;
		}
		set
		{
			if (value > 0f && m_liftInput == 0f)
			{
				m_jumpInput = 1f;
			}
			m_liftInput = value;
		}
	}

	public bool IsGliding
	{
		get
		{
			return m_isGliding;
		}
	}

	public bool IsRotatingFromInput
	{
		get
		{
			return m_airLiftInputTimer >= inputRotationSpeedRampDuration;
		}
	}

	public float AirInputRatio
	{
		get
		{
			return m_airLiftInputTimer / inputRotationSpeedRampDuration;
		}
	}

	private void UpdateSurfaceEffects()
	{
		if ((bool)m_currentSurfaceEffect)
		{
			if (m_airTimer < maxTimeOffGroundForEffects)
			{
				m_currentSurfaceEffect.Position = base.transform.position - m_collider.radius * m_collider.LastContactInfo.normal;
				m_currentSurfaceEffect.Active = true;
				m_currentSurfaceEffect.TrailScale = 1f - m_airTimer / maxTimeOffGroundForEffects;
			}
			else
			{
				m_currentSurfaceEffect.Active = false;
			}
		}
	}

	private void SetSurfaceType(SurfaceType surfaceType)
	{
		FXSurfaceEffect value;
		m_surfaceEffects.TryGetValue(surfaceType, out value);
		if (value != m_currentSurfaceEffect)
		{
			bool active = false;
			if ((bool)m_currentSurfaceEffect)
			{
				active = m_currentSurfaceEffect.Active;
				m_currentSurfaceEffect.Active = false;
			}
			m_currentSurfaceEffect = value;
			if ((bool)m_currentSurfaceEffect)
			{
				m_currentSurfaceEffect.Active = active;
			}
		}
	}

	protected virtual void OnCollision(GeometryUtils.ContactInfo contactInfo)
	{
		if (contactInfo.collider != m_lastContactCollider)
		{
			m_lastContactCollider = contactInfo.collider;
			SetSurfaceType((!m_lastContactCollider) ? defaultSurfaceType : m_lastContactCollider.surfaceType);
		}
		if (rotateTowardsVelocityOnCollision)
		{
			Vector3 normalized = Vector3.Cross(Vector3.forward, m_collider.velocity.normalized).normalized;
			Quaternion rotation = Quaternion.LookRotation(Vector3.forward, normalized);
			base.transform.rotation = rotation;
		}
	}

	private void Jump(float forwardSpeedRatio)
	{
		float z = base.transform.rotation.eulerAngles.z;
		Quaternion quaternion = Quaternion.identity;
		if (jumpSnapRotation > 0f && (z > 180f || z < maxInputRotation))
		{
			quaternion = Quaternion.AngleAxis(jumpSnapRotation, Vector3.forward);
		}
		float magnitude = m_collider.velocity.magnitude;
		if (rotateTowardsVelocityWhenJumping && magnitude > Mathf.Epsilon)
		{
			Vector3 normalized = Vector3.Cross(Vector3.forward, m_collider.velocity * (1f / magnitude)).normalized;
			Quaternion quaternion2 = Quaternion.LookRotation(Vector3.forward, normalized);
			base.transform.rotation = quaternion2 * quaternion;
			m_collider.velocity = base.transform.right * magnitude;
		}
		else
		{
			base.transform.rotation *= quaternion;
		}
		if (!m_collider.LastContactInfo.isOffScreen)
		{
			if ((bool)m_voSource)
			{
				m_voSource.PlayRandomVO(jumpSounds, forcePlayJumpSound);
			}
			if ((bool)m_currentSurfaceEffect && m_collider.OnGround)
			{
				m_currentSurfaceEffect.Jump();
			}
		}
		float num = Mathf.Lerp(minJumpImpulse, maxJumpImpulse, forwardSpeedRatio);
		if (applyJumpInLocalSpace)
		{
			m_collider.ApplyJumpImpulse(base.transform.up * num);
		}
		else
		{
			m_collider.ApplyJumpImpulse(new Vector3(0f, num, 0f));
		}
		if (OnSkierJump != null)
		{
			OnSkierJump(this);
		}
		OnJump();
	}

	private void Awake()
	{
		m_collider = GetComponent<CircleCollider>();
		m_maxSpeed = m_collider.maxSpeed;
		m_voSource = GetComponent<VOSource>();
		FXSurfaceEffect[] array = surfaceEffectPrefabs;
		foreach (FXSurfaceEffect fXSurfaceEffect in array)
		{
			FXSurfaceEffect value = TransformUtils.Instantiate(fXSurfaceEffect, base.transform, true);
			m_surfaceEffects[fXSurfaceEffect.surfaceType] = value;
		}
	}

	private void OnEnable()
	{
		m_isGliding = false;
		m_liftInput = 0f;
		m_jumpInput = 0f;
		m_groundTimer = 0f;
		m_airTimer = 0f;
		m_airJumpTimer = 0f;
		m_airLiftInputTimer = 0f;
		if ((bool)m_collider && initialSpeedLimitDuration > 0f && (bool)Player.Instance)
		{
			m_speedLimitTimer = initialSpeedLimitDuration;
			m_collider.maxSpeed = initialSpeedLimit;
		}
		else
		{
			m_speedLimitTimer = 0f;
			m_collider.maxSpeed = m_maxSpeed;
		}
		m_lastContactCollider = null;
		m_surfaceEffects.TryGetValue(defaultSurfaceType, out m_currentSurfaceEffect);
		s_allSkiers.Add(this);
	}

	private void OnDisable()
	{
		s_allSkiers.Remove(this);
	}

	private void Start()
	{
		CircleCollider collider = m_collider;
		collider.OnCollision = (CircleCollider.OnCollisionDelegate)Delegate.Combine(collider.OnCollision, new CircleCollider.OnCollisionDelegate(OnCollision));
	}

	private void FixedUpdate()
	{
        if(SkiGameManager.Instance.isOnline)
        {
            minInputRotationSpeed = 240;
            maxInputRotationSpeed = 380;
        }
		if (!m_collider)
		{
			m_collider = GetComponent<CircleCollider>();
		}
		if (!m_collider.OnGround || (maxJumpImpulse > 0f && LiftInput > 0f))
		{
			m_collider.suction = 0f;
			m_collider.drag = airDrag;
		}
		else
		{
			float num = 0.25f;
			float t = Mathf.Min(1f, m_groundTimer / num);
			m_collider.suction = Mathf.Lerp(0f, groundSuction, t);
			if (usePlayerSkierModifiers)
			{
				m_collider.suction *= PlayerSkier.skierGroundSuctionRatio;
			}
			m_collider.drag = Mathf.Lerp(0f, groundDrag, t);
		}
		float num2 = Vector3.Dot(base.transform.right, m_collider.velocity);
		float num3 = Mathf.Clamp(num2 / m_collider.maxSpeed, 0f, 1f);
		if (m_collider.OnGround)
		{
			Vector3 normalized = Vector3.Cross(m_collider.GroundContactInfo.normal, Vector3.forward).normalized;
			float num4 = Vector3.Dot(normalized, m_collider.velocity);
			if (minGroundSpeed > 0f && num4 < minGroundSpeed)
			{
				float num5 = minGroundSpeed - num4;
				m_collider.velocity += normalized * num5;
				num4 += num5;
			}
			if (num4 < maxSpeedForGroundAcceleration)
			{
				m_collider.velocity += normalized * groundAcceleration * Time.deltaTime;
			}
			if (Vector3.Dot(m_collider.GroundContactInfo.normal, Vector3.up) >= 0f)
			{
				Quaternion b = Quaternion.LookRotation(Vector3.forward, m_collider.GroundContactInfo.normal);
				b *= Quaternion.AngleAxis(groundRotationOffset, Vector3.forward);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, groundRotationFilter * Time.deltaTime);
			}
			if (!m_collider.GroundContactInfo.isOffScreen && m_airTimer > 0f)
			{
				if ((bool)m_currentSurfaceEffect)
				{
					m_currentSurfaceEffect.Land(0f - m_collider.GroundContactInfo.normalSpeed);
				}
				if ((bool)m_voSource)
				{
					if (badLandSounds.Length > 0 && 0f - m_collider.GroundContactInfo.normalSpeed > badLandSpeed)
					{
						m_voSource.PlayRandomVO(badLandSounds);
					}
					else
					{
						m_voSource.PlayRandomVO(landSounds);
					}
				}
			}
			m_isGliding = false;
			m_airTimer = 0f;
			m_airJumpTimer = 0f;
			m_airLiftInputTimer = 0f;
			if (m_jumpInput > 0f)
			{
				Jump(num3);
			}
			m_groundTimer += Time.deltaTime;
		}
		else
		{
			if (rotateTowardsVelocity)
			{
				Vector3 normalized2 = Vector3.Cross(Vector3.forward, m_collider.velocity.normalized).normalized;
				Quaternion b2 = Quaternion.LookRotation(Vector3.forward, normalized2);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b2, airRotationFilter * Time.deltaTime);
			}
			else
			{
				float num6 = 0f;
				if (m_liftInput > Mathf.Epsilon)
				{
					m_airLiftInputTimer = Mathf.Min(inputRotationSpeedRampDuration, m_airLiftInputTimer + Time.deltaTime);
					float num7 = maxInputRotation;
					if (maxInputRotationIsRelativeToVelocity)
					{
						Vector2 vector = m_collider.velocity;
						float num8 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
						num7 = Mathf.Min(maxInputRotation, num8 + maxInputRotation);
					}
					float z = base.transform.rotation.eulerAngles.z;
					if (z >= 180f || z < num7)
					{
						float t2 = m_airLiftInputTimer / inputRotationSpeedRampDuration;
						num6 = Mathf.Lerp(minInputRotationSpeed, maxInputRotationSpeed, t2);
					}
					else if (maxInputRotationIsRelativeToVelocity)
					{
						num6 = 0f - maxInputRotationSpeed;
					}
					if (maxHeightAboveTerrainForInput > 0f)
					{
						float height = 0f;
						Vector3 normal = Vector3.up;
						if (Terrain.GetTerrainForLayer(TerrainLayer.Game).GetHeightAndNormal(base.transform.position, ref height, ref normal))
						{
							float num9 = base.transform.position.y - height;
							if (num9 > minHeightAboveTerrainForInput)
							{
								float num10 = Mathf.Min(1f, (num9 - minHeightAboveTerrainForInput) / (maxHeightAboveTerrainForInput - minHeightAboveTerrainForInput));
								float num11 = 0f - Mathf.Sign(Vector3.Dot(base.transform.right, Vector3.right));
								num6 += maxInputRotationSpeed * 1.5f * num11 * num10;
							}
						}
					}
				}
				if (alwaysApplyGravityRotation || m_liftInput <= Mathf.Epsilon)
				{
					m_airLiftInputTimer = 0f;
					float num12 = 0f - Mathf.Sign(Vector3.Dot(base.transform.right, Vector3.right));
					num6 += Mathf.Lerp(maxGravityRotationSpeed, minGravityRotationSpeed, num3) * num12;
				}
				float num13 = num6 * Time.deltaTime;
				if (!float.IsNaN(num13))
				{
					base.transform.rotation *= Quaternion.AngleAxis(num13, Vector3.forward);
				}
			}
			float num14 = Vector3.Dot(base.transform.up, m_collider.velocity);
			float num15 = Mathf.Lerp(minLateralDrag, maxLateralDrag, num3);
			if (usePlayerSkierModifiers)
			{
				num15 *= PlayerSkier.skierLateralDragRatio;
			}
			if (!(num14 > 0f))
			{
				num14 *= Mathf.Max(0f, Vector3.Dot(base.transform.right, Vector3.right));
				m_collider.velocity += base.transform.up * Mathf.Min(0f - num14, num15 * Time.deltaTime);
			}
			m_isGliding = num2 > minForwardSpeedForGlide && Mathf.Abs(num14) <= maxLateralSpeedForGlide;
			if (m_collider.OnCeiling)
			{
				m_airTimer = 0f;
			}
			else
			{
				m_airTimer += Time.deltaTime;
			}
			m_groundTimer = 0f;
			if (timeBetweenAirJumps > 0f)
			{
				m_airJumpTimer += Time.deltaTime;
				if (m_jumpInput > 0f && m_airJumpTimer > timeBetweenAirJumps)
				{
					Jump(num3);
					m_airJumpTimer = 0f;
				}
			}
		}
		m_jumpInput = 0f;
		if (m_speedLimitTimer > 0f)
		{
			m_speedLimitTimer -= Time.deltaTime;
			if (m_speedLimitTimer <= 0f)
			{
				m_collider.maxSpeed = m_maxSpeed;
			}
		}
	}

	private void Update()
	{
		UpdateSurfaceEffects();
	}
}
