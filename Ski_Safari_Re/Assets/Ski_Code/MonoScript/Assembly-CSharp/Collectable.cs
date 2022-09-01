using UnityEngine;

public class Collectable : MonoBehaviour
{
	public delegate void OnPlayerCollectDelegate(Player player, Collectable collectable);

	public int count = 1;

	public float radius = 1f;

	public string category = "ground";

	public bool affectedByMagnet = true;

	public bool despawnOnCollect = true;

	public bool allowChains = true;

	public bool igniteOnCollect;

	public GameObject collectEffect;

	public static OnPlayerCollectDelegate OnPlayerCollect;

	private float m_pickupRadius;

	private bool m_moveToPlayer;

	public void PlayerCollect(Player player)
	{
		SkiGameManager.Instance.IncrementCollectableCount(count);
		if (igniteOnCollect)
		{
			player.Ignite();
		}
		if (OnPlayerCollect != null && PlayerManager.IsHumanPlayer(player))
		{
			OnPlayerCollect(player, this);
		}
		if ((bool)collectEffect)
		{
			Pool.Spawn(collectEffect, base.transform.position, base.transform.rotation);
		}
		if (despawnOnCollect)
		{
			Pool.Despawn(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		m_moveToPlayer = false;
		m_pickupRadius = ((!CoinMagnet.Instance || !affectedByMagnet) ? radius : CoinMagnet.Instance.attractionRadius);
	}

	private void FixedUpdate()
	{
		if (m_moveToPlayer || !Player.Instance)
		{
			return;
		}
		Player instance = Player.Instance;
		if (((Vector2)(instance.transform.position - base.transform.position)).magnitude < m_pickupRadius + instance.Collider.radius)
		{
			if (affectedByMagnet && (bool)CoinMagnet.Instance)
			{
				CoinMagnet.Instance.Attract(this);
				m_moveToPlayer = true;
			}
			else
			{
				PlayerCollect(instance);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
