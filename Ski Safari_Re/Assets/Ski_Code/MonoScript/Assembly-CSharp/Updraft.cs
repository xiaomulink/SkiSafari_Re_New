using UnityEngine;

public class Updraft : MonoBehaviour
{
	public float speed = 10f;

	private float m_height = 1f;

	private float m_halfWidth = 1f;

	private void Start()
	{
		m_height = GetComponent<Collider>().bounds.size.y;
		m_halfWidth = GetComponent<Collider>().bounds.size.x / 2f;
	}

	private void OnTriggerStay(Collider collider)
	{
		Flyer component = collider.GetComponent<Flyer>();
		if ((bool)component)
		{
			Vector3 vector = collider.transform.position - base.transform.position;
			float num = Mathf.Min(1f, (1f - Mathf.Abs(vector.x) / m_halfWidth) * (1f - vector.y / m_height));
			component.SetUpdraftSpeed(speed * num);
		}
	}
}
