using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
	public Collectable collectablePrefab;

	public int maxCollectables = 20;

	public float cullDistance = 50f;

	public float minSpawnDistance = 20f;

	public float maxSpawnDistance = 50f;

	public float groupChance = 0.5f;

	public float groupSeparation = 3f;

	public float minSeparation = 5f;

	public float maxSeparation = 15f;

	public float minHeightOffset = -10f;

	public float maxHeightOffset = 10f;

	public TerrainLayer terrainLayer;

	public float minHeightAboveTerrain = 3f;

	private List<Collectable> m_collectables = new List<Collectable>();

	private int m_collectableCount;

	private float m_lastCollectablePosX;

	private float m_lastCollectablePosY;

	private void Start()
	{
		for (int i = 0; i < maxCollectables; i++)
		{
			m_collectables.Add(null);
		}
	}

	private void Update()
	{
		if ((bool)Player.Instance)
		{
			UpdateCollectables();
		}
	}

	private void UpdateCollectables()
	{
		Player instance = Player.Instance;
		Vector3 position = instance.transform.position;
		int num = 0;
		while (num < m_collectableCount)
		{
			Collectable collectable = m_collectables[num];
			if (!collectable || position.x - collectable.transform.position.x > cullDistance)
			{
				if ((bool)collectable)
				{
					Object.Destroy(collectable.gameObject);
				}
				m_collectables[num] = m_collectables[--m_collectableCount];
			}
			else
			{
				num++;
			}
		}
		m_lastCollectablePosX = Mathf.Max(m_lastCollectablePosX, instance.transform.position.x + minSpawnDistance);
		float min = position.y + minHeightOffset;
		float max = position.y + maxHeightOffset;
		while (m_collectableCount < maxCollectables)
		{
			if (Random.value <= groupChance)
			{
				m_lastCollectablePosX += groupSeparation;
			}
			else
			{
				m_lastCollectablePosX += Random.Range(minSeparation, maxSeparation);
				m_lastCollectablePosY = Mathf.Clamp(m_lastCollectablePosY + Random.Range(minHeightOffset, maxHeightOffset), min, max);
			}
			Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
			if ((bool)terrainForLayer)
			{
				float height = 0f;
				if (!terrainForLayer.GetHeight(m_lastCollectablePosX, ref height))
				{
					break;
				}
				m_lastCollectablePosY = height + minHeightAboveTerrain;
			}
			Collectable value = Object.Instantiate(position: new Vector3(m_lastCollectablePosX, m_lastCollectablePosY, 0f), original: collectablePrefab, rotation: Quaternion.identity) as Collectable;
			m_collectables[m_collectableCount++] = value;
			if (m_lastCollectablePosX - instance.transform.position.x >= maxSpawnDistance)
			{
				break;
			}
		}
	}
}
