using UnityEngine;

public class Parallax : MonoBehaviour
{
	public float parallaxScale = 0.5f;

	private void Update()
	{
		Vector3 position = Camera.main.transform.position * parallaxScale;
		position.z = base.transform.position.z;
		base.transform.position = position;
	}
}
