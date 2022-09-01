using UnityEngine;

public class AvalancheDestroyable : MonoBehaviour
{
	public Vector3 offset;

	public GameObject destroyEffect;

	public void Destroy()
	{
		if ((bool)destroyEffect)
		{
			Vector3 position = base.transform.TransformPoint(offset);
			Pool.Spawn(destroyEffect, position, base.transform.rotation);
		}
		Pool.Despawn(base.gameObject);
	}

	private void Update()
	{
		Vector3 position = base.transform.TransformPoint(offset);
		if ((bool)Avalanche.Instance && CameraUtils.IsPointVisible(position) && Avalanche.Instance.Contains(position))
		{
			Destroy();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(base.transform.TransformPoint(offset), 0.5f);
	}
}
