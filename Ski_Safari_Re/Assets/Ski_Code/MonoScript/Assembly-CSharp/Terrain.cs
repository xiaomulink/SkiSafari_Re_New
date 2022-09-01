using System;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
	private struct CurveInfo
	{
		public Vector3 startPos;

		public float height;

		public float length;

		public float offset;

		public float segmentWidth;

		public int segmentCount;

		public int startSegmentIndex;

		public int currentRelativeSegmentIndex;

		public TerrainCurveParams curveParams;

		public Vector3 CalcWorldPos()
		{
			float f = offset + (float)currentRelativeSegmentIndex / (float)segmentCount * length;
			return new Vector3(startPos.x + segmentWidth * (float)currentRelativeSegmentIndex, startPos.y + (Mathf.Sin(f) * 0.5f - 0.5f) * (0f - height), startPos.z);
		}
	}

	[Serializable]
	public class CurveMarker
	{
		public int relativeSegmentIndex;

		public int segmentCount;

		public TerrainCurveParams curveParams;
	}

	[Serializable]
	public class ChunkInfo
	{
		public GameObject gameObject;

		public Mesh mesh;

		public Mesh edgeMesh;

		public Vector3[] points;

		public Vector3[] normals;

		public int startSegmentIndex;

		public int endSegmentIndex;

		public List<CurveMarker> curveMarkers = new List<CurveMarker>();

		public ChunkInfo(GameObject gameObject, Mesh mesh, Mesh edgeMesh, Vector3[] points, Vector3[] normals)
		{
			this.gameObject = gameObject;
			this.mesh = mesh;
			this.edgeMesh = edgeMesh;
			this.points = points;
			this.normals = normals;
            gameObject.AddComponent<TerrainChunk>();

            gameObject.GetComponent<TerrainChunk>().mesh = mesh;
            gameObject.GetComponent<TerrainChunk>().edgeMesh = edgeMesh;
            gameObject.GetComponent<TerrainChunk>().points = points;
            gameObject.GetComponent<TerrainChunk>().normals = normals;
        }
       
	}

	public class CurveChecker
	{
		private Terrain m_terrain;

		private int m_chunkIndex;

		private int m_curveMarkerIndex;

		public CurveChecker(Terrain terrain, int chunkIndex, int curveMarkerIndex)
		{
			m_terrain = terrain;
			m_chunkIndex = chunkIndex;
			m_curveMarkerIndex = curveMarkerIndex;
		}

		public bool ContainsAnyCurves(List<string> curveNames, int startPosX, int distanceX)
		{
			if (distanceX == 0)
			{
				string curveName = m_terrain.Chunks[m_chunkIndex].curveMarkers[m_curveMarkerIndex].curveParams.curveName;
				if (curveNames.Contains(curveName))
				{
					return true;
				}
				return false;
			}
			int num = startPosX + distanceX;
			int num2 = Mathf.FloorToInt((float)num / m_terrain.segmentWidth);
			int num3 = m_chunkIndex;
			int num4 = m_curveMarkerIndex;
			ChunkInfo chunkInfo = m_terrain.Chunks[num3];
			int num5 = chunkInfo.startSegmentIndex + chunkInfo.curveMarkers[num4].relativeSegmentIndex;
			do
			{
				if (curveNames.Contains(m_terrain.Chunks[num3].curveMarkers[num4].curveParams.curveName))
				{
					return true;
				}
				if (++num4 >= m_terrain.Chunks[num3].curveMarkers.Count)
				{
					if (++num3 >= m_terrain.Chunks.Count)
					{
						return false;
					}
					chunkInfo = m_terrain.Chunks[num3];
					num4 = 0;
				}
				num5 = chunkInfo.startSegmentIndex + chunkInfo.curveMarkers[num4].relativeSegmentIndex;
			}
			while (num2 >= num5);
			return false;
		}
	}

	private struct CollisionTestResult
	{
		public Vector3 position;

		public float radius;

		public Vector3 velocity;

		public float deltaTime;

		public int segmentIndexMin;

		public int segmentIndexMax;

		public int chunkIndex;

		public int chunkIndexOffset;

		public GeometryUtils.ContactInfo[] segmentContacts;

		public int segmentContactCount;

		public bool hasLineContact;
	}

	public TerrainLayer terrainLayer;

	public TerrainLayer matchTerrainLayer;

	public float initialHeight;

	public int lookAheadSegmentCount = 100;

	public int cullBehindSegmentCount = 50;

	public float segmentWidth = 1f;

	public float edgeHeight = 0.1f;

	public float surfaceHeight = 5f;

	public float surfaceDepth = 5f;

	public float segmentHeight = 10f;

	public float segmentDepth = 10f;

	public int segmentsPerChunk = 20;

	public int maxSegmentContacts = 4;

	public Material edgeMaterial;

	public Material material;

	public int textureScale = 10;

	public float surfaceTextureRatio = 0.5f;

	public TerrainCurveParams initialCurveParams;

	public TerrainCurveParams[] curveParamsList;

	private CurveInfo m_currentCurveInfo;

	private TerrainCurveParams m_curveParams;

	private int m_initialPosX;

	private int m_curveParamsStartPosX;

	private List<ChunkInfo> m_pooledChunks = new List<ChunkInfo>();

	public List<ChunkInfo> m_chunks = new List<ChunkInfo>();

	private int m_chunkIndexOffset;

	private List<TerrainCurveParams> m_curveParamsQueue = new List<TerrainCurveParams>();

	private GeometryUtils.ContactInfo[] m_segmentContacts;

	private List<LineCollider> m_lineColliders = new List<LineCollider>();

	private List<LineCollider> m_terrainLineColliders = new List<LineCollider>();

	private List<Cave> m_caves = new List<Cave>();

	private static Terrain[] s_terrainInstance = new Terrain[4];

	private CollisionTestResult m_lastCollisionTest;

	public TerrainLayer MatchTerrainLayer
	{
		get
		{
			return matchTerrainLayer;
		}
	}

	public List<ChunkInfo> Chunks
	{
		get
		{
			return m_chunks;
		}
	}

	public static Terrain GetTerrainForLayer(TerrainLayer terrainLayer)
	{
		return s_terrainInstance[(int)terrainLayer];
	}

	public bool HasCurve(string curveName)
	{
		TerrainCurveParams[] array = curveParamsList;
		foreach (TerrainCurveParams terrainCurveParams in array)
		{
			if (terrainCurveParams.curveName == curveName)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPositionInBounds(float posX)
	{
		if (m_chunks.Count == 0)
		{
			return false;
		}
		float f = posX / segmentWidth;
		int num = Mathf.FloorToInt(f);
		return num >= m_chunks[0].startSegmentIndex;
	}

	public TerrainType GetTerrainType(float posX)
	{
		for (int i = 0; i < m_caves.Count; i++)
		{
			Cave cave = m_caves[i];
			if (posX >= cave.BoundsMin.x && posX < cave.BoundsMax.x)
			{
				if (posX >= cave.PointsBoundsMin.x && posX < cave.PointsBoundsMax.x)
				{
					return TerrainType.Cave;
				}
				return (TerrainType)3;
			}
		}
		return TerrainType.Terrain;
	}

	private bool FindLineInterceptionAtPos(float posX, out Vector3 interceptPos, out Vector3 interceptNormal, bool calculateNormal)
	{
		for (int i = 0; i < m_caves.Count; i++)
		{
			Cave cave = m_caves[i];
			if (posX >= cave.PointsBoundsMin.x && posX < cave.PointsBoundsMax.x)
			{
				float num = (posX - cave.TopPoints[0].x) / cave.segmentWidth;
				int num2 = Mathf.FloorToInt(num);
				Vector3 vector = cave.TopPoints[num2];
				Vector3 vector2 = cave.TopPoints[num2 + 1];
				float t = num - (float)num2;
				interceptPos = Vector3.Lerp(vector, vector2, t);
				if (calculateNormal)
				{
					GeometryUtils.CalculateNormal(vector, vector2, out interceptNormal);
				}
				else
				{
					interceptNormal = Vector3.zero;
				}
				return true;
			}
		}
		for (int j = 0; j < m_terrainLineColliders.Count; j++)
		{
			LineCollider lineCollider = m_terrainLineColliders[j];
			if (lineCollider == null)
			{
				continue;
			}
			for (int k = 0; k < lineCollider.Lines.Count; k++)
			{
				LineCollider.Line line = lineCollider.Lines[k];
				float x = line.pointA.x;
				float x2 = line.pointB.x;
				if (posX >= x && posX <= x2 && x < x2)
				{
					float t2 = (posX - x) / (x2 - x);
					interceptPos = Vector3.Lerp(line.pointA, line.pointB, t2);
					if (calculateNormal)
					{
						GeometryUtils.CalculateNormal(line.pointA, line.pointB, out interceptNormal);
					}
					else
					{
						interceptNormal = Vector3.zero;
					}
					return true;
				}
			}
		}
		float num3 = posX / segmentWidth;
		int num4 = Mathf.FloorToInt(num3);
		for (int l = 0; l < m_chunks.Count; l++)
		{
			ChunkInfo chunkInfo = m_chunks[l];
			if (num4 < chunkInfo.endSegmentIndex)
			{
				if (num4 < chunkInfo.startSegmentIndex)
				{
					num4 = chunkInfo.startSegmentIndex;
					num3 = num4;
				}
				int num5 = num4 - chunkInfo.startSegmentIndex;
				float t3 = num3 - (float)num4;
				interceptPos = Vector3.Lerp(chunkInfo.points[num5], chunkInfo.points[num5 + 1], t3);
				if (calculateNormal)
				{
					interceptNormal = Vector3.Lerp(chunkInfo.normals[num5], chunkInfo.normals[num5 + 1], t3);
				}
				else
				{
					interceptNormal = Vector3.zero;
				}
				return true;
			}
		}
		interceptPos = Vector3.zero;
		interceptNormal = Vector3.zero;
		return false;
	}

	private bool FindLineInterceptionAtPos(Vector3 pos, out Vector3 interceptPos, out Vector3 interceptNormal, bool calculateNormal)
	{
		float x = pos.x;
		for (int i = 0; i < m_caves.Count; i++)
		{
			Cave cave = m_caves[i];
			if (!(x >= cave.PointsBoundsMin.x) || !(x < cave.PointsBoundsMax.x))
			{
				continue;
			}
			float num = (x - cave.TopPoints[0].x) / cave.segmentWidth;
			int num2 = Mathf.FloorToInt(num);
			if (!(pos.y <= (cave.TopPoints[num2].y + cave.BottomPoints[num2].y) * 0.5f))
			{
				Vector3 vector = cave.TopPoints[num2];
				Vector3 vector2 = cave.TopPoints[num2 + 1];
				float t = num - (float)num2;
				interceptPos = Vector3.Lerp(vector, vector2, t);
				if (calculateNormal)
				{
					GeometryUtils.CalculateNormal(vector, vector2, out interceptNormal);
				}
				else
				{
					interceptNormal = Vector3.zero;
				}
				return true;
			}
		}
		for (int j = 0; j < m_terrainLineColliders.Count; j++)
		{
			LineCollider lineCollider = m_terrainLineColliders[j];
			for (int k = 0; k < lineCollider.Lines.Count; k++)
			{
				LineCollider.Line line = lineCollider.Lines[k];
				float x2 = line.pointA.x;
				float x3 = line.pointB.x;
				if (x >= x2 && x <= x3 && x2 < x3)
				{
					float t2 = (x - x2) / (x3 - x2);
					interceptPos = Vector3.Lerp(line.pointA, line.pointB, t2);
					if (calculateNormal)
					{
						GeometryUtils.CalculateNormal(line.pointA, line.pointB, out interceptNormal);
					}
					else
					{
						interceptNormal = Vector3.zero;
					}
					return true;
				}
			}
		}
		float num3 = x / segmentWidth;
		int num4 = Mathf.FloorToInt(num3);
		for (int l = 0; l < m_chunks.Count; l++)
		{
			ChunkInfo chunkInfo = m_chunks[l];
			if (num4 < chunkInfo.endSegmentIndex)
			{
				if (num4 < chunkInfo.startSegmentIndex)
				{
					num4 = chunkInfo.startSegmentIndex;
					num3 = num4;
				}
				int num5 = num4 - chunkInfo.startSegmentIndex;
				float t3 = num3 - (float)num4;
				interceptPos = Vector3.Lerp(chunkInfo.points[num5], chunkInfo.points[num5 + 1], t3);
				if (calculateNormal)
				{
					interceptNormal = Vector3.Lerp(chunkInfo.normals[num5], chunkInfo.normals[num5 + 1], t3);
					interceptNormal.Normalize();
				}
				else
				{
					interceptNormal = Vector3.zero;
				}
				return true;
			}
		}
		interceptPos = Vector3.zero;
		interceptNormal = Vector3.zero;
		return false;
	}

	public bool GetHeight(float posX, ref float height)
	{
		Vector3 interceptPos;
		Vector3 interceptNormal;
		if (FindLineInterceptionAtPos(posX, out interceptPos, out interceptNormal, false))
		{
			height = interceptPos.y;
			return true;
		}
		return false;
	}

	public bool GetHeightAndNormal(float posX, ref float height, ref Vector3 normal)
	{
		Vector3 interceptPos;
		if (FindLineInterceptionAtPos(posX, out interceptPos, out normal, true))
		{
			height = interceptPos.y;
			return true;
		}
		return false;
	}

	public bool GetHeightAndNormal(Vector3 pos, ref float height, ref Vector3 normal)
	{
		Vector3 interceptPos;
		if (FindLineInterceptionAtPos(pos, out interceptPos, out normal, true))
		{
			height = interceptPos.y;
			return true;
		}
		return false;
	}

	public bool GetCameraHeight(Vector3 pos, ref float height)
	{
		float num = pos.x / segmentWidth;
		int num2 = Mathf.FloorToInt(num);
		for (int i = 0; i < m_chunks.Count; i++)
		{
			ChunkInfo chunkInfo = m_chunks[i];
			if (num2 < chunkInfo.endSegmentIndex)
			{
				if (num2 < chunkInfo.startSegmentIndex)
				{
					return true;
				}
				int num3 = num2 - chunkInfo.startSegmentIndex;
				Vector3 a = chunkInfo.points[0];
				Vector3 b = chunkInfo.points[chunkInfo.points.Length - 1];
				float t = ((float)num3 + (num - (float)num2)) / (float)(chunkInfo.points.Length - 1);
				height = pos.y - Vector3.Lerp(a, b, t).y;
				return true;
			}
		}
		return false;
	}

	public bool IsPointInCave(Vector3 pos)
	{
		for (int i = 0; i < m_caves.Count; i++)
		{
			Cave cave = m_caves[i];
			if (pos.x >= cave.PointsBoundsMin.x && pos.x < cave.PointsBoundsMax.x)
			{
				int num = Mathf.FloorToInt((pos.x - cave.TopPoints[0].x) / cave.segmentWidth);
				int num2 = num + 1;
				return pos.y <= cave.BottomPoints[num].y || pos.y <= cave.BottomPoints[num2].y;
			}
		}
		return false;
	}

	public bool IsPointAboveCave(Vector3 pos)
	{
		for (int i = 0; i < m_caves.Count; i++)
		{
			Cave cave = m_caves[i];
			if (pos.x >= cave.PointsBoundsMin.x && pos.x < cave.PointsBoundsMax.x)
			{
				int num = Mathf.FloorToInt((pos.x - cave.TopPoints[0].x) / cave.segmentWidth);
				int num2 = num + 1;
				return pos.y >= cave.TopPoints[num].y || pos.y >= cave.TopPoints[num2].y;
			}
		}
		return false;
	}

	public bool IsPositionInOrAboveCave(int posX)
	{
		for (int i = 0; i < m_caves.Count; i++)
		{
			Cave cave = m_caves[i];
			if ((float)posX >= cave.PointsBoundsMin.x && (float)posX < cave.PointsBoundsMax.x)
			{
				return true;
			}
		}
		return false;
	}

	public string GetCurveName(float posX)
	{
		float f = posX / segmentWidth;
		int num = Mathf.FloorToInt(f);
		for (int i = 0; i < m_chunks.Count; i++)
		{
			ChunkInfo chunkInfo = m_chunks[i];
			if (num >= chunkInfo.endSegmentIndex)
			{
				continue;
			}
			if (num < chunkInfo.startSegmentIndex)
			{
				return string.Empty;
			}
			if (num == 0)
			{
				return chunkInfo.curveMarkers[0].curveParams.curveName;
			}
			int num2 = num - chunkInfo.startSegmentIndex;
			for (int j = 1; j < chunkInfo.curveMarkers.Count; j++)
			{
				if (num2 < chunkInfo.curveMarkers[j].relativeSegmentIndex)
				{
					return chunkInfo.curveMarkers[j - 1].curveParams.curveName;
				}
			}
			return chunkInfo.curveMarkers[chunkInfo.curveMarkers.Count - 1].curveParams.curveName;
		}
		return string.Empty;
	}

	public CurveChecker GetCurveChecker(int posX)
	{
		float f = (float)posX / segmentWidth;
		int num = Mathf.FloorToInt(f);
		for (int i = 0; i < m_chunks.Count; i++)
		{
			ChunkInfo chunkInfo = m_chunks[i];
			if (num >= chunkInfo.endSegmentIndex)
			{
				continue;
			}
			if (num < chunkInfo.startSegmentIndex)
			{
				return null;
			}
			if (num == 0)
			{
				return new CurveChecker(this, i, 0);
			}
			int num2 = num - chunkInfo.startSegmentIndex;
			for (int j = 1; j < chunkInfo.curveMarkers.Count; j++)
			{
				if (num2 < chunkInfo.curveMarkers[j].relativeSegmentIndex)
				{
					return new CurveChecker(this, i, j - 1);
				}
			}
			return new CurveChecker(this, i, chunkInfo.curveMarkers.Count - 1);
		}
		return null;
	}

	public void AddStaticLineCollider(LineCollider lineCollider)
	{
		m_lineColliders.Add(lineCollider);
		if (lineCollider.isTerrain)
		{
			m_terrainLineColliders.Add(lineCollider);
		}
	}

	public void RemoveStaticLineCollider(LineCollider lineCollider)
	{
		m_lineColliders.Remove(lineCollider);
		if (lineCollider.isTerrain)
		{
			m_terrainLineColliders.Remove(lineCollider);
		}
	}

	public void AddCave(Cave cave)
	{
		m_caves.Add(cave);
	}

	public void RemoveCave(Cave cave)
	{
		m_caves.Remove(cave);
	}

	private bool SnapToSurface(Vector3 position, float radius, Vector3 velocity, ref GeometryUtils.ContactInfo contactInfo)
	{
		float height = 0f;
		Vector3 normal = Vector3.up;
		if (GetHeightAndNormal(position, ref height, ref normal))
		{
			float num = position.y - radius;
			if (height > num && Vector3.Dot(velocity, normal) < 0f)
			{
				float num2 = radius * (1f / normal.y);
				contactInfo.position = new Vector3(position.x, height + num2, position.z);
				contactInfo.normal = normal;
				contactInfo.distance = height - num;
				return true;
			}
		}
		return false;
	}

	public bool TestCircleCollision(Vector3 position, float radius, Vector3 velocity, float deltaTime, int layer, ref GeometryUtils.ContactInfo contactInfo, string name = "")
	{

		if (m_chunks.Count == 0)
		{
			return false;
		}
		float magnitude = velocity.magnitude;
		if (magnitude <= Mathf.Epsilon)
		{
			return false;
		}
		if (!CameraUtils.IsPointVisible(position))
		{
			if (SnapToSurface(position, radius, velocity, ref contactInfo))
			{
				contactInfo.isOffScreen = true;
				return true;
			}
			return false;
		}
		Vector3 vector = velocity / magnitude;
		float num = magnitude * deltaTime;
		Vector3 rhs = position + vector * num;
		Vector3 vector2 = Vector3.Min(position, rhs) - new Vector3(radius, radius, 0f);
		Vector3 vector3 = Vector3.Max(position, rhs) + new Vector3(radius, radius, 0f);
		int num2 = 0;
		m_segmentContacts[num2].Invalidate();
		CollisionTestResult lastCollisionTest = default(CollisionTestResult);
		lastCollisionTest.position = position;
		lastCollisionTest.radius = radius;
		lastCollisionTest.velocity = velocity;
		lastCollisionTest.deltaTime = deltaTime;
		lastCollisionTest.chunkIndexOffset = m_chunkIndexOffset;
		int num3 = 1 << layer;
		for (int i = 0; i < m_lineColliders.Count; i++)
		{
			LineCollider lineCollider = m_lineColliders[i];
			if (!(magnitude > lineCollider.minSpeed) || ((int)lineCollider.collisionMask & num3) == 0)
			{
				continue;
			}
			for (int j = 0; j < lineCollider.Lines.Count; j++)
			{
				LineCollider.Line line = lineCollider.Lines[j];
				if (GeometryUtils.CircleLineIntersection(position, vector, num, radius, line.pointA, line.pointB, lineCollider.allowPointCollisions, ref m_segmentContacts[num2]) && m_segmentContacts[num2].distance - Mathf.Epsilon <= contactInfo.distance)
				{
					m_segmentContacts[num2].collider = lineCollider;
					m_segmentContacts[num2].colliderLine = line;
					if (++num2 >= maxSegmentContacts)
					{
						break;
					}
					m_segmentContacts[num2].Invalidate();
				}
			}
			if (num2 >= maxSegmentContacts)
			{
				break;
			}
		}
		if (num2 < maxSegmentContacts)
		{
			for (int k = 0; k < m_caves.Count; k++)
			{
				Cave cave = m_caves[k];
				if (cave.PointCount <= 0)
				{
					continue;
				}
				int num4 = Mathf.Max(0, Mathf.FloorToInt((vector2.x - cave.TopPoints[0].x) / cave.segmentWidth));
				int num5 = Mathf.Min(cave.PointCount - 2, Mathf.CeilToInt((vector3.x - cave.TopPoints[0].x) / cave.segmentWidth));
				for (int l = num4; l <= num5; l++)
				{
					bool allowPointCollisions = l == 0;
					if (GeometryUtils.CircleLineIntersection(position, vector, num, radius, cave.TopPoints[l], cave.TopPoints[l + 1], allowPointCollisions, ref m_segmentContacts[num2]))
					{
						if (m_segmentContacts[num2].distance - Mathf.Epsilon <= contactInfo.distance)
						{
							if (++num2 >= maxSegmentContacts)
							{
								break;
							}
							m_segmentContacts[num2].Invalidate();
						}
					}
					else if (GeometryUtils.CircleLineIntersection(position, vector, num, radius, cave.BottomPoints[l + 1], cave.BottomPoints[l], allowPointCollisions, ref m_segmentContacts[num2]) && m_segmentContacts[num2].distance - Mathf.Epsilon <= contactInfo.distance)
					{
						if (++num2 >= maxSegmentContacts)
						{
							break;
						}
						m_segmentContacts[num2].Invalidate();
					}
				}
				if (num2 >= maxSegmentContacts)
				{
					break;
				}
			}
		}
		int num6 = Mathf.FloorToInt(vector2.x / segmentWidth);
		int num7 = Mathf.CeilToInt(vector3.x / segmentWidth);
		int num8 = (lastCollisionTest.chunkIndex = FindChunkForSegment(num6));
		lastCollisionTest.segmentIndexMin = num6;
		lastCollisionTest.segmentIndexMax = num7;
		if (num8 != int.MaxValue && num2 < maxSegmentContacts)
		{
			for (int m = num6; m <= num7; m++)
			{
				if (num8 >= m_chunks.Count)
				{
					break;
				}
				ChunkInfo chunkInfo = m_chunks[num8];
				int num9 = m - chunkInfo.startSegmentIndex;
				if (GeometryUtils.CircleLineIntersection(position, vector, num, radius, chunkInfo.points[num9], chunkInfo.points[num9 + 1], false, ref m_segmentContacts[num2]) && !(m_segmentContacts[num2].distance - Mathf.Epsilon > contactInfo.distance))
				{
					if (++num2 >= maxSegmentContacts)
					{
						break;
					}
					m_segmentContacts[num2].Invalidate();
				}
				if (m >= chunkInfo.endSegmentIndex - 1)
				{
					num8++;
				}
			}
		}
		lastCollisionTest.segmentContacts = m_segmentContacts;
		lastCollisionTest.segmentContactCount = num2;
		lastCollisionTest.hasLineContact = true;
		if (num2 > 0)
		{
			int num10 = int.MinValue;
			float num11 = float.MaxValue;
			for (int n = 0; n < num2; n++)
			{
				if (m_segmentContacts[n].distance < num11 || (!m_segmentContacts[n].isPoint && m_segmentContacts[num10].isPoint))
				{
					num11 = m_segmentContacts[n].distance;
					num10 = n;
				}
			}
			if (num10 != int.MinValue)
			{
				contactInfo = m_segmentContacts[num10];
			}
		}
		if (contactInfo.IsValid())
		{
			m_lastCollisionTest = lastCollisionTest;
			return true;
		}
		return SnapToSurface(position, radius, velocity, ref contactInfo);
	}

	public void Reset()
	{
		base.enabled = false;
		base.enabled = true;
	}

	private int FindChunkForSegment(int segmentIndex)
	{
		for (int i = 0; i < m_chunks.Count; i++)
		{
			ChunkInfo chunkInfo = m_chunks[i];
			if (segmentIndex < chunkInfo.endSegmentIndex)
			{
				if (segmentIndex < chunkInfo.startSegmentIndex)
				{
					return int.MaxValue;
				}
				return i;
			}
		}
		return int.MaxValue;
	}

	private void ShuffleCurveParams()
	{
		m_curveParamsQueue.Clear();
		TerrainCurveParams[] array = curveParamsList;
		foreach (TerrainCurveParams terrainCurveParams in array)
		{
			for (int j = 0; j < terrainCurveParams.occurances; j++)
			{
				m_curveParamsQueue.Add(terrainCurveParams);
			}
		}
		m_curveParamsQueue.Shuffle();
	}

	private void CheckCurveParams()
	{
		int num = Mathf.FloorToInt(m_currentCurveInfo.startPos.x - (float)m_initialPosX);
		if (!((float)(num - m_curveParamsStartPosX) >= m_curveParams.operationalDistance))
		{
			return;
		}
		if (m_curveParamsQueue.Count == 0)
		{
			ShuffleCurveParams();
		}
		bool flag = false;
		for (int i = 0; i < m_curveParamsQueue.Count; i++)
		{
			if (m_curveParamsQueue[0].CheckCriteria(num))
			{
				m_curveParams = m_curveParamsQueue[0];
				m_curveParamsQueue.RemoveAt(0);
				flag = true;
				break;
			}
			TerrainCurveParams item = m_curveParamsQueue[0];
			m_curveParamsQueue.RemoveAt(0);
			m_curveParamsQueue.Add(item);
		}
		if (flag)
		{
			m_curveParamsStartPosX = num;
		}
		else
		{
			ShuffleCurveParams();
		}
	}

	private void InitialiseCurve(Vector3 startPos, int startSegmentIndex, ref CurveInfo curveInfo)
	{
		curveInfo.startPos = startPos;
		curveInfo.length = (float)Math.PI / 180f * m_curveParams.angleLength;
		curveInfo.offset = (float)Math.PI / 180f * m_curveParams.angleOffset;
		curveInfo.segmentWidth = segmentWidth;
		curveInfo.segmentCount = UnityEngine.Random.Range(m_curveParams.segmentCountMin, m_curveParams.segmentCountMax);
		curveInfo.startSegmentIndex = startSegmentIndex;
		curveInfo.currentRelativeSegmentIndex = 0;
		float num = 0f;
		if (MatchTerrainLayer != terrainLayer)
		{
			float height = 0f;
			if (s_terrainInstance[(int)MatchTerrainLayer].GetHeight(startPos.x + curveInfo.segmentWidth * (float)curveInfo.segmentCount, ref height))
			{
				num = height - startPos.y;
			}
		}
		curveInfo.height = num + UnityEngine.Random.Range(m_curveParams.heightMin, m_curveParams.heightMax);
		curveInfo.curveParams = m_curveParams;
		if ((bool)m_curveParams.fixedSpawnParams)
		{
			SpawnManager.ForegroundInstance.ManualSpawn(m_curveParams.fixedSpawnParams, startPos, Quaternion.identity, m_curveParams.fixedSpawnFlags);
		}
		if (m_curveParams.preventSpawning)
		{
			Vector3 endPos = startPos;
			endPos.x += curveInfo.segmentWidth * (float)curveInfo.segmentCount;
			SpawnManager.ForegroundInstance.AddSpawnAvoidance(startPos, endPos);
		}
		CheckCurveParams();
	}

	private bool GenerateChunk(ref CurveInfo curveInfo)
	{
		if (m_pooledChunks.Count == 0)
		{
			return false;
		}
		if (MatchTerrainLayer != terrainLayer)
		{
			int num = curveInfo.segmentCount - curveInfo.currentRelativeSegmentIndex;
			if (num < segmentsPerChunk)
			{
				Terrain terrain = s_terrainInstance[(int)MatchTerrainLayer];
				float num2 = segmentsPerChunk - num + m_curveParams.segmentCountMax;
				float posX = curveInfo.CalcWorldPos().x + (float)Mathf.CeilToInt(num2 * segmentWidth);
				float height = 0f;
				if (!terrain.GetHeight(posX, ref height))
				{
					return false;
				}
			}
		}
		ChunkInfo chunkInfo = m_pooledChunks[0];
		m_pooledChunks.RemoveAt(0);
		Vector3[] vertices = chunkInfo.mesh.vertices;
		Vector3[] points = chunkInfo.points;
		Vector3[] normals = chunkInfo.normals;
		int num3 = 0;
		int num4 = 0;
		Vector3 vector = new Vector3(0f, surfaceHeight, surfaceDepth);
		Vector3 vector2 = new Vector3(0f, segmentHeight, segmentDepth);
		Vector3[] vertices2 = chunkInfo.edgeMesh.vertices;
		int num5 = 0;
		Vector3 vector3 = new Vector3(0f, edgeHeight, 0f);
		Vector3 vector4 = curveInfo.CalcWorldPos();
		int num6 = curveInfo.startSegmentIndex + curveInfo.currentRelativeSegmentIndex;
		chunkInfo.curveMarkers.Clear();
		chunkInfo.curveMarkers.Add(new CurveMarker
		{
			relativeSegmentIndex = 0,
			segmentCount = curveInfo.segmentCount,
			curveParams = curveInfo.curveParams
		});
		for (int i = 0; i <= segmentsPerChunk; i++)
		{
			Vector3 vector5 = curveInfo.CalcWorldPos();
			Vector3 vector6 = vector5 - vector4;
			vertices[num3++] = vector6;
			vertices[num3++] = vector6 - vector;
			vertices[num3++] = vector6 - vector2;
			vertices2[num5++] = vector6 + vector3;
			vertices2[num5++] = vector6;
			points[num4++] = vector5;
			if (i < segmentsPerChunk && ++curveInfo.currentRelativeSegmentIndex > curveInfo.segmentCount)
			{
				InitialiseCurve(vector5, curveInfo.startSegmentIndex + curveInfo.segmentCount, ref curveInfo);
				curveInfo.currentRelativeSegmentIndex = 1;
				chunkInfo.curveMarkers.Add(new CurveMarker
				{
					relativeSegmentIndex = i,
					segmentCount = curveInfo.segmentCount,
					curveParams = curveInfo.curveParams
				});
			}
		}
		int num7 = normals.Length;
		GeometryUtils.CalculateNormal(points[0], points[1], out normals[0]);
		for (int j = 1; j < num7 - 1; j++)
		{
			GeometryUtils.CalculateNormal(points[j - 1], points[j + 1], out normals[j]);
		}
		GeometryUtils.CalculateNormal(points[num7 - 2], points[num7 - 1], out normals[num7 - 1]);
		chunkInfo.gameObject.transform.position = vector4;
		chunkInfo.mesh.vertices = vertices;
		chunkInfo.mesh.RecalculateBounds();
		chunkInfo.edgeMesh.vertices = vertices2;
		chunkInfo.edgeMesh.RecalculateBounds();
		chunkInfo.startSegmentIndex = num6;
		chunkInfo.endSegmentIndex = num6 + segmentsPerChunk;
		chunkInfo.gameObject.SetActive(true);
		m_chunks.Add(chunkInfo);
		return true;
	}

	public void GenerateInitialChunks()
	{
		if (m_chunks.Count > 0)
		{
			return;
		}
		foreach (ChunkInfo pooledChunk in m_pooledChunks)
		{
			pooledChunk.gameObject.SetActive(false);
		}
		if (MatchTerrainLayer != terrainLayer)
		{
			s_terrainInstance[(int)MatchTerrainLayer].GenerateInitialChunks();
		}
		Camera main = Camera.main;
		float z = base.transform.position.z - main.transform.position.z;
		Vector3 vector = main.ViewportToWorldPoint(new Vector3(0f, 0.5f, z));
		vector.y = 0f;
		int num = Mathf.FloorToInt(vector.x / segmentWidth);
		Vector3 startPos = new Vector3((float)num * segmentWidth, vector.y + initialHeight, base.transform.position.z);
		m_initialPosX = (m_curveParamsStartPosX = Mathf.FloorToInt(startPos.x));
		m_curveParams = initialCurveParams;
		m_curveParamsStartPosX = 0;
		m_chunkIndexOffset = 0;
		InitialiseCurve(startPos, num, ref m_currentCurveInfo);
		while (m_pooledChunks.Count > 0 && GenerateChunk(ref m_currentCurveInfo))
		{
		}
	}

	private Mesh CreateSegmentMesh()
	{
		int num = (segmentsPerChunk + 1) * 3;
		int num2 = segmentsPerChunk * 12;
		Vector3[] vertices = new Vector3[num];
		int[] array = new int[num2];
		Vector2[] array2 = new Vector2[num];
		float num3 = 0f;
		float num4 = 1f / (float)textureScale;
		int num5 = 0;
		for (int i = 0; i <= segmentsPerChunk; i++)
		{
			array2[num5++] = new Vector2(num3, 1f);
			array2[num5++] = new Vector2(num3, surfaceTextureRatio);
			array2[num5++] = new Vector2(num3, 0f);
			num3 += num4;
			if (num3 > 1f)
			{
				num4 = 0f - num4;
			}
			else if (num3 < 0f)
			{
				num4 = 0f - num4;
			}
		}
		int num6 = 0;
		for (int j = 0; j <= segmentsPerChunk - 1; j++)
		{
			int num7 = j * 3;
			array[num6++] = num7;
			array[num6++] = num7 + 3;
			array[num6++] = num7 + 1;
			array[num6++] = num7 + 1;
			array[num6++] = num7 + 3;
			array[num6++] = num7 + 4;
			array[num6++] = num7 + 1;
			array[num6++] = num7 + 4;
			array[num6++] = num7 + 2;
			array[num6++] = num7 + 2;
			array[num6++] = num7 + 4;
			array[num6++] = num7 + 5;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = array;
		mesh.uv = array2;
		return mesh;
	}

	private Mesh CreateEdgeMesh()
	{
		int num = (segmentsPerChunk + 1) * 2;
		int num2 = segmentsPerChunk * 6;
		Vector3[] vertices = new Vector3[num];
		int[] array = new int[num2];
		Vector2[] array2 = new Vector2[num];
		float num3 = 0f;
		float num4 = 1f / (float)textureScale;
		int num5 = 0;
		for (int i = 0; i <= segmentsPerChunk; i++)
		{
			array2[num5++] = new Vector2(num3, 1f);
			array2[num5++] = new Vector2(num3, 0f);
			num3 += num4;
			if (num3 > 1f)
			{
				num4 = 0f - num4;
			}
			else if (num3 < 0f)
			{
				num4 = 0f - num4;
			}
		}
		int num6 = 0;
		for (int j = 0; j <= segmentsPerChunk - 1; j++)
		{
			int num7 = j * 2;
			array[num6++] = num7;
			array[num6++] = num7 + 2;
			array[num6++] = num7 + 1;
			array[num6++] = num7 + 1;
			array[num6++] = num7 + 2;
			array[num6++] = num7 + 3;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = array;
		mesh.uv = array2;
		return mesh;
	}

	private void CreateChunk()
	{
		Vector3[] points = new Vector3[segmentsPerChunk + 1];
		Vector3[] normals = new Vector3[segmentsPerChunk + 1];
		GameObject gameObject = new GameObject("Terrain Chunk");
		gameObject.transform.parent = base.transform;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = CreateSegmentMesh();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		GameObject gameObject2 = new GameObject("Edge");
		gameObject2.transform.parent = gameObject.transform;
		MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
		Mesh edgeMesh = (meshFilter2.mesh = CreateEdgeMesh());
		MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
		meshRenderer2.material = edgeMaterial;
		gameObject.SetActive(false);
		m_pooledChunks.Add(new ChunkInfo(gameObject, meshFilter.mesh, edgeMesh, points, normals));
	}

	private void Awake()
	{
		if (s_terrainInstance[(int)terrainLayer] != null)
		{
		}
		s_terrainInstance[(int)terrainLayer] = this;
		int num = (lookAheadSegmentCount + cullBehindSegmentCount) / segmentsPerChunk;
		for (int i = 0; i < num; i++)
		{
			CreateChunk();
		}
		m_segmentContacts = new GeometryUtils.ContactInfo[maxSegmentContacts];
		for (int j = 0; j < maxSegmentContacts; j++)
		{
			m_segmentContacts[j].Invalidate();
		}
	}



    private void OnDisable()
	{
		while (m_chunks.Count > 0)
		{
			m_pooledChunks.Add(m_chunks[0]);
			m_chunks.RemoveAt(0);
		}
		m_curveParamsQueue.Clear();
		m_lineColliders.Clear();
	}

	private void Update()
	{
		if (m_chunks.Count != 0 && (!SkiGameManager.Instance.TitleScreenActive || terrainLayer != 0))
		{
			Camera main = Camera.main;
			float z = base.transform.position.z - main.transform.position.z;
			int num = Mathf.FloorToInt(main.ViewportToWorldPoint(new Vector3(1f, 0f, base.transform.position.z)).x / segmentWidth);
			if (num > m_chunks[m_chunks.Count - 1].startSegmentIndex - lookAheadSegmentCount)
			{
				GenerateChunk(ref m_currentCurveInfo);
			}
			int num2 = Mathf.FloorToInt(main.ViewportToWorldPoint(new Vector3(-1f, 0f, z)).x / segmentWidth);
			if (num2 > m_chunks[0].endSegmentIndex + cullBehindSegmentCount)
			{
				m_chunks[0].gameObject.SetActive(false);
				m_pooledChunks.Add(m_chunks[0]);
				m_chunks.RemoveAt(0);
				m_chunkIndexOffset++;
			}
		}
	}
}
