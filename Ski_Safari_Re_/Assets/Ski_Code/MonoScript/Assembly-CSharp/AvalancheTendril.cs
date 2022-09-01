using UnityEngine;

public class AvalancheTendril : MonoBehaviour
{
	public TerrainLayer terrainLayer;

	public float speed = 50f;

	public float drag = 0.1f;

	public float gravity = -40f;

	public float lifetime = 5f;

	private float m_lifetimer;

	private TrailRenderer m_trailRenderer;

	private float m_initialTime;

	private float m_initialStartWidth;

	private void Awake()
	{
		m_trailRenderer = GetComponent<TrailRenderer>();
		m_initialTime = m_trailRenderer.time;
		m_initialStartWidth = m_trailRenderer.startWidth;
	}

	private void FixedUpdate()
	{
		m_lifetimer += Time.fixedDeltaTime;
		if (m_lifetimer > lifetime)
		{
			Pool.Despawn(base.gameObject);
			return;
		}
		float t = m_lifetimer / lifetime;
		m_trailRenderer.time = Mathf.Lerp(m_initialTime, 0f, t);
		m_trailRenderer.startWidth = Mathf.Lerp(m_initialStartWidth, 0f, t);
		speed -= speed * (drag * Time.deltaTime);
		Vector3 vector = base.transform.right * speed;
		Vector3 position = base.transform.position + vector * Time.fixedDeltaTime;
		float height = 0f;
		Vector3 normal = Vector3.up;
		if (Terrain.GetTerrainForLayer(terrainLayer).GetHeightAndNormal(position.x, ref height, ref normal))
		{
			if (position.y > height)
			{
				vector.y += gravity * Time.fixedDeltaTime;
				Vector3 normalized = Vector3.Cross(vector.normalized, Vector3.back).normalized;
				base.transform.rotation = Quaternion.LookRotation(Vector3.forward, normalized);
			}
			else
			{
				position.y = height;
				base.transform.rotation = Quaternion.LookRotation(Vector3.forward, normal);
			}
		}
		base.transform.position = position;
	}
}
