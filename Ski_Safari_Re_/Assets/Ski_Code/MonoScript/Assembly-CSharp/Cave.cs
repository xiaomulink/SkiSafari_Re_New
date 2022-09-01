using System;
using UnityEngine;

public class Cave : MonoBehaviour
{
	private class CaveMeshStrip
	{
		public GameObject gameObject;

		public Mesh mesh;

		public MeshFilter meshFilter;

		public MeshRenderer meshRenderer;

		public Vector3[] vertices;

		public Vector2[] uvs;

		public bool divided;

		private int m_vertexIndex;

		public void Reset()
		{
			m_vertexIndex = 0;
		}

		public void AddSegment(Vector3 top, Vector3 bottom, float depth)
		{
			Vector3 vector = (top + bottom) * 0.5f;
			vector.z += depth;
			vertices[m_vertexIndex] = top;
			vertices[m_vertexIndex + 1] = vector;
			vertices[m_vertexIndex + 2] = bottom;
			m_vertexIndex += 3;
		}

		public void AddSegment(Vector3 top, Vector3 bottom)
		{
			vertices[m_vertexIndex] = top;
			vertices[m_vertexIndex + 1] = bottom;
			m_vertexIndex += 2;
		}

		public void Enable()
		{
			Vector3 top = vertices[m_vertexIndex - 2];
			Vector3 bottom = vertices[m_vertexIndex - 1];
			if (divided)
			{
				while (m_vertexIndex < vertices.Length)
				{
					AddSegment(top, bottom, 0f);
				}
			}
			else
			{
				while (m_vertexIndex < vertices.Length)
				{
					AddSegment(top, bottom);
				}
			}
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.RecalculateBounds();
			meshRenderer.enabled = true;
		}

		public void Disable()
		{
			if ((bool)meshRenderer)
			{
				meshRenderer.enabled = false;
			}
		}
	}

	public TerrainLayer terrainLayer;

	public float segmentWidth = 1f;

	public float edgeHeight = 0.1f;

	public float minSegmentHeight = 4f;

	public float maxSegmentHeight = 10f;

	public int maxSegments = 200;

	public int entranceSegments = 10;

	public int entranceCurveSegments = 50;

	public int exitSegments = 4;

	public int exitCurveSegments = 20;

	public int truncatedExitCurveSegments = 2;

	public int spawnAvoidanceBufferSegments = 3;

	public float surfaceDepth = -5f;

	public float tunnelHeight = 10f;

	public float tunnelExtraHeight = 3f;

	public float segmentsPerCeilingCurve = 10f;

	public float tunnelDepth = 1f;

	public float tunnelOverlap = 0.5f;

	public float detailHeight = 2f;

	public float maxSlope = 45f;

	public Material tunnelMaterial;

	public Material detailMaterial;

	public Material surfaceMaterial;

	public Material edgeMaterial;

	public int tunnelTextureScale = 20;

	public int detailTextureScale = 10;

	public int surfaceTextureScale = 10;

	public int edgeTextureScale = 10;

	public SpawnParams groundSpawnParams;

	public SpawnParams ceilingSpawnParams;

	private CaveMeshStrip m_tunnelMeshStrip;

	private CaveMeshStrip m_detailMeshStrip;

	private CaveMeshStrip m_surfaceMeshStrip;

	private CaveMeshStrip m_edgeMeshStrip;

	private Vector3[] m_topPoints;

	private Vector3[] m_bottomPoints;

	private Vector3[] m_groundPoints;

	private int m_pointCount;

	private int m_currentSegmentIndex;

	private int m_currentMaxSegments;

	private int m_currentExitCurveSegments;

	private Vector3 m_currentPos;

	private bool m_generated;

	private Vector2 m_boundsMin;

	private Vector2 m_boundsMax;

	private Vector2 m_pointsBoundsMin;

	private Vector2 m_pointsBoundsMax;

	public Vector2 BoundsMin
	{
		get
		{
			return m_boundsMin;
		}
	}

	public Vector2 BoundsMax
	{
		get
		{
			return m_boundsMax;
		}
	}

