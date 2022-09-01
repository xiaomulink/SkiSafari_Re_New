public class AISkier : AI
{
	public AnimatedSprite sprite;

	private CircleCollider m_collider;

	private void Awake()
	{
		m_collider = GetComponent<CircleCollider>();
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
		base.FixedUpdate();
	}
}
