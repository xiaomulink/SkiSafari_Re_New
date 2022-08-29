using UnityEngine;

public class TrajectorySpawner : MonoBehaviour
{
	public SpawnParams spawnParams;

	public SpawnManager.SpawnFlags spawnFlags;

	public float totalDistance = 10f;

	public float randomChance = 0.5f;

	private void OnEnable()
	{
		if ((bool)SkiGameManager.Instance && SkiGameManager.Instance.Started && Random.value <= randomChance)
		{
			float num = 0f;
			float num2 = spawnParams.leftClearance + spawnParams.rightClearance;
			Vector3 position = base.transform.position;
			do
			{
				SpawnManager.ForegroundInstance.ManualSpawn(spawnParams, position, Quaternion.identity, spawnFlags);
				num += num2;
				position += base.transform.right * num2;
			}
			while (num < totalDistance);
		}
	}
}
