using System;
using System.Threading.Tasks;
using UnityEngine;

public class Avalanche : MonoBehaviour
{
	[Serializable]
	public class DistanceModifier
	{
		public float distance = 5000f;

		public float farSpeedIncrease = 5f;

		public float closeSpeedIncrease = 5f;

		public float trailHeightIncrease = 1f;

		public float effectsAnimDuration = 1f;
	}

	public class Segment
	{
		public Vector3 position;

		public Vector3 dir;

		public float heightRatio;

		public float terrainOffset;

		public float additionalHeight;

		public float verticalSpeed;
	}

	public static Avalanche Instance;

	public TerrainLayer terrainLayer;

	public float closeSpeed = 20f;

	public float farSpeed = 40f;

	public float farDistance = 100f;

	public float caveHelperSpeedRatio = 0.75f;

	public float gravity = -40f;

	public float rotationFilter = 5f;

	public DistanceModifier[] distanceModifiers;

	public Material trailMaterial;

	public float trailHeight = 10f;

	public float trailSurfacePenetraation = 2f;

	public int trailHeadSegmentCount = 20;

	public int trailSegmentCount = 100;

	public float trailSegmentWidth = 0.5f;

	public float tailGravity = -20f;

	public float tailFallGrowthSpeed = 5f;

	public Animation effectsAnim;

	public Transform effectsNode;

	public float effectsRotationFilter = 5f;

	public AvalancheTendril tendrilPrefab;

	public float timeBetweenTendrilsMin = 1f;

	public float timeBetweenTendrilsMax = 5f;

	public float tendrilSpeedOffset = -5f;

	public float tendrilRotationOffset = -30f;

	public float tendrilPositionOffset = -5f;

	private float m_currentSpeed;

	private int m_distanceModifierIndex;

	private float m_previousDistance;

	private float m_previousCloseSpeed;

	private float m_previousFarSpeed;

	private float m_previousTrailHeight;

	private float m_previousEffectsAnimTime;

	private AnimationState m_effectsAnimState;

	private  ParticleSystem[] m_effectEmitters;

	private Quaternion m_lastEffectsNodeRotation;

	private GameObject m_trail;

	private MeshFilter m_meshFilter;

	private Mesh m_mesh;

	private Vector3[] m_vertices;

	private int[] m_triangles;

	private Vector2[] m_uvs;

	private Vector3 m_lastTrailPosition;

	private float m_lastTerrainHeight;

	private Segment[] m_segments;

	private float m_tendrilTimer;

	private bool m_isOffScreen;

