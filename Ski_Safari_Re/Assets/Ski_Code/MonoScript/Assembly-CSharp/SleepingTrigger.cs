using UnityEngine;

public class SleepingTrigger : MonoBehaviour
{
	public ParticleSystem particles;

	public AnimatedSprite sprite;

	public float radius = 2f;

	private float m_sqrRadius;

	private void Wake()
	{
		sprite.PlayAnimation("wake");
		particles.enableEmission = false;
		CancelInvoke("Sleep");
		Invoke("Sleep", 1.3f);
	}

	private void Sleep()
	{
		sprite.PlayAnimation("sleeping");
		particles.enableEmission = true;
	}

	private void Awake()
	{
		m_sqrRadius = radius * radius;
	}

	private void Update()
	{
		foreach (Skier allSkier in Skier.AllSkiers)
		{
			if (allSkier.Collider.terrainLayer == TerrainLayer.Game && ((Vector2)(base.transform.position - allSkier.transform.position)).sqrMagnitude < m_sqrRadius + allSkier.Collider.SqrRadius && CameraUtils.IsPointVisible(allSkier.transform.position))
			{
				CancelInvoke("Wake");
				Invoke("Wake", 0.2f);
				break;
			}
		}
	}
}
