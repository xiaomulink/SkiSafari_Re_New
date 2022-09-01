using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 axis = Vector3.up;

	public float speed = 90f;

	private void Update()
	{
		base.transform.rotation *= Quaternion.AngleAxis(speed * Time.deltaTime, axis);
	}
}
