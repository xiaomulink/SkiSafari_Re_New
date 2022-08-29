using System;
using System.Collections.Generic;
using UnityEngine;

public class CreditsSpawner : MonoBehaviour
{
	[Serializable]
	public class Entry
	{
		public GameObject prefab;

		public int offset;

		public int radius = 20;

		public float angleMatch = 0.5f;
	}

	public static CreditsSpawner Instance;

	public Entry[] entries;

	public int spawnDistance = 100;

	public int cullDistance = 100;

	public float heightOffset = 20f;

	public float depthOffset = -10f;

	public float maxSlope = 45f;

	private bool m_enableSpawning;

	private int m_nextEntryIndex;

	private int m_nextSpawnPos;

	private List<GameObject> m_entryInstances = new List<GameObject>();

	public bool EnableSpawning
	{
		get
		{
			return m_enableSpawning;
		}
		set
		{
			m_enableSpawning = value;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		m_nextEntryIndex = 0;
		m_nextSpawnPos = 0;
	}

	private void OnDisable()
	{
		foreach (GameObject entryInstance in m_entryInstances)
		{
			if ((bool)entryInstance)
			{
				Pool.Despawn(entryInstance);
			}
		}
		m_entryInstances.Clear();
	}

	private void Update()
	{
		if (!m_enableSpawning)
		{
			return;
		}
		Player instance = Player.Instance;
		if (!instance || SkiGameManager.Instance.TitleScreenActive)
		{
			return;
		}
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(TerrainLayer.Game);
		if (!terrainForLayer)
		{
			return;
		}
		float z = Mathf.Abs(terrainForLayer.transform.position.z - Camera.main.transform.position.z);
		int num = Mathf.FloorToInt(Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, z)).x);
		int num2 = Mathf.CeilToInt(Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, z)).x);
		int num3 = 0;
		while (num3 < m_entryInstances.Count)
		{
			GameObject gameObject = m_entryInstances[num3];
			if (!gameObject)
			{
				m_entryInstances.RemoveAt(num3);
				continue;
			}
			int num4 = num - Mathf.FloorToInt(gameObject.transform.position.x);
			if (num4 > cullDistance)
			{
				Pool.Despawn(gameObject);
				m_entryInstances.RemoveAt(num3);
			}
			else
			{
				num3++;
			}
		}
		if (m_nextEntryIndex < entries.Length)
		{
			int num5 = num2 + spawnDistance;
			if (num5 < m_nextSpawnPos)
			{
				return;
			}
			Entry entry = entries[m_nextEntryIndex];
			if (Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPositionInOrAboveCave(num5))
			{
				return;
			}
			float height = 0f;
			if (terrainForLayer.GetHeight(num5 + entry.radius, ref height))
			{
				float height2 = 0f;
				terrainForLayer.GetHeight(num5, ref height2);
				Quaternion identity = Quaternion.identity;
				float num6 = Mathf.Atan2(height - height2, entry.radius);
				identity = Quaternion.AngleAxis(num6 * entry.angleMatch * 57.29578f, Vector3.forward);
				float num7 = Mathf.Max(height2, height);
				Vector3 position = new Vector3(num5, num7 + heightOffset, terrainForLayer.transform.position.z + depthOffset);
				GameObject item = Pool.Spawn(entry.prefab, position, identity);
				m_entryInstances.Add(item);
				m_nextEntryIndex++;
				if (m_nextEntryIndex < entries.Length)
				{
					m_nextSpawnPos = num5 + entries[m_nextEntryIndex].offset;
				}
			}
		}
		else if (m_entryInstances.Count == 0)
		{
			m_enableSpawning = false;
			GUITransitionAnimator[] onPlayShowTransitions = SkiGameManager.Instance.onPlayShowTransitions;
			foreach (GUITransitionAnimator gUITransitionAnimator in onPlayShowTransitions)
			{
				gUITransitionAnimator.Show();
			}
		}
	}
}
