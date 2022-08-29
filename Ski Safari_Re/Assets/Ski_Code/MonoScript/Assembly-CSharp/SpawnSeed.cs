using UnityEngine;

public class SpawnSeed : MonoBehaviour
{
	public SpawnParams spawnParams;

	public SpawnManager.SpawnFlags spawnFlags;

	public TerrainLayer terrainLayer;

	public int count;

	private Vector3 m_spawnPos;

	private int m_spawnCount;

	private void UpdateSpawning()
	{
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if ((bool)terrainForLayer)
		{
			float height = 0f;
			Vector3 normal = Vector3.zero;
			while (m_spawnCount < count && terrainForLayer.GetHeightAndNormal(m_spawnPos, ref height, ref normal))
			{
				m_spawnPos.y = height + (spawnParams.terrainHeightOffset + Random.value * spawnParams.randomTerrainHeightOffset) / normal.y;
				Quaternion rotation = ((!spawnParams.matchSlopeRotation) ? Quaternion.identity : Quaternion.LookRotation(Vector3.forward, normal));
				SpawnManager.ForegroundInstance.ManualSpawn(spawnParams, m_spawnPos, rotation, spawnFlags);
				m_spawnPos += new Vector3(normal.y, 0f - normal.x) * (spawnParams.leftClearance + spawnParams.rightClearance);
				m_spawnCount++;
			}
		}
	}

	private void OnEnable()
	{
		if ((bool)SkiGameManager.Instance && SkiGameManager.Instance.Started)
		{
			m_spawnPos = base.transform.position;
			m_spawnCount = 0;
			UpdateSpawning();
		}
	}

	private void Update()
	{
		UpdateSpawning();
	}
}
