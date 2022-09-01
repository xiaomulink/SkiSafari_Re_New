using UnityEngine;

public class Water : MonoBehaviour
{
	public float width = 1f;

	public float height = 1f;

	protected float m_halfWidth;

	protected float m_halfHeight;

	protected void Awake()
	{
		m_halfWidth = width * 0.5f;
		m_halfHeight = height * 0.5f;
	}

	private void Update()
	{
		if ((bool)Player.Instance)
		{
			Vector2 vector = base.transform.position - Player.Instance.transform.position;
			if (vector.x >= 0f - m_halfWidth && vector.x <= m_halfWidth && vector.y >= 0f - m_halfHeight && vector.y <= m_halfHeight)
			{
				Player.Instance.Freeze();
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(base.transform.position, new Vector3(width, height, 0f));
	}
}