	public int PointCount
	{
		get
		{
			return m_pointCount;
		}
	}

	public Vector3[] TopPoints
	{
		get
		{
			return m_topPoints;
		}
	}

	public Vector3[] BottomPoints
	{
		get
		{
			return m_bottomPoints;
		}
	}

	public Vector2 PointsBoundsMin
	{
		get
		{
			return m_pointsBoundsMin;
		}
	}

	public Vector2 PointsBoundsMax
	{
		get
		{
			return m_pointsBoundsMax;
		}
	}

	private void Generate()
	{
		if (m_generated)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		float height = 0f;
		Vector3 normal = Vector3.up;
		while (terrainForLayer.GetHeightAndNormal(m_currentPos.x, ref height, ref normal))
		{
			int num = m_currentSegmentIndex * 3;
			float num2 = 1f / normal.y;
			float num3 = 0f;
			float num4 = tunnelHeight * num2;
			if (m_currentSegmentIndex > entranceSegments)
			{
				float num5 = (float)(m_currentSegmentIndex - entranceSegments) / segmentsPerCeilingCurve;
				float num6 = Mathf.Sin(num5 * (float)Math.PI * 0.5f) * tunnelExtraHeight * num2;
				num4 += num6;
			}
			float num7 = Mathf.Max(minSegmentHeight, maxSegmentHeight - (num4 - tunnelHeight));
			if (m_currentSegmentIndex > entranceSegments + entranceCurveSegments && m_currentSegmentIndex < m_currentMaxSegments - (exitSegments + m_currentExitCurveSegments))
			{
				float f = Vector3.Angle(Vector3.up, normal);
				if (Mathf.Abs(f) > maxSlope)
				{
					m_currentExitCurveSegments = truncatedExitCurveSegments;
					m_currentMaxSegments = m_currentSegmentIndex + exitSegments + m_currentExitCurveSegments;
				}
			}
			float num8 = 0f;
			if (m_currentSegmentIndex <= entranceSegments)
			{
				float f2 = (float)m_currentSegmentIndex / (float)entranceSegments;
				num3 = num4 * Mathf.Pow(f2, 1.25f);
			}
			else if (m_currentSegmentIndex >= m_currentMaxSegments - exitSegments)
			{
				float f3 = (float)(m_currentMaxSegments - m_currentSegmentIndex) / (float)exitSegments;
				num3 = num4 * Mathf.Pow(f3, 2f);
			}
			else
			{
				if (m_currentSegmentIndex <= entranceSegments + entranceCurveSegments)
				{
					float num9 = (float)(m_currentSegmentIndex - entranceSegments) / (float)entranceCurveSegments;
					float num10 = Mathf.Sin(num9 * (float)Math.PI * 0.5f);
					num8 = num7 * num10 * Mathf.Lerp(num2, 1f, num9);
				}
				else if (m_currentSegmentIndex >= m_currentMaxSegments - (exitSegments + m_currentExitCurveSegments))
				{
					float num11 = 1f - (float)(m_currentSegmentIndex - (m_currentMaxSegments - (exitSegments + m_currentExitCurveSegments))) / (float)m_currentExitCurveSegments;
					float num12 = Mathf.Sin(num11 * (float)Math.PI * 0.5f);
					num8 = num7 * num12;
				}
				else
				{
					num8 = num7;
				}
				num3 = num4 + num8;
			}
			m_currentPos.y = height + num3;
			m_boundsMin = Vector2.Min(m_boundsMin, m_currentPos);
			m_boundsMax = Vector2.Max(m_boundsMax, m_currentPos);
			Vector3 vector = m_currentPos - position;
			Vector3 vector2 = vector;
			vector2.y -= num3;
			Vector3 bottom = vector2;
			bottom.y -= tunnelOverlap;
			bottom.z += tunnelDepth;
			if (m_currentSegmentIndex < entranceSegments || m_currentSegmentIndex > m_currentMaxSegments - exitSegments)
			{
				Vector3 top = vector;
				if (m_currentSegmentIndex > 0 && m_currentSegmentIndex < m_currentMaxSegments)
				{
					top.z += tunnelDepth;
				}
				float num13 = num3 / num4;
				m_tunnelMeshStrip.AddSegment(top, bottom, 0f);
				m_tunnelMeshStrip.uvs[num].y = num13;
				m_tunnelMeshStrip.uvs[num + 1].y = num13 * 0.5f;
			}
			else
			{
				Vector3 vector3 = vector2;
				vector3.y += num4 + Mathf.Min(tunnelOverlap, num3 - num4);
				m_tunnelMeshStrip.AddSegment(vector3, bottom, tunnelDepth * 0.5f);
				m_tunnelMeshStrip.uvs[num].y = 1f;
				m_tunnelMeshStrip.uvs[num + 1].y = 0.5f;
				m_detailMeshStrip.AddSegment(vector3, vector3 - new Vector3(0f, detailHeight * num2, 0f));
				m_surfaceMeshStrip.AddSegment(vector, vector3, surfaceDepth * (num8 / num7));
				m_edgeMeshStrip.AddSegment(vector + new Vector3(0f, edgeHeight * num2, 0f), vector);
				m_topPoints[m_pointCount] = m_currentPos;
				m_bottomPoints[m_pointCount] = new Vector3(m_currentPos.x, height + num4, m_currentPos.z);
				m_groundPoints[m_pointCount] = new Vector3(m_currentPos.x, height, m_currentPos.z);
				m_pointsBoundsMin = Vector2.Min(m_pointsBoundsMin, Vector3.Min(m_topPoints[m_pointCount], m_bottomPoints[m_pointCount]));
				m_pointsBoundsMax = Vector2.Max(m_pointsBoundsMax, Vector3.Max(m_topPoints[m_pointCount], m_bottomPoints[m_pointCount]));
				m_pointCount++;
			}
			m_currentSegmentIndex++;
			if (m_currentSegmentIndex > m_currentMaxSegments)
			{
				FinishGenerate();
				break;
			}
			m_currentPos.x += segmentWidth;
		}
	}