	public bool Contains(Vector3 position)
	{
		if (!m_mesh.bounds.Contains(position))
		{
			return false;
		}
		int num = -1;
		Segment[] segments = m_segments;
		foreach (Segment segment in segments)
		{
			num++;
			Vector3 position2 = segment.position;
			if (!(position.x < position2.x - trailHeight) && !(position.x > position2.x + trailHeight) && !(position.y > position2.y + trailHeight))
			{
				Vector3 lhs = position - position2;
				float num2 = Vector3.Dot(lhs, segment.dir);
				if (num2 <= trailHeight * segment.heightRatio && num2 >= 0f - (trailSurfacePenetraation * segment.heightRatio + segment.additionalHeight))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void UpdateDistanceModifiers()
	{
		float currentDistance = SkiGameManager.Instance.CurrentDistance;
		float num = currentDistance - m_previousDistance;
		if (num > distanceModifiers[m_distanceModifierIndex].distance)
		{
			if (m_distanceModifierIndex < distanceModifiers.Length - 1)
			{
				DistanceModifier distanceModifier = distanceModifiers[m_distanceModifierIndex];
				float num2 = num - distanceModifier.distance;
				m_previousDistance = currentDistance - num2;
				m_previousFarSpeed += distanceModifier.farSpeedIncrease;
				m_previousCloseSpeed += distanceModifier.closeSpeedIncrease;
				m_previousTrailHeight += distanceModifier.trailHeightIncrease;
				m_previousEffectsAnimTime += distanceModifier.effectsAnimDuration;
				m_distanceModifierIndex++;
			}
			else
			{
				num = distanceModifiers[m_distanceModifierIndex].distance;
			}
		}
		DistanceModifier distanceModifier2 = distanceModifiers[m_distanceModifierIndex];
		float num3 = Mathf.Clamp01(num / distanceModifier2.distance);
		farSpeed = m_previousFarSpeed + num3 * distanceModifier2.farSpeedIncrease;
		closeSpeed = m_previousCloseSpeed + num3 * distanceModifier2.closeSpeedIncrease;
		trailHeight = m_previousTrailHeight + num3 * distanceModifier2.trailHeightIncrease;
		m_effectsAnimState.time = m_previousEffectsAnimTime + num3 * distanceModifier2.effectsAnimDuration;
		effectsAnim.Sample();
	}

	private void SetIsOffscreen(bool state)
	{
		if (state != m_isOffScreen)
		{
            ParticleSystem[] effectEmitters = m_effectEmitters;
			foreach (ParticleSystem ParticleEmitter in effectEmitters)
			{
				  ParticleEmitter.enableEmission = !state;
			}
			m_isOffScreen = state;
		}
	}

	private void UpdateMovement()
	{
		Vector3 position = m_segments[trailHeadSegmentCount].position;
		position.y += trailHeight;
		Vector3 vector = Camera.main.WorldToViewportPoint(position);
		if (vector.x > 1f)
		{
			SetIsOffscreen(true);
			return;
		}
		if (!Player.Instance && vector.y < 0f)
		{
			SetIsOffscreen(true);
			return;
		}
		if (Camera.main.WorldToViewportPoint(m_segments[trailHeadSegmentCount].position).x < -1f)
		{
			SetIsOffscreen(true);
		}
		else
		{
			SetIsOffscreen(false);
		}
		Transform transform = ((!Player.Instance) ? Camera.main.transform : Player.Instance.transform);
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if (terrainForLayer.IsPositionInOrAboveCave((int)base.transform.position.x) && terrainForLayer.IsPointAboveCave(transform.position))
		{
			m_currentSpeed = closeSpeed * caveHelperSpeedRatio;
		}
		else
		{
			float num = transform.position.x - base.transform.position.x;
			float t = Mathf.Clamp01(num / farDistance);
			m_currentSpeed = Mathf.Lerp(closeSpeed, farSpeed, t);
		}
		Vector3 vector2 = base.transform.right * m_currentSpeed;
		Vector3 vector3 = base.transform.position + vector2 * Time.deltaTime;
		Quaternion b = base.transform.rotation;
		float height = 0f;
		Vector3 normal = Vector3.up;
		if (terrainForLayer.IsPositionInBounds(vector3.x) && terrainForLayer.GetHeightAndNormal(vector3, ref height, ref normal))
		{
			if (vector3.y > height)
			{
				vector2.y += gravity * Time.deltaTime;
				Vector3 normalized = Vector3.Cross(vector2.normalized, Vector3.back).normalized;
				b = Quaternion.LookRotation(Vector3.forward, normalized);
			}
			else
			{
				vector3.y = height;
				b = Quaternion.LookRotation(Vector3.forward, normal);
			}
		}
		else if ((bool)Player.Instance)
		{
			Vector3 normalized3 = Vector3.Cross(rhs: (Player.Instance.transform.position - base.transform.position).normalized, lhs: Vector3.forward).normalized;
			b = Quaternion.LookRotation(Vector3.forward, normalized3);
			height = vector3.y;
		}
		base.transform.position = vector3;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, rotationFilter * Time.deltaTime);
		m_lastTerrainHeight = height;
	}

	private void UpdateTrail()
	{
		Segment[] segments = m_segments;
		foreach (Segment segment in segments)
		{
			if (segment.terrainOffset > 0f)
			{
				float num = Mathf.Min(segment.terrainOffset, tailFallGrowthSpeed * Time.deltaTime);
				segment.additionalHeight = Mathf.Min(segment.terrainOffset, segment.additionalHeight + num);
			}
		}
		Vector3 position = base.transform.position;
		float num2 = Vector3.Distance(position, m_lastTrailPosition);
		if (num2 >= trailSegmentWidth)
		{
			m_lastTrailPosition = position;
			Segment segment2 = m_segments[trailSegmentCount - 1];
			for (int num3 = trailSegmentCount - 1; num3 >= trailHeadSegmentCount; num3--)
			{
				Segment segment3 = m_segments[num3 - 1];
				m_segments[num3] = segment3;
				segment3.heightRatio = 1f;
				int num4 = num3 * 2;
				m_vertices[num4] = segment3.position + segment3.dir * (0f - trailSurfacePenetraation - segment3.additionalHeight);
				m_vertices[num4 + 1] = segment3.position + segment3.dir * trailHeight;
			}
			for (int num5 = trailHeadSegmentCount - 1; num5 > 0; num5--)
			{
				Segment segment4 = m_segments[num5 - 1];
				m_segments[num5] = segment4;
				float num6 = 1f - (float)num5 / (float)(trailHeadSegmentCount - 1);
				float num7 = (segment4.heightRatio = Mathf.Sqrt(1f - num6 * num6));
				int num8 = num5 * 2;
				m_vertices[num8] = segment4.position + segment4.dir * ((0f - trailSurfacePenetraation) * num7 - segment4.additionalHeight);
				m_vertices[num8 + 1] = segment4.position + segment4.dir * (trailHeight * num7);
			}
			segment2.position = position;
			segment2.dir = base.transform.up;
			segment2.terrainOffset = position.y - m_lastTerrainHeight;
			segment2.additionalHeight = 0f;
			segment2.verticalSpeed = 0f;
			segment2.heightRatio = 0f;
			m_segments[0] = segment2;
			m_vertices[0] = position;
			m_vertices[1] = position;
		}
		else
		{
			for (int num9 = trailSegmentCount - 1; num9 >= trailHeadSegmentCount; num9--)
			{
				Segment segment5 = m_segments[num9];
				m_vertices[num9 * 2] = segment5.position + segment5.dir * (0f - trailSurfacePenetraation - segment5.additionalHeight);
			}
			float num10 = num2 / trailSegmentWidth;
			for (int num11 = trailHeadSegmentCount - 1; num11 > 0; num11--)
			{
				Segment segment6 = m_segments[num11];
				float num12 = Mathf.Max(0f, 1f - ((float)num11 + num10) / (float)(trailHeadSegmentCount - 1));
				float num13 = (segment6.heightRatio = Mathf.Sqrt(1f - num12 * num12));
				int num14 = num11 * 2;
				m_vertices[num14] = segment6.position + segment6.dir * ((0f - trailSurfacePenetraation) * segment6.heightRatio - segment6.additionalHeight);
				m_vertices[num14 + 1] = segment6.position + segment6.dir * (trailHeight * num13);
			}
			m_vertices[0] = (m_vertices[1] = position);
		}
		m_mesh.vertices = m_vertices;
		m_mesh.RecalculateBounds();
	}

	private void UpdateEffects()
	{
		m_lastEffectsNodeRotation = Quaternion.Lerp(m_lastEffectsNodeRotation, base.transform.rotation, effectsRotationFilter * Time.deltaTime);
		effectsNode.rotation = m_lastEffectsNodeRotation;
	}

	private void UpdateTendrils()
	{
		if ((bool)tendrilPrefab)
		{
			m_tendrilTimer -= Time.deltaTime;
			if (m_tendrilTimer <= 0f)
			{
				m_tendrilTimer = UnityEngine.Random.Range(timeBetweenTendrilsMin, timeBetweenTendrilsMax);
				Vector3 position = base.transform.position + base.transform.forward * tendrilPositionOffset;
				Quaternion rotation = Quaternion.AngleAxis(tendrilRotationOffset, Vector3.forward) * base.transform.rotation;
				AvalancheTendril avalancheTendril = Pool.Spawn(tendrilPrefab, position, rotation);
				avalancheTendril.speed = m_currentSpeed + tendrilSpeedOffset;
			}
		}
	}

	private void UpdateAudio()
	{
		float b = ((!m_isOffScreen && !SkiGameManager.Instance.Finished) ? 1f : 0f);
		GetComponent<AudioSource>().volume = Mathf.Lerp(GetComponent<AudioSource>().volume, b, 5f * Time.deltaTime);
	}

	private void InitialiseTrailMesh()
	{
		m_trail = new GameObject("Avalanche Trail");
		m_meshFilter = m_trail.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = m_trail.AddComponent<MeshRenderer>();
		meshRenderer.material = trailMaterial;
		m_vertices = new Vector3[trailSegmentCount * 2];
		m_triangles = new int[(m_vertices.Length - 2) * 3];
		m_uvs = new Vector2[m_vertices.Length];
		int num = 0;
		for (int i = 0; i < m_vertices.Length - 2; i += 2)
		{
			m_triangles[num++] = i;
			m_triangles[num++] = i + 2;
			m_triangles[num++] = i + 1;
			m_triangles[num++] = i + 1;
			m_triangles[num++] = i + 2;
			m_triangles[num++] = i + 3;
		}
		float num2 = 0f;
		float num3 = 1f / (float)trailSegmentCount;
		for (int j = 0; j < m_vertices.Length; j += 2)
		{
			m_uvs[j] = new Vector2(num2, 0f);
			m_uvs[j + 1] = new Vector2(num2, 1f);
			num2 += num3;
		}
		m_mesh = new Mesh();
		m_mesh.vertices = m_vertices;
		m_mesh.triangles = m_triangles;
		m_mesh.uv = m_uvs;
		m_meshFilter.mesh = m_mesh;
	}

	private void Awake()
	{
		if (terrainLayer == TerrainLayer.Game)
		{
			Instance = this;
		}
		InitialiseTrailMesh();
		m_lastEffectsNodeRotation = Quaternion.identity;
	}

	private async void Start()
	{
		m_lastTrailPosition = base.transform.position;
		for (int i = 0; i < m_vertices.Length; i++)
		{
			m_vertices[i] = m_lastTrailPosition;
		}
		m_segments = new Segment[trailSegmentCount];
		for (int j = 0; j < trailSegmentCount; j++)
		{
			Segment segment = new Segment();
			segment.position = m_lastTrailPosition;
			m_segments[j] = segment;
		}
		m_distanceModifierIndex = 0;
		m_previousDistance = 0f;
		m_previousCloseSpeed = closeSpeed;
		m_previousFarSpeed = farSpeed;
		m_previousTrailHeight = trailHeight;
		m_previousEffectsAnimTime = 0f;
		m_effectsAnimState = effectsAnim[effectsAnim.clip.name];
		m_effectsAnimState.time = 0f;
		effectsAnim.Sample();
		m_effectEmitters = effectsAnim.GetComponentsInChildren<ParticleSystem>();
        if(SkiGameManager.Instance.isOnline)
        {
            await Task.Delay(5000);
            gameObject.SetActive(false);
        }
	}

	private void OnDestroy()
	{
		if ((bool)m_trail)
		{
			UnityEngine.Object.Destroy(m_trail);
		}
	}

	private void Update()
	{
		if (Time.deltaTime != 0f)
		{
			UpdateDistanceModifiers();
			UpdateMovement();
			UpdateTrail();
			UpdateTendrils();
			UpdateAudio();
		}
	}

	private void LateUpdate()
	{
		UpdateEffects();
	}

	private void OnDrawGizmosSelected()
	{
		if (m_segments != null)
		{
			Gizmos.color = Color.magenta;
			Vector3 from = m_segments[0].position;
			for (int i = 1; i < trailSegmentCount; i++)
			{
				Vector3 position = m_segments[i].position;
				Gizmos.DrawLine(from, position);
				Gizmos.DrawLine(position, position + m_segments[i].terrainOffset * Vector3.down);
				from = position;
			}
		}
	}
}
