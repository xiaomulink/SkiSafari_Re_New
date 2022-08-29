using UnityEngine;

public class FXCameraShake : MonoBehaviour
{
	public float magnitude = 1f;

	public float duration = 1f;

	public float radius = 10f;

	public bool persistent;

	public void TriggerShake()
	{
		if ((bool)FollowCamera.Instance && !SkiGameManager.Instance.Initialising)
		{
			Vector3 vector = FollowCamera.Instance.transform.position - base.transform.position;
			vector.z = 0f;
			float num = vector.magnitude;
			if (num < radius)
			{
				float num2 = 1f - num / radius;
				FollowCamera.Instance.Shake(duration, magnitude * num2);
			}
		}
	}

	private void OnEnable()
	{
		TriggerShake();
		if (!persistent)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		TriggerShake();
	}
}
