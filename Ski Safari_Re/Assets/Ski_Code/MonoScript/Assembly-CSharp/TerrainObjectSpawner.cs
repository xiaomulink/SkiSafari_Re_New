using UnityEngine;

public class TerrainObjectSpawner : MonoBehaviour
{
	public enum Prerequisite
	{
		None = 0,
		SnowballMode = 1
	}

	public TerrainLayer terrainLayer;

	public Prerequisite prerequisite;

	public GameObject[] prefabs;

	public int maxObjects = 20;

	public float cullDistance = 100f;

	public float minSpawnDistance = 20f;

	public float maxSpawnDistance = 100f;

	public float minSeparation = 20f;

	public float maxSeparation = 40f;

	public float minSlope = 30f;

	public float maxSlope = 90f;

	public bool matchSlopeRotation = true;

	public float terrainHeightOffset;

	private GameObject[] m_instances;

	private int m_instanceCount;

	private float m_lastPos;

	private void Awake()
	{
		m_instances = new GameObject[maxObjects];
	}

	private void Update()
	{
		Player instance = Player.Instance;
		if (!instance)
		{
			return;
		}
		Vector3 position = instance.transform.position;
		int num = 0;
		while (num < m_instanceCount)
		{
			GameObject gameObject = m_instances[num];
			if (!gameObject)
			{
				m_instances[num] = m_instances[--m_instanceCount];
				continue;
			}
			float num2 = position.x - gameObject.transform.position.x;
			if (num2 > cullDistance)
			{
				Pool.Despawn(gameObject.gameObject);
				m_instances[num] = m_instances[--m_instanceCount];
			}
			else
			{
				num++;
			}
		}
		Prerequisite prerequisite = this.prerequisite;
		if (prerequisite == Prerequisite.SnowballMode && !Player.Instance)
		{
			return;
		}
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if (!terrainForLayer)
		{
			return;
		}
		float min = position.x + minSpawnDistance;
		float num3 = position.x + maxSpawnDistance;
		while (m_instanceCount < maxObjects)
		{
			float num4 = Mathf.Clamp(m_lastPos + Random.Range(minSeparation, maxSeparation), min, num3);
			if (num4 >= num3)
			{
				break;
			}
			float height = 0f;
			Vector3 normal = Vector3.zero;
			if (terrainForLayer.GetHeightAndNormal(num4, ref height, ref normal))
			{
				float num5 = Vector3.Angle(Vector3.up, normal);
				if (Vector3.Dot(normal, Vector3.right) < 0f)
				{
					num5 = 0f - num5;
				}
				if (num5 > minSlope && num5 <= maxSlope)
				{
					Vector3 position2 = new Vector3(num4, height + terrainHeightOffset, terrainForLayer.transform.position.z);
					Quaternion rotation = ((!matchSlopeRotation) ? Quaternion.identity : Quaternion.LookRotation(Vector3.forward, normal));
					GameObject gameObject2 = Pool.Spawn(prefabs[Random.Range(0, prefabs.Length)], position2, rotation);
					m_instances[m_instanceCount++] = gameObject2;
				}
			}
			m_lastPos = num4;
		}
	}
}
