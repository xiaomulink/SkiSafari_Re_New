using UnityEngine;

public class SpawnBlocker : MonoBehaviour
{
	public float distance = 10f;

	private void OnEnable()
	{
		if ((bool)SpawnManager.ForegroundInstance)
		{
			SpawnManager.ForegroundInstance.AddSpawnAvoidance(base.transform.position, base.transform.position + new Vector3(distance, 0f, 0f));
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(base.transform.position + Vector3.right * (distance * 0.5f), new Vector3(distance, 1f, 0f));
	}
}