	private void FinishGenerate()
	{
		int num = 0;
		float num2 = 0f;
		float num3 = Mathf.Max(1f, Mathf.Floor((float)m_currentMaxSegments / (float)tunnelTextureScale));
		float num4 = num3 / (float)m_currentMaxSegments;
		for (int i = 0; i <= m_currentMaxSegments; i++)
		{
			m_tunnelMeshStrip.uvs[num++].x = num2;
			m_tunnelMeshStrip.uvs[num++].x = num2;
			m_tunnelMeshStrip.uvs[num++].x = num2;
			num2 += num4;
		}
		m_generated = true;
		m_tunnelMeshStrip.Enable();
		m_detailMeshStrip.Enable();
		m_surfaceMeshStrip.Enable();
		m_edgeMeshStrip.Enable();
		SpawnObjects(ceilingSpawnParams, m_bottomPoints);
		SpawnObjects(groundSpawnParams, m_groundPoints);
		Vector3 position = base.transform.position;
		Vector3 endPos = m_groundPoints[spawnAvoidanceBufferSegments];
		SpawnManager.ForegroundInstance.AddSpawnAvoidance(position, endPos);
		Vector3 startPos = m_groundPoints[m_currentMaxSegments - (exitSegments + entranceSegments + spawnAvoidanceBufferSegments)];
		Vector3 currentPos = m_currentPos;
		SpawnManager.ForegroundInstance.AddSpawnAvoidance(startPos, currentPos);
	}

