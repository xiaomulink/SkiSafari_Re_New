using System;
using UnityEngine;

public class Comment_HitHazard : Comment
{
	public string category = "rock";

	public float chance = 0.25f;

	public bool skierOnly = true;

	public int minCount = 1;

	private int m_count;

	private void OnHitHazard(Player previousPlayer, Player player, Hazard hazard)
	{
		if (hazard.category == category && (!skierOnly || (!(player is PlayerSkierBellyslide) && !(player is PlayerSkierMounted) && !(player is PlayerSkierSleeping))) && ++m_count > minCount && UnityEngine.Random.value >= chance)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		Player.OnPlayerHitHazard = (Player.OnHitHazardDelegate)Delegate.Combine(Player.OnPlayerHitHazard, new Player.OnHitHazardDelegate(OnHitHazard));
		m_count = 0;
	}

	private void OnDisable()
	{
		Player.OnPlayerHitHazard = (Player.OnHitHazardDelegate)Delegate.Remove(Player.OnPlayerHitHazard, new Player.OnHitHazardDelegate(OnHitHazard));
	}
}
