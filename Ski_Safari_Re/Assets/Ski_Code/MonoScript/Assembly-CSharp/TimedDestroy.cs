using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
	public float duration = 1f;

	public void SetTimer(float newDuration)
	{
		CancelInvoke("Destroy");
		Invoke("Destroy", newDuration);
	}

	private void Destroy()
	{
		Pool.Despawn(base.gameObject);
	}

	private void OnEnable()
	{
		Invoke("Destroy", duration);
	}

	private void OnDisable()
	{
		CancelInvoke("Destroy");
	}
}
