using System;

public class Stunt_SmashHazard : Stunt
{
	[Serializable]
	public class HazardDescriptor
	{
		public string hazardCategory;

		public string description = "Smashing!";

		public float score = 100f;

		public bool immunitiesOnly;
	}

	public HazardDescriptor[] hazardDescriptors;

	protected void OnHitHazard(Player previousPlayer, Player player, Hazard hazard)
	{
		HazardDescriptor hazardDescriptor = null;
		HazardDescriptor[] array = hazardDescriptors;
		foreach (HazardDescriptor hazardDescriptor2 in array)
		{
			if (hazardDescriptor2.hazardCategory == hazard.category)
			{
				hazardDescriptor = hazardDescriptor2;
				break;
			}
		}
		if (hazardDescriptor == null)
		{
			return;
		}
		if (hazardDescriptor.immunitiesOnly)
		{
			if (!player.IsImmuneToHazard(hazard))
			{
				return;
			}
		}
		else if (!player.IsInvulnerable)
		{
			return;
		}
		base.Manager.AddScore(hazardDescriptor.score, hazardDescriptor.description);
	}

	protected override void OnEnable()
	{
		Player.OnPlayerHitHazard = (Player.OnHitHazardDelegate)Delegate.Combine(Player.OnPlayerHitHazard, new Player.OnHitHazardDelegate(OnHitHazard));
	}

	protected override void OnDisable()
	{
		Player.OnPlayerHitHazard = (Player.OnHitHazardDelegate)Delegate.Remove(Player.OnPlayerHitHazard, new Player.OnHitHazardDelegate(OnHitHazard));
	}
}
