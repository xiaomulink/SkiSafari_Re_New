using System;
using UnityEngine;

public class Stunt_Collect : Stunt
{
	[Serializable]
	public class ChainScore
	{
		public string category = "ground";

		public string description = "Gold rush!";

		public float score = 50f;
	}

	public delegate void OnCollectableBoostDelegate(Player player, Collectable collectable);

	public int minCountForBoost = 6;

	public float maxDistanceBetweenCollectables = 2f;

	public ChainScore[] chainScores;

	public GameObject collectEffect;

	public GameObject boostCollectEffect;

	public static OnCollectableBoostDelegate OnCollectableBoost;

	private int m_currentCount;

	private float m_lastPosX;

	protected void OnPlayerCollect(Player player, Collectable collectable)
	{
		if ((bool)CoinMagnet.Instance)
		{
			TransformUtils.Instantiate(collectEffect, player.transform, false, true);
		}
		else if (collectable.allowChains)
		{
			float x = collectable.transform.position.x;
			float num = x - m_lastPosX;
			if (num < maxDistanceBetweenCollectables)
			{
				m_currentCount++;
				if (m_currentCount >= minCountForBoost)
				{
					player.Ignite();
					ChainScore[] array = chainScores;
					foreach (ChainScore chainScore in array)
					{
						if (chainScore.category == collectable.category)
						{
							base.Manager.AddScore(chainScore.score, chainScore.description);
							break;
						}
					}
					m_currentCount = 0;
					TransformUtils.Instantiate(boostCollectEffect, player.transform, false, true);
					if (OnCollectableBoost != null)
					{
						OnCollectableBoost(player, collectable);
					}
				}
				else
				{
					TransformUtils.Instantiate(collectEffect, player.transform, false, true);
				}
			}
			else
			{
				m_currentCount = 1;
				TransformUtils.Instantiate(collectEffect, player.transform, false, true);
			}
			m_lastPosX = x;
		}
		else
		{
			TransformUtils.Instantiate(boostCollectEffect, player.transform, false, true);
		}
	}

	protected override void OnEnable()
	{
		Collectable.OnPlayerCollect = (Collectable.OnPlayerCollectDelegate)Delegate.Combine(Collectable.OnPlayerCollect, new Collectable.OnPlayerCollectDelegate(OnPlayerCollect));
	}

	protected override void OnDisable()
	{
		Collectable.OnPlayerCollect = (Collectable.OnPlayerCollectDelegate)Delegate.Remove(Collectable.OnPlayerCollect, new Collectable.OnPlayerCollectDelegate(OnPlayerCollect));
	}
}
