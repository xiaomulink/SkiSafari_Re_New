using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	[Serializable]
	public class QueuedSpawn
	{
		public SpawnParams spawnParams;

		public SpawnFlags spawnFlags;

		public int minPosX;
	}

	public enum SpawnFlags
	{
		None = 0,
		IgnoreDistance = 1,
		IgnoreSlope = 2,
		IgnoreSpawnLimit = 4,
		IgnoreSeparation = 8,
		IgnoreAll = -1
	}

	public class SpawnInfo
	{
		public SpawnParams spawnParams;

		public int separation;

		public int lastSpawnPosX = -32768;

		public PoolRef lastSpawnedInstance;

		public bool grouping;

		public List<PoolTable> poolTables = new List<PoolTable>();

		public List<PoolTable> indirectPoolTables = new List<PoolTable>();
	}

	public class SpawnAvoidanceInfo
	{
		public Vector3 startPos;

		public Vector3 endPos;
	}

	public static SpawnManager ForegroundInstance;

	public TerrainLayer terrainLayer;

	public bool isPrimary;

	public int cullBehindDistance = 100;

	public int cullForwardDistance = 200;

	public int spawnDistance = 100;

	public int spawnStepSize = 1;

	public int initialOffScreenSpawnOffset = 5;

	public SpawnParams[] spawnParamsList;

	public QueuedSpawn[] initialQueuedSpawns;

	private Transform m_container;

	private List<SpawnParams> m_availableSpawnParams = new List<SpawnParams>();

	private Dictionary<SpawnParams, SpawnInfo> m_spawnParamsInfo = new Dictionary<SpawnParams, SpawnInfo>();

	private List<SpawnInfo> m_spawnInfoList = new List<SpawnInfo>();

	private int m_minSpawnPosX;

	private int m_currentPosX = -32768;

	[SerializeField]
	private List<QueuedSpawn> m_queuedSpawns = new List<QueuedSpawn>();

	private List<SpawnAvoidanceInfo> m_spawnAvoidances = new List<SpawnAvoidanceInfo>();

	private static SpawnManager[] s_instances = new SpawnManager[4];

	public static SpawnManager GetForLayer(TerrainLayer terrainLayer)
	{
		return s_instances[(int)terrainLayer];
	}

	public void QueueSpawn(SpawnParams spawnParams, SpawnFlags spawnFlags, int minPosX)
	{
		RegisterSpawnParams(spawnParams);
		int i;
		for (i = 0; i < m_queuedSpawns.Count && m_queuedSpawns[i].minPosX <= minPosX; i++)
		{
		}
		m_queuedSpawns.Insert(i, new QueuedSpawn
		{
			spawnParams = spawnParams,
			spawnFlags = spawnFlags,
			minPosX = minPosX
		});
	}

	public GameObject ManualSpawn(SpawnParams spawnParams, Vector3 position, Quaternion rotation, SpawnFlags spawnFlags)
	{
		SpawnInfo spawnInfo = RegisterSpawnParams(spawnParams);
		if ((spawnFlags & SpawnFlags.IgnoreSpawnLimit) == 0)
		{
			int activeCount = GetActiveCount(spawnInfo);
			if (activeCount >= spawnParams.maxActiveSpawns)
			{
				return null;
			}
		}
		if ((spawnFlags & SpawnFlags.IgnoreDistance) == 0 && position.x < (float)spawnParams.requiredDistance)
		{
			return null;
		}
		int num = Mathf.RoundToInt(position.x);
		if ((spawnFlags & SpawnFlags.IgnoreSeparation) == 0)
		{
			int num2 = num - spawnInfo.lastSpawnPosX;
			if ((float)num2 < spawnParams.minSeparation)
			{
				return null;
			}
		}
		if (spawnParams.blackListCurves.Count > 0)
		{
			Terrain.CurveChecker curveChecker = Terrain.GetTerrainForLayer(TerrainLayer.Game).GetCurveChecker(Mathf.RoundToInt(position.x));
			if (curveChecker != null && curveChecker.ContainsAnyCurves(spawnParams.blackListCurves, m_currentPosX, spawnParams.blackListCurveDistance))
			{
				return null;
			}
		}
		GameObject gameObject = Pool.Spawn(spawnParams.prefabs[UnityEngine.Random.Range(0, spawnParams.prefabs.Length)], position, rotation);
		gameObject.transform.parent = m_container;
		if (gameObject.isStatic && (float)(spawnParams.leftClearance + spawnParams.rightClearance) > 0f)
		{
			Vector3 position2 = gameObject.transform.position;
			AddSpawnAvoidance(position2 - new Vector3(spawnParams.leftClearance, 0f), position2 + new Vector3(spawnParams.rightClearance, 0f));
		}
		spawnInfo.lastSpawnPosX = num;
		spawnInfo.lastSpawnedInstance = gameObject;
		return gameObject;
	}

	public void AddSpawnAvoidance(Vector3 startPos, Vector3 endPos)
	{
		if (m_minSpawnPosX < Mathf.FloorToInt(endPos.x))
		{
			m_spawnAvoidances.Add(new SpawnAvoidanceInfo
			{
				startPos = startPos,
				endPos = endPos
			});
		}
	}

	public void Reset()
	{
		base.enabled = false;
		base.enabled = true;
	}

	public SpawnInfo RegisterSpawnParams(SpawnParams spawnParams)
	{

        if (!m_spawnParamsInfo.ContainsKey(spawnParams))
		{
			SpawnInfo spawnInfo = new SpawnInfo();
			spawnInfo.spawnParams = spawnParams;
			GameObject[] prefabs = spawnParams.prefabs;
			foreach (GameObject prefab in prefabs)
			{
				PoolTable table = Pool.GetTable(prefab);
				spawnInfo.poolTables.Add(table);
			}
			m_spawnParamsInfo[spawnParams] = spawnInfo;
			m_spawnInfoList.Add(spawnInfo);
			return spawnInfo;
		}
		return m_spawnParamsInfo[spawnParams];
	}

	private int GetActiveCount(SpawnInfo spawnInfo)
	{
		int num = 0;
		foreach (PoolTable poolTable in spawnInfo.poolTables)
		{
			num += poolTable.ActiveCount;
		}
		foreach (PoolTable indirectPoolTable in spawnInfo.indirectPoolTables)
		{
			num += indirectPoolTable.ActiveCount;
		}
		return num;
	}

	private void UpdateCulling(int cameraLeftPosX, int cameraRightPosX)
	{
		for (int i = 0; i < m_spawnInfoList.Count; i++)
		{
			SpawnInfo spawnInfo = m_spawnInfoList[i];
			int num = cameraLeftPosX - spawnInfo.spawnParams.cullBehindDistance;
			int num2 = Mathf.Max(m_currentPosX, cameraRightPosX + spawnInfo.spawnParams.cullForwardDistance);
			for (int j = 0; j < spawnInfo.poolTables.Count; j++)
			{
				PoolTable poolTable = spawnInfo.poolTables[j];
				for (int k = 0; k < poolTable.ActiveObjects.Count; k++)
				{
					GameObject gameObject = poolTable.ActiveObjects[k];
					float x = gameObject.transform.position.x;
					if (x < (float)num || x > (float)num2)
					{
						Pool.Despawn(gameObject);
					}
				}
			}
		}
	}

	private GameObject TrySpawn(SpawnParams spawnParams, Terrain terrain, float height, Vector3 normal, float slope, int maxSpawnPosX, SpawnFlags spawnFlags, Terrain.CurveChecker curveChecker)
	{
       
        SpawnInfo spawnInfo = m_spawnParamsInfo[spawnParams];
		if (((slope < spawnParams.minSlope || slope > spawnParams.maxSlope) && (spawnFlags & SpawnFlags.IgnoreSlope) == 0) || (m_currentPosX < spawnParams.requiredDistance && (spawnFlags & SpawnFlags.IgnoreDistance) == 0) || m_currentPosX - m_minSpawnPosX < spawnParams.leftClearance || maxSpawnPosX - m_currentPosX < spawnParams.rightClearance)
		{
			return null;
		}
		int num = m_currentPosX - spawnInfo.lastSpawnPosX;
		if (spawnInfo.grouping)
		{
			if (num > spawnParams.leftClearance + spawnParams.rightClearance)
			{
				spawnInfo.grouping = false;
				return null;
			}
		}
		else if (num < spawnInfo.separation)
		{
			return null;
		}
		if ((spawnFlags & SpawnFlags.IgnoreSpawnLimit) == 0)
		{
			int activeCount = GetActiveCount(spawnInfo);
			if (activeCount >= spawnParams.maxActiveSpawns)
			{
				return null;
			}
		}
        try
        {
            if (spawnParams.blackListCurves.Count > 0 && curveChecker.ContainsAnyCurves(spawnParams.blackListCurves, m_currentPosX, spawnParams.blackListCurveDistance))
            {
                return null;
            }
       
        if (spawnParams.avoidCaveBoundaries && terrain.IsPositionInOrAboveCave(m_currentPosX) != terrain.IsPositionInOrAboveCave(m_currentPosX + spawnParams.rightClearance))
		{
			return null;
		}
        }
        catch
        {

        }
        float num2 = spawnParams.terrainHeightOffset + UnityEngine.Random.value * spawnParams.randomTerrainHeightOffset;
		Vector3 position = new Vector3(m_currentPosX, height + num2, terrain.transform.position.z);
		Quaternion rotation = ((!spawnParams.matchSlopeRotation) ? Quaternion.identity : Quaternion.LookRotation(Vector3.forward, normal));
		GameObject gameObject = Pool.Spawn(spawnParams.prefabs[UnityEngine.Random.Range(0, spawnParams.prefabs.Length)], position, rotation);
		gameObject.transform.parent = m_container;
		spawnInfo.lastSpawnPosX = m_currentPosX;
		spawnInfo.lastSpawnedInstance = gameObject;
		m_minSpawnPosX = m_currentPosX + spawnParams.rightClearance;
		if (spawnParams.groupable && UnityEngine.Random.value <= spawnParams.groupChance)
		{
			spawnInfo.grouping = true;
			m_currentPosX = m_minSpawnPosX + spawnParams.leftClearance;
		}
		else
		{
			spawnInfo.grouping = false;
			m_currentPosX = m_minSpawnPosX + spawnStepSize;
		}
		spawnInfo.separation = Mathf.RoundToInt(UnityEngine.Random.Range(spawnParams.minSeparation, spawnParams.maxSeparation));
		return gameObject;
	}

	private void UpdateSpawning(Terrain terrain, int cameraLeftPosX, int cameraRightPosX)
	{
		if (m_currentPosX == -32768)
		{
			m_currentPosX = cameraLeftPosX - initialOffScreenSpawnOffset;
			m_minSpawnPosX = m_currentPosX;
			foreach (KeyValuePair<SpawnParams, SpawnInfo> item in m_spawnParamsInfo)
			{
				item.Value.lastSpawnPosX = m_minSpawnPosX;
				item.Value.separation = 0;
			}
		}
		m_currentPosX = Mathf.Max(m_currentPosX, cameraLeftPosX - cullBehindDistance);
		int num = cameraRightPosX + spawnDistance;
		int num2 = int.MaxValue;
		while (m_currentPosX < num)
		{
			float height = 0f;
			Vector3 normal = Vector3.zero;
			if (!terrain.GetHeightAndNormal(m_currentPosX, ref height, ref normal))
			{
				break;
			}
			float num3 = Vector3.Angle(Vector3.up, normal);
			if (Vector3.Dot(normal, Vector3.right) < 0f)
			{
				num3 = 0f - num3;
			}
			Terrain.CurveChecker curveChecker = terrain.GetCurveChecker(m_currentPosX);
			GameObject gameObject = null;
			num2 = int.MaxValue;
			bool flag = false;
			int num4 = 0;
			while (num4 < m_spawnAvoidances.Count)
			{
				SpawnAvoidanceInfo spawnAvoidanceInfo = m_spawnAvoidances[num4];
				if ((float)m_currentPosX >= spawnAvoidanceInfo.startPos.x)
				{
					if (!terrain.IsPointInCave(spawnAvoidanceInfo.startPos) || !terrain.IsPointInCave(spawnAvoidanceInfo.endPos))
					{
						m_minSpawnPosX = Mathf.Max(m_minSpawnPosX, Mathf.CeilToInt(spawnAvoidanceInfo.endPos.x));
						m_currentPosX = Mathf.Max(m_currentPosX, m_minSpawnPosX);
						flag = true;
					}
					m_spawnAvoidances.RemoveAt(num4);
				}
				else
				{
					num2 = Mathf.Min(num2, Mathf.FloorToInt(spawnAvoidanceInfo.startPos.x));
					num4++;
				}
			}
			if (flag)
			{
				continue;
			}
			QueuedSpawn queuedSpawn = null;
			foreach (QueuedSpawn queuedSpawn2 in m_queuedSpawns)
			{
				if (m_currentPosX >= queuedSpawn2.minPosX)
				{
					gameObject = TrySpawn(queuedSpawn2.spawnParams, terrain, height, normal, num3, int.MaxValue, queuedSpawn2.spawnFlags, curveChecker);
					if ((bool)gameObject)
					{
						m_queuedSpawns.Remove(queuedSpawn2);
						break;
					}
				}
				else if (m_currentPosX >= queuedSpawn2.minPosX - queuedSpawn2.spawnParams.leftClearance)
				{
					if (queuedSpawn == null || queuedSpawn2.minPosX < queuedSpawn.minPosX)
					{
						queuedSpawn = queuedSpawn2;
					}
				}
				else
				{
					num2 = Mathf.Min(num2, queuedSpawn2.minPosX - queuedSpawn2.spawnParams.leftClearance);
				}
			}
			if (queuedSpawn != null)
			{
				m_currentPosX = queuedSpawn.minPosX;
				continue;
			}
			if (!gameObject)
			{
				foreach (SpawnParams availableSpawnParam in m_availableSpawnParams)
				{
					gameObject = TrySpawn(availableSpawnParam, terrain, height, normal, num3, num2, SpawnFlags.None, curveChecker);
					if ((bool)gameObject)
					{
						break;
					}
				}
			}
			if (!gameObject)
			{
				m_currentPosX += spawnStepSize;
			}
		}
	}

	private void Awake()
	{
		if (terrainLayer == TerrainLayer.Game)
		{
			if (isPrimary)
			{
				ForegroundInstance = this;
				s_instances[0] = this;
			}
		}
		else
		{
			if (s_instances[(int)terrainLayer] != null)
			{
			}
			s_instances[(int)terrainLayer] = this;
		}
		GameObject gameObject = new GameObject(base.name + "_container");
		m_container = gameObject.transform;
		m_container.parent = base.transform.parent;
	}

	private void OnEnable()
	{
		if ((bool)Pool.Instance)
		{
			QueuedSpawn[] array = initialQueuedSpawns;
			foreach (QueuedSpawn queuedSpawn in array)
			{
				QueueSpawn(queuedSpawn.spawnParams, queuedSpawn.spawnFlags, queuedSpawn.minPosX);
			}
			m_availableSpawnParams.Clear();
			SpawnParams[] array2 = spawnParamsList;
			foreach (SpawnParams spawnParams in array2)
			{
				m_availableSpawnParams.Add(spawnParams);
				RegisterSpawnParams(spawnParams);
			}
		}
	}

	private void OnDisable()
	{
		foreach (SpawnInfo spawnInfo in m_spawnInfoList)
		{
			foreach (PoolTable poolTable in spawnInfo.poolTables)
			{
				foreach (GameObject activeObject in poolTable.ActiveObjects)
				{
					poolTable.Despawn(activeObject);
				}
				poolTable.Update();
			}
			spawnInfo.separation = 0;
			spawnInfo.lastSpawnPosX = -32768;
			spawnInfo.lastSpawnedInstance = null;
			spawnInfo.grouping = false;
		}
		m_minSpawnPosX = 0;
		m_currentPosX = -32768;
		m_queuedSpawns.Clear();
		m_spawnAvoidances.Clear();
	}

	private void Update()
	{
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if ((bool)terrainForLayer)
		{
			float z = Mathf.Abs(terrainForLayer.transform.position.z - Camera.main.transform.position.z);
			int cameraLeftPosX = Mathf.FloorToInt(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, z)).x);
			int cameraRightPosX = Mathf.CeilToInt(Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, z)).x);
			UpdateCulling(cameraLeftPosX, cameraRightPosX);
			UpdateSpawning(terrainForLayer, cameraLeftPosX, cameraRightPosX);
		}
	}

	private void OnDrawGizmos()
	{
		if (this != ForegroundInstance)
		{
			return;
		}
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if (!terrainForLayer || !terrainForLayer.enabled)
		{
			return;
		}
		foreach (SpawnAvoidanceInfo spawnAvoidance in m_spawnAvoidances)
		{
			if (!terrainForLayer.IsPointInCave(spawnAvoidance.startPos) || !terrainForLayer.IsPointInCave(spawnAvoidance.endPos))
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
			}
			else
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
			}
			Vector3 center = (spawnAvoidance.startPos + spawnAvoidance.endPos) * 0.5f;
			Gizmos.DrawCube(center, new Vector3(spawnAvoidance.endPos.x - spawnAvoidance.startPos.x, 20f, 0f));
		}
		Gizmos.color = Color.red;
		float height = 0f;
		float num = 100f;
		float z = Mathf.Abs(terrainForLayer.transform.position.z - Camera.main.transform.position.z);
		int num2 = Mathf.FloorToInt(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, z)).x);
		int num3 = Mathf.CeilToInt(Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, z)).x);
		int num4 = num2 - cullBehindDistance;
		int num5 = num3 + cullForwardDistance;
		if (terrainForLayer.GetHeight(num4, ref height))
		{
			Gizmos.color = Color.red;
			Vector3 vector = new Vector3(num4, height, terrainForLayer.transform.position.z);
			Gizmos.DrawLine(vector, vector + Vector3.up * num);
			Gizmos.DrawCube(vector, new Vector3(10f, 0.1f, 10f));
		}
		if (terrainForLayer.GetHeight(m_currentPosX, ref height))
		{
			Gizmos.color = Color.blue;
			Vector3 vector2 = new Vector3(m_currentPosX, height, terrainForLayer.transform.position.z);
			Gizmos.DrawLine(vector2, vector2 + Vector3.up * num);
			Gizmos.DrawCube(vector2, new Vector3(10f, 0.1f, 10f));
		}
		if (terrainForLayer.GetHeight(num5, ref height))
		{
			Gizmos.color = Color.red;
			Vector3 vector3 = new Vector3(num5, height, terrainForLayer.transform.position.z);
			Gizmos.DrawLine(vector3, vector3 + Vector3.up * num);
			Gizmos.DrawCube(vector3, new Vector3(10f, 0.1f, 10f));
		}
	}
}
