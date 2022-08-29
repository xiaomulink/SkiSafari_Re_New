using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject[] prefabs;

	public int maxObjects = 20;

	public Vector2 cullDistance = new Vector3(100f, 100f);

	public Vector3 minSpawnDistance = new Vector3(20f, 0f, 50f);

	public Vector3 maxSpawnDistance = new Vector3(50f, 0f, 100f);

	public Vector3 minSeparation = new Vector3(5f, -10f, -10f);

	public Vector3 maxSeparation = new Vector3(15f, 10f, 10f);

	private GameObject[] m_instances;

	private int m_instanceCount;

	private Vector3 m_lastPos = Vector3.zero;

	private void UpdateCulling()
	{
		Vector3 position = Camera.main.transform.position;
		int num = 0;
		while (num < m_instanceCount)
		{
			GameObject gameObject = m_instances[num];
			if (!gameObject)
			{
				m_instances[num] = m_instances[--m_instanceCount];
				continue;
			}
			Vector3 vector = position - gameObject.transform.position;
			if (vector.x > cullDistance.x || Mathf.Abs(vector.y) > cullDistance.y)
			{
				Pool.Despawn(gameObject.gameObject);
				m_instances[num] = m_instances[--m_instanceCount];
			}
			else
			{
				num++;
			}
		}
	}

	private void UpdateSpawning(Vector3 minPos, Vector3 maxPos)
	{
		Vector3 vector = default(Vector3);
		while (m_instanceCount < maxObjects)
		{
			vector.x = Mathf.Clamp(m_lastPos.x + Random.Range(minSeparation.x, maxSeparation.x), minPos.x, maxPos.x);
			if (vector.x >= maxPos.x)
			{
				break;
			}
			vector.y = Mathf.Clamp(m_lastPos.y + Random.Range(minSeparation.y, maxSeparation.y), minPos.y, maxPos.y);
			vector.z = Mathf.Clamp(m_lastPos.z + Random.Range(minSeparation.z, maxSeparation.z), minPos.z, maxPos.z);
			GameObject gameObject = Pool.Spawn(prefabs[Random.Range(0, prefabs.Length)], vector, Quaternion.identity);
			m_instances[m_instanceCount++] = gameObject;
			m_lastPos = vector;
		}
	}

	private void Awake()
	{
		m_instances = new GameObject[maxObjects];
	}

	private void Update()
	{
		UpdateCulling();
		if ((bool)Player.Instance)
		{
			Vector3 position = Player.Instance.transform.position;
			Vector3 minPos = position + minSpawnDistance;
			Vector3 maxPos = position + maxSpawnDistance;
			UpdateSpawning(minPos, maxPos);
		}
	}
}
