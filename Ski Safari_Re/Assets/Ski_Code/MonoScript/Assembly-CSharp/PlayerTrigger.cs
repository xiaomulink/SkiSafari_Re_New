using UnityEngine;

public abstract class PlayerTrigger : MonoBehaviour
{
	public float radius = 1f;

	protected float m_sqrRadius;

	private bool m_playerInsideTrigger;

	protected virtual PlayerManager.PlayerGroup PlayerGroup
	{
		get
		{
			return PlayerManager.PlayerGroup.Human;
		}
	}

	protected abstract void OnPlayerTrigger(Player player);

	protected virtual void Awake()
	{
		m_sqrRadius = radius * radius;
	}

	protected virtual void OnEnable()
	{
		m_playerInsideTrigger = false;
	}

	protected virtual void FixedUpdate()
	{
		foreach (Player player in PlayerManager.GetPlayers(PlayerGroup))
		{
			if (((Vector2)(base.transform.position - player.transform.position)).magnitude < radius + player.Collider.radius)
			{
				if (!m_playerInsideTrigger)
				{
					OnPlayerTrigger(player);
					m_playerInsideTrigger = true;
				}
				return;
			}
		}
		m_playerInsideTrigger = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
