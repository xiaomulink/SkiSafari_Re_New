using UnityEngine;

public class CircleCollider : MonoBehaviour
{
	public delegate bool OnPreCollisionDelegate(GeometryUtils.ContactInfo contactInfo);

	public delegate void OnCollisionDelegate(GeometryUtils.ContactInfo contactInfo);

	public TerrainLayer terrainLayer;

	public float radius = 0.5f;

	public Vector3 velocity = Vector3.zero;

	public float gravity = -10f;

	public float maxSpeed = 300f;

	public float bounce;

	public float reflect;

	public float suction;

	public float drag;

	public float groundContactTimeThreshold = 0.1f;

	public int maxCollisionSteps = 2;

	public OnPreCollisionDelegate OnPreCollision;

	public OnCollisionDelegate OnCollision;

	private Vector3 m_initialVelocity;

	private float m_sqrRadius;

	private GeometryUtils.ContactInfo m_contactInfo = default(GeometryUtils.ContactInfo);

	private float m_lastGroundContactTime;

	private GeometryUtils.ContactInfo m_lastGroundContactInfo;

	private float m_lastCeilingContactTime;

	private GeometryUtils.ContactInfo m_lastCeilingContactInfo;

	private Vector3 m_lastPosition;

	private Vector3 m_secondLastPosition;

	private float m_lastFixedUpdateTime;

	public float SqrRadius
	{
		get
		{
			return m_sqrRadius;
		}
	}

	public bool OnGround
	{
		get
		{
			return Time.time - m_lastGroundContactTime <= groundContactTimeThreshold;
		}
	}

	public bool OnCeiling
	{
		get
		{
			return Time.time - m_lastCeilingContactTime <= groundContactTimeThreshold;
		}
	}

	public GeometryUtils.ContactInfo GroundContactInfo
	{
		get
		{
			return m_lastGroundContactInfo;
		}
	}

	public GeometryUtils.ContactInfo LastContactInfo
	{
		get
		{
			return (!(m_lastGroundContactTime >= m_lastCeilingContactTime)) ? m_lastCeilingContactInfo : m_lastGroundContactInfo;
		}
	}

	public void ApplyJumpImpulse(Vector3 impulse)
	{
		velocity += impulse;
		m_lastGroundContactTime = Time.time - groundContactTimeThreshold - Mathf.Epsilon;
	}

	public void Teleport(Vector3 position)
	{
		m_lastPosition = position;
		base.transform.position = position;
	}

	private void UpdateVelocity()
	{
		velocity.y += gravity * Time.deltaTime;
		if (OnGround && suction > 0f)
		{
			velocity -= m_lastGroundContactInfo.normal * suction * Time.deltaTime;
		}
		if (drag > 0f)
		{
			velocity -= velocity * drag;
		}
	}
    //¸üÐÂ
	private void UpdateMovement()
	{
		float magnitude = velocity.magnitude;
		if (!(magnitude > Mathf.Epsilon))
		{
			return;
		}
		m_contactInfo.Invalidate();
		float num = Time.deltaTime;
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if ((bool)terrainForLayer)
		{
			for (int i = 0; i < maxCollisionSteps; i++)
			{
				if (!terrainForLayer.TestCircleCollision(base.transform.position, radius, velocity, num, base.gameObject.layer, ref m_contactInfo,name))
				{
					break;
				}
				float num2 = m_contactInfo.distance / magnitude;
				num -= num2;
				base.transform.position = m_contactInfo.position;
				Vector3 lhs = velocity / magnitude;
				float num3 = Vector3.Dot(lhs, m_contactInfo.normal);
				m_contactInfo.normalSpeed = num3 * magnitude;
				m_contactInfo.angle = Mathf.Acos(num3) * 57.29578f;
				if (OnPreCollision != null && !OnPreCollision(m_contactInfo))
				{
					break;
				}
				float num4 = bounce;
				float num5 = reflect;
				if ((bool)m_contactInfo.collider)
				{
					num4 += m_contactInfo.collider.bounce;
					num5 += m_contactInfo.collider.reflect;
				}
				velocity -= m_contactInfo.normal * m_contactInfo.normalSpeed * (1f + num4);
				if (num5 > 0f)
				{
					Vector3 normalized = Vector3.Cross(m_contactInfo.normal, Vector3.forward).normalized;
					velocity -= normalized * m_contactInfo.normalSpeed * num5 * Mathf.Sign(Vector3.Dot(normalized, velocity));
				}
				if (Vector3.Dot(m_contactInfo.normal, Vector3.up) > 0f)
				{
					m_lastGroundContactTime = Time.time;
					m_lastGroundContactInfo = m_contactInfo;
				}
				else
				{
					m_lastCeilingContactTime = Time.time;
					m_lastCeilingContactInfo = m_contactInfo;
				}
				if (OnCollision != null)
				{
					OnCollision(m_contactInfo);
				}
				m_contactInfo.Invalidate();
			}
		}
		base.transform.position += velocity * num;
		magnitude = velocity.magnitude;
		if (magnitude > maxSpeed)
		{
			velocity *= maxSpeed / magnitude;
			magnitude = maxSpeed;
		}
		if (!OnGround)
		{
			m_lastGroundContactInfo.Invalidate();
		}
	}

	private void Awake()
	{
		m_initialVelocity = velocity;
		m_sqrRadius = radius * radius;
	}

	private void OnEnable()
	{
		velocity = m_initialVelocity;
		m_lastGroundContactTime = float.MinValue;
		m_lastGroundContactInfo.Invalidate();
		m_lastCeilingContactTime = float.MinValue;
		m_lastCeilingContactInfo.Invalidate();
		m_lastPosition = base.transform.position;
		m_secondLastPosition = base.transform.position;
		m_lastFixedUpdateTime = Time.time;
	}

	public void FixedUpdate()
	{
		base.transform.position = m_lastPosition;
		UpdateVelocity();
		UpdateMovement();
		m_secondLastPosition = m_lastPosition;
		m_lastPosition = base.transform.position;
		m_lastFixedUpdateTime = Time.time;
	}

	private void Update()
	{
		float num = Time.time - m_lastFixedUpdateTime;
		float t = num / Time.fixedDeltaTime;
		base.transform.position = Vector3.Lerp(m_secondLastPosition, m_lastPosition, t);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
