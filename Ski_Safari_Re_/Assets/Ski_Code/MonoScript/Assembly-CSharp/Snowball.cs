using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider))]
public class Snowball : MonoBehaviour
{
	public Transform rotationNode;

	public Transform scaleNode;

	public float growRate = 0.5f;

	public float minRadius = 1f;

	public float maxRadius = 2f;

	public float colliderBaseRadius;

	public float minSpeed = 40f;

	public float maxSpeed = 50f;

	public float minJumpImpulse = 10f;

	public float maxJumpImpulse = 20f;

	public float groundAcceleration = 10f;

	public float groundSuction;

	public float groundRotationFriction = 5f;

	public float airRotationDrag = 0.25f;

	public float airBrakeRotationDeceleration;

	public float minRotationSpeed = 90f;

	public float maxRotationSpeed = 10000f;

	public FXSurfaceEffect[] surfaceEffectPrefabs;

	public SurfaceType defaultSurfaceType;

	public float maxTimeOffGroundForEffects = 0.15f;

	private CircleCollider m_collider;

	private Vector3 m_initialScale = Vector3.one;

	private Dictionary<SurfaceType, FXSurfaceEffect> m_surfaceEffects = new Dictionary<SurfaceType, FXSurfaceEffect>();

	private LineCollider m_lastContactCollider;

	private FXSurfaceEffect m_currentSurfaceEffect;

	private float m_radius;

	private float m_liftInput;

	private float m_jumpInput;

	private float m_rotationSpeed;

	private float m_airTimer;

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
			if (value > 0f && m_liftInput == 0f && m_collider.OnGround)
			{
				m_jumpInput = 1f;
			}
			m_liftInput = value;
		}
	}

	public float RotationSpeed
	{
		get
		{
			return m_rotationSpeed;
		}
	}

	public void Grow(float growAmount)
	{
		m_radius = Mathf.Min(m_radius + growAmount, maxRadius);
		m_collider.radius = colliderBaseRadius + m_radius;
		if ((bool)scaleNode)
		{
			scaleNode.localScale = m_initialScale * m_radius;
		}
		if (m_collider.OnGround)
		{
			base.transform.position += m_collider.GroundContactInfo.normal * growAmount;
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
	}

	private void Awake()
	{
		m_collider = GetComponent<CircleCollider>();
		if ((bool)scaleNode)
		{
			m_initialScale = scaleNode.localScale;
		}
		FXSurfaceEffect[] array = surfaceEffectPrefabs;
		foreach (FXSurfaceEffect fXSurfaceEffect in array)
		{
			FXSurfaceEffect value = TransformUtils.Instantiate(fXSurfaceEffect, base.transform, true);
			m_surfaceEffects[fXSurfaceEffect.surfaceType] = value;
		}
	}

	private void OnEnable()
	{
		m_liftInput = 0f;
		m_jumpInput = 0f;
		m_airTimer = 0f;
		m_radius = minRadius;
		m_collider.radius = colliderBaseRadius + minRadius;
		if ((bool)scaleNode)
		{
			scaleNode.localScale = m_initialScale * minRadius;
		}
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		rotationNode.rotation = rotation;
		m_collider.maxSpeed = minSpeed;
		m_rotationSpeed = Mathf.Clamp(m_collider.velocity.magnitude / m_collider.radius * 57.29578f, minRotationSpeed, maxRotationSpeed);
		m_lastContactCollider = null;
		m_surfaceEffects.TryGetValue(defaultSurfaceType, out m_currentSurfaceEffect);
	}

	private void FixedUpdate()
	{
		if (!m_collider)
		{
			m_collider = GetComponent<CircleCollider>();
		}
		if (LiftInput > 0f || m_jumpInput > 0f)
		{
			m_collider.suction = 0f;
		}
		else
		{
			m_collider.suction = groundSuction;
		}
		float num = Vector3.Dot(base.transform.right, m_collider.velocity);
		float t = Mathf.Clamp(num / m_collider.maxSpeed, 0f, 1f);
		if (m_jumpInput > 0f)
		{
			float y = Mathf.Lerp(minJumpImpulse, maxJumpImpulse, t);
			m_collider.ApplyJumpImpulse(new Vector3(0f, y));
			m_jumpInput = 0f;
			if ((bool)m_currentSurfaceEffect)
			{
				m_currentSurfaceEffect.Jump();
			}
		}
		if (m_collider.OnGround)
		{
			if (m_airTimer > 0f && (bool)m_currentSurfaceEffect)
			{
				m_currentSurfaceEffect.Land(0f - m_collider.GroundContactInfo.normalSpeed);
			}
			Vector3 normalized = Vector3.Cross(m_collider.GroundContactInfo.normal, Vector3.forward).normalized;
			float num2 = Vector3.Dot(normalized, m_collider.velocity);
			float b = Mathf.Min(maxRotationSpeed, num2 / m_collider.radius * 57.29578f);
			if (num2 < m_collider.maxSpeed)
			{
				float num3 = Mathf.Min(m_collider.maxSpeed - num2, groundAcceleration * Time.fixedDeltaTime);
				m_collider.velocity += normalized * num3;
			}
			m_rotationSpeed = Mathf.Lerp(m_rotationSpeed, b, groundRotationFriction * Time.fixedDeltaTime);
			float growAmount = growRate * Time.fixedDeltaTime;
			Grow(growAmount);
			m_airTimer = 0f;
		}
		else
		{
			if (m_liftInput > 0f && airBrakeRotationDeceleration > 0f)
			{
				float z = rotationNode.eulerAngles.z;
				if (m_rotationSpeed < minRotationSpeed && z >= 45f && z <= 315f)
				{
					m_rotationSpeed = Mathf.Lerp(m_rotationSpeed, minRotationSpeed, airRotationDrag * Time.fixedDeltaTime);
				}
				else
				{
					m_rotationSpeed -= airBrakeRotationDeceleration * Time.fixedDeltaTime;
				}
			}
			else
			{
				m_rotationSpeed = Mathf.Lerp(m_rotationSpeed, minRotationSpeed, airRotationDrag * Time.fixedDeltaTime);
			}
			m_airTimer += Time.deltaTime;
		}
		rotationNode.rotation *= Quaternion.AngleAxis((0f - m_rotationSpeed) * Time.fixedDeltaTime, Vector3.forward);
	}

	private void Update()
	{
		UpdateSurfaceEffects();
	}
}
