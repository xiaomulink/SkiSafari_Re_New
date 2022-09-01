using System.Collections.Generic;
using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
	public static CoinMagnet Instance;

	public ParticleSystem particles;

	public float attractionRadius = 15f;

	public float attractSpeed = 20f;

	public float collectRadius = 1.5f;

	private List<Collectable> m_coins = new List<Collectable>();

	public void Attract(Collectable coin)
	{
		if (!m_coins.Contains(coin))
		{
			m_coins.Add(coin);
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		m_coins.Clear();
	}

	private void LateUpdate()
	{
		Player instance = Player.Instance;
		if ((bool)instance)
		{
			Vector3 position = instance.transform.position;
			Vector3 velocity = instance.Collider.velocity;
			base.transform.position = position;
			int num = 0;
			while (num < m_coins.Count)
			{
				Collectable collectable = m_coins[num];
				Vector3 vector = position - collectable.transform.position;
				float num2 = vector.magnitude;
				if (num2 > 0f)
				{
					Vector3 vector2 = vector * (1f / num2);
					float num3 = Mathf.Max(0f, Vector3.Dot(vector2, velocity)) + attractSpeed;
					float num4 = num3 * Time.deltaTime;
					Vector3 position2 = collectable.transform.position + vector2 * num4;
					collectable.transform.position = position2;
					num2 -= num4;
				}
				if (num2 <= collectable.radius)
				{
					collectable.PlayerCollect(instance);
					m_coins.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
		else
		{
			m_coins.Clear();
		}
        try
        {
            particles.enableEmission = m_coins.Count > 0;
        }
        catch
        {

        }
	}
}
