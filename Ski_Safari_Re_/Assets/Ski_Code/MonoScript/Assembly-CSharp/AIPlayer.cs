using UnityEngine;

public class AIPlayer : AI
{
	public AnimatedSprite sprite;

	public float speedMultiplierWhenBehind = 1f;

	public float distanceOffsetForSpeedMultiplier;

	public float maxDistanceForSpeedMultiplier;

	private CircleCollider m_collider;

	private float m_normalMaxSpeed;

	private void Awake()
	{
		m_collider = GetComponent<CircleCollider>();
		m_normalMaxSpeed = m_collider.maxSpeed;
	}

	protected override void FixedUpdate()
	{
		if ((bool)sprite)
		{
			if (m_collider.OnGround)
			{
				if (sprite.CurrentAnimation != "idle")
				{
					sprite.PlayAnimation("idle");
				}
			}
			else if (sprite.CurrentAnimation != "glide")
			{
				sprite.PlayAnimation("glide");
			}
		}
		m_collider.maxSpeed = m_normalMaxSpeed;
		if (maxDistanceForSpeedMultiplier > 0f && (bool)Player.Instance)
		{
			float num = Player.Instance.transform.position.x - base.transform.position.x + distanceOffsetForSpeedMultiplier;
			if (num > 0f)
			{
				float num2 = Mathf.Min(1f, num / maxDistanceForSpeedMultiplier);
				m_collider.maxSpeed = m_normalMaxSpeed * (1f + num2 * (speedMultiplierWhenBehind - 1f));
			}
		}
		base.FixedUpdate();
	}
}
