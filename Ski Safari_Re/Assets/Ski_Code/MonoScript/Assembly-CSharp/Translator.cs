using UnityEngine;

public class Translator : MonoBehaviour
{
	public Vector3 velocity;

	private void Update()
	{
		base.transform.position += velocity * Time.deltaTime;
	}
}
