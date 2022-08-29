using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider))]
public class Flyer : MonoBehaviour
{
	public float defaultSpeed = 5f;

	public float acceleration;

	public float minLateralDrag = 5f;

	public float maxLateralDrag = 50f;

	public float reverseDrag = 20f;

	public float minGravityRotationSpeed = 30f;

	public float maxGravityRotationSpeed = 90f;

	public float minInputRotationSpeed = 90f;

	public float maxInputRotationSpeed = 180f;

	public float minWindRotationSpeed = 180f;

	public float maxWindRotationSpeed = 360f;

	public float maxWindSpeedForRotation = 50f;

	public float groundRotationFilter = 5f;

	public float groundRotationOffset = 5f;

	public float groundAcceleration;

	public float groundSuction;

	public float groundDrag;

	private CircleCollider m_collider;

	private float m_liftInput;

	private float m_throttleInput;

	private Vector3 m_windVelocity;

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
			m_liftInput = value;
		}
	}

	public float ThrottleInput
	{
		get
		{
			return m_throttleInput;
		}
		set
		{
			m_throttleInput = value;
		}
	}

	public void SetUpdraftSpeed(float updraftSpeed)
	{
		m_windVelocity.y = Mathf.Max(m_windVelocity.y, updraftSpeed);
	}

	protected virtual void OnCollision(GeometryUtils.ContactInfo contactInfo)
	{
	}

	private void OnEnable()
	{
		m_liftInput = 0f;
		m_throttleInput = 1f;
		m_windVelocity = Vector3.zero;
	}

	private void Start()
	{
		m_collider = GetComponent<CircleCollider>();
		CircleCollider collider = m_collider;
		collider.OnCollision = (CircleCollider.OnCollisionDelegate)Delegate.Combine(collider.OnCollision, new CircleCollider.OnCollisionDelegate(OnCollision));
	}

	private void FixedUpdate()
	{
		if (!m_collider)
		{
			m_collider = GetComponent<CircleCollider>();
		}
		if (LiftInput > 0f)
		{
			m_collider.suction = 0f;
		}
		else
		{
			m_collider.suction = groundSuction;
		}
		Vector3 rhs = m_collider.velocity - m_windVelocity;
		float num = Vector3.Dot(base.transform.right, rhs);
		float num2 = Mathf.Clamp01(num / m_collider.maxSpeed);
		float num3 = Vector3.Dot(base.transform.up, rhs);
		if (m_collider.OnGround)
		{
			m_collider.drag = groundDrag;
			Vector3 normalized = Vector3.Cross(m_collider.GroundContactInfo.normal, Vector3.forward).normalized;
			m_collider.velocity += normalized * groundAcceleration * m_throttleInput * Time.deltaTime;
			Quaternion b = Quaternion.LookRotation(Vector3.forward, m_collider.GroundContactInfo.normal);
			b *= Quaternion.AngleAxis(groundRotationOffset, Vector3.forward);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, groundRotationFilter * Time.deltaTime);
		}
		else
		{
			m_collider.drag = 0f;
			float num4 = 0f - Vector3.Dot(base.transform.right, Vector3.right);
			float num5 = 0f;
			float magnitude = m_windVelocity.magnitude;
			if (magnitude > 0f)
			{
				float t = Mathf.Min(1f, magnitude / maxWindSpeedForRotation);
				Vector3 normalized2 = Vector3.Cross(m_collider.velocity, Vector3.forward).normalized;
				float num6 = Vector3.Dot(base.transform.right, normalized2);
				num5 = Mathf.Lerp(minWindRotationSpeed, maxWindRotationSpeed, t) * num6;
			}
			float num7 = Mathf.Lerp(maxGravityRotationSpeed, minGravityRotationSpeed, num2) * num4;
			float num8 = Mathf.Lerp(minInputRotationSpeed, maxInputRotationSpeed, num2) * m_liftInput;
			float angle = (num7 + num8 + num5) * Time.deltaTime;
			base.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
			Vector3 velocity = m_collider.velocity;
			if (num < defaultSpeed)
			{
				velocity += base.transform.right * (acceleration * m_throttleInput * Time.deltaTime);
			}
			float num9 = Mathf.Lerp(minLateralDrag, maxLateralDrag, num2 * num2);
			if (num3 > 0f)
			{
				velocity -= base.transform.up * Mathf.Min(num3, num9 * Time.deltaTime);
			}
			else
			{
				velocity += base.transform.up * Mathf.Min(0f - num3, num9 * Time.deltaTime);
			}
			m_collider.velocity = velocity;
		}
		m_windVelocity = Vector3.zero;
	}
}