	private void SpawnObjects(SpawnParams spawnParams, Vector3[] points)
	{
		if (!spawnParams)
		{
			return;
		}
		int num = Mathf.CeilToInt((float)spawnParams.leftClearance / segmentWidth);
		int num2 = num + Mathf.RoundToInt(UnityEngine.Random.Range(0f, spawnParams.maxSeparation) / segmentWidth);
		int num3 = m_pointCount - num;
		while (num2 < num3)
		{
			Vector3 vector = points[num2 - 1];
			Vector3 vector2 = points[num2 + 1];
			Vector3 vector3 = new Vector3(vector.y - vector2.y, vector2.x - vector.x, 0f);
			float num4 = Vector3.Angle(Vector3.up, vector3) * Mathf.Sign(vector3.x);
			if (num4 >= spawnParams.minSlope && num4 <= spawnParams.maxSlope)
			{
				Quaternion rotation = ((!spawnParams.matchSlopeRotation) ? Quaternion.identity : Quaternion.LookRotation(Vector3.forward, vector3));
				SpawnManager.ForegroundInstance.ManualSpawn(spawnParams, points[num2], rotation, SpawnManager.SpawnFlags.None);
				num2 += Mathf.RoundToInt(UnityEngine.Random.Range(spawnParams.minSeparation, spawnParams.maxSeparation) / segmentWidth);
			}
			else
			{
				num2++;
			}
		}
	}

	private CaveMeshStrip CreateMeshStrip(string meshName, int segments, Material material, float textureScale)
	{
		CaveMeshStrip caveMeshStrip = new CaveMeshStrip();
		caveMeshStrip.gameObject = new GameObject(meshName);
		caveMeshStrip.gameObject.transform.position = base.transform.position;
		caveMeshStrip.gameObject.transform.parent = base.transform;
		caveMeshStrip.mesh = new Mesh();
		caveMeshStrip.meshFilter = caveMeshStrip.gameObject.AddComponent<MeshFilter>();
		caveMeshStrip.meshFilter.mesh = caveMeshStrip.mesh;
		caveMeshStrip.meshRenderer = caveMeshStrip.gameObject.AddComponent<MeshRenderer>();
		caveMeshStrip.meshRenderer.material = material;
		caveMeshStrip.meshRenderer.enabled = false;
		caveMeshStrip.divided = false;
		Vector3[] array = new Vector3[(segments + 1) * 2];
		int[] array2 = new int[(array.Length - 2) * 3];
		Vector2[] array3 = new Vector2[array.Length];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		float num4 = 0f;
		float num5 = 1f / textureScale;
		for (int i = 0; i < segments; i++)
		{
			array2[num2++] = num;
			array2[num2++] = num + 2;
			array2[num2++] = num + 1;
			array2[num2++] = num + 1;
			array2[num2++] = num + 2;
			array2[num2++] = num + 3;
			array3[num3++] = new Vector2(num4, 1f);
			array3[num3++] = new Vector2(num4, 0f);
			num4 += num5;
			if (num4 > 1f)
			{
				num4 = 1f - (num4 - 1f);
			}
			array[num++] = Vector3.zero;
			array[num++] = Vector3.zero;
		}
		array3[num3++] = new Vector2(num4, 1f);
		array3[num3++] = new Vector2(num4, 0f);
		array[num++] = Vector3.zero;
		array[num++] = Vector3.zero;
		caveMeshStrip.mesh.vertices = array;
		caveMeshStrip.mesh.triangles = array2;
		caveMeshStrip.mesh.uv = array3;
		caveMeshStrip.vertices = array;
		caveMeshStrip.uvs = array3;
		return caveMeshStrip;
	}

