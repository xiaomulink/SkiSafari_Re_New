using UnityEngine;

public class UVScroller : MonoBehaviour
{
	public Vector2 speed = new Vector2(1f, 0f);

	private Material m_material;

	private float m_lastTime;

	private void OnEnable()
	{
		m_material = GetComponent<Renderer>().material;
		m_lastTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - m_lastTime;
		Vector2 vector = speed * num;
		m_material.mainTextureOffset += vector;
		m_lastTime = realtimeSinceStartup;
	}
}
