using UnityEngine;

public class PlayerSkierSleeping : PlayerSkier
{
	public AnimatedSprite bedSprite;

	public float delayBeforeWake = 2f;

	private float m_wakeTimer;

	public override string Category
	{
		get
		{
			return "sleeping";
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_wakeTimer = delayBeforeWake;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.IsDisabling && m_wakeTimer > 0f)
		{
			m_wakeTimer -= Time.deltaTime;
			if (m_wakeTimer <= 0f)
			{
				bedSprite.PlayAnimation("wake");
			}
		}
	}
}
