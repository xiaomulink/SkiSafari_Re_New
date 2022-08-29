using UnityEngine;

public class iTween_ScaleFrom : MonoBehaviour
{
	public Vector3 scale = Vector3.zero;

	public float time = 0.5f;

	private void OnEnable()
	{
		Go.from(base.transform, time, new GoTweenConfig().scale(scale));
	}
}