	private CaveMeshStrip CreateDividedMeshStrip(string meshName, int segments, Material material, float textureScale)
	{
		CaveMeshStrip caveMeshStrip = new CaveMeshStrip();
		caveMeshStrip.gameObject = new GameObject(meshName);
		caveMeshStrip.gameObject.transform.position = base.transform.position;
		caveMeshStrip.gameObject.transform.parent = base.transform;
		caveMeshStrip.mesh = new Mesh();
		caveMeshStrip.meshFilter = caveMeshStrip.gameObject.AddComponent<MeshFilter>();
		caveMeshStrip.meshFilter.mesh = caveMeshStrip.mesh;
		caveMeshStrip.meshRenderer = caveMeshStrip.gameObject.AddComponent<MeshRenderer>();
		caveMeshStrip.meshRenderer.material = material;
		caveMeshStrip.meshRenderer.enabled = false;
		caveMeshStrip.divided = true;
		Vector3[] array = new Vector3[(segments + 1) * 3];
		int[] array2 = new int[(array.Length - 3) * 4];
		Vector2[] array3 = new Vector2[array.Length];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		float num4 = 0f;
		float num5 = 1f / textureScale;
		for (int i = 0; i < segments; i++)
		{
			array2[num2++] = num;
			array2[num2++] = num + 3;
			array2[num2++] = num + 1;
			array2[num2++] = num + 1;
			array2[num2++] = num + 3;
			array2[num2++] = num + 4;
			array2[num2++] = num + 1;
			array2[num2++] = num + 4;
			array2[num2++] = num + 2;
			array2[num2++] = num + 2;
			array2[num2++] = num + 4;
			array2[num2++] = num + 5;
			array3[num3++] = new Vector2(num4, 1f);
			array3[num3++] = new Vector2(num4, 0.5f);
			array3[num3++] = new Vector2(num4, 0f);
			num4 += num5;
			if (num4 > 1f)
			{
				num4 = 1f - (num4 - 1f);
			}
			array[num++] = Vector3.zero;
			array[num++] = Vector3.zero;
			array[num++] = Vector3.zero;
		}
		array3[num3++] = new Vector2(num4, 1f);
		array3[num3++] = new Vector2(num4, 0.5f);
		array3[num3++] = new Vector2(num4, 0f);
		array[num++] = Vector3.zero;
		array[num++] = Vector3.zero;
		array[num++] = Vector3.zero;
		caveMeshStrip.mesh.vertices = array;
		caveMeshStrip.mesh.triangles = array2;
		caveMeshStrip.mesh.uv = array3;
		caveMeshStrip.vertices = array;
		caveMeshStrip.uvs = array3;
		return caveMeshStrip;
	}

	private void Awake()
	{
		int num = maxSegments - entranceSegments - exitSegments;
		m_tunnelMeshStrip = CreateDividedMeshStrip("Tunnel", maxSegments, tunnelMaterial, maxSegments);
		m_detailMeshStrip = CreateMeshStrip("Detail", num, detailMaterial, detailTextureScale);
		m_surfaceMeshStrip = CreateDividedMeshStrip("Surface", num, surfaceMaterial, surfaceTextureScale);
		m_edgeMeshStrip = CreateMeshStrip("Edge", num, edgeMaterial, edgeTextureScale);
		m_topPoints = new Vector3[num + 1];
		m_bottomPoints = new Vector3[num + 1];
		m_groundPoints = new Vector3[num + 1];
	}

	private void OnEnable()
	{
		m_currentPos = base.transform.position;
		m_pointCount = 0;
		m_currentSegmentIndex = 0;
		m_currentMaxSegments = maxSegments;
		m_currentExitCurveSegments = exitCurveSegments;
		m_generated = false;
		m_boundsMin = (m_pointsBoundsMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		m_boundsMax = (m_pointsBoundsMax = new Vector3(float.MinValue, float.MinValue, float.MinValue));
		m_tunnelMeshStrip.Reset();
		m_detailMeshStrip.Reset();
		m_surfaceMeshStrip.Reset();
		m_edgeMeshStrip.Reset();
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if ((bool)terrainForLayer)
		{
			terrainForLayer.AddCave(this);
		}
	}

	private void OnDisable()
	{
		m_tunnelMeshStrip.Disable();
		m_detailMeshStrip.Disable();
		m_surfaceMeshStrip.Disable();
		m_edgeMeshStrip.Disable();
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if ((bool)terrainForLayer)
		{
			terrainForLayer.RemoveCave(this);
		}
	}

	private void Update()
	{
		Generate();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		for (int i = 0; i < m_pointCount - 1; i++)
		{
			Gizmos.DrawLine(m_topPoints[i], m_topPoints[i + 1]);
			Gizmos.DrawLine(m_bottomPoints[i], m_bottomPoints[i + 1]);
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(Vector2.Lerp(m_pointsBoundsMin, m_pointsBoundsMax, 0.5f), m_pointsBoundsMax - m_pointsBoundsMin);
	}
}
