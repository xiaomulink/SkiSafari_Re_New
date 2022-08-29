using UnityEngine;

public class AILaunched : AI
{
	public float revertDelay = 4f;

	public GameObject revertPrefab;

	public Sound revertSound;

	private CircleCollider m_collider;

	private float m_lifeTimer;

	private void Awake()
	{
		m_collider = GetComponent<CircleCollider>();
	}

	private void OnEnable()
	{
		m_lifeTimer = 0f;
	}

	protected override void FixedUpdate()
	{
		m_lifeTimer += Time.deltaTime;
		if (m_lifeTimer > revertDelay || m_collider.OnGround)
		{
			GameObject gameObject = Pool.Spawn(revertPrefab, base.transform.position, base.transform.rotation);
			CircleCollider component = gameObject.GetComponent<CircleCollider>();
			if ((bool)component)
			{
				component.velocity = m_collider.velocity;
			}
			if ((bool)revertSound)
			{
				VOSource component2 = gameObject.GetComponent<VOSource>();
				if ((bool)component2)
				{
					component2.PlayOneShotVO(revertSound, true);
				}
			}
			Pool.Despawn(base.gameObject);
		}
		else
		{
			base.FixedUpdate();
		}
	}
}
