using System;

public class Achievement_SecondChance : Achievement_Count
{
	public float requiredMaxDistance = -1f;

	public override bool HasBasicRequirements
	{
		get
		{
			if (!SkiGameManager.Instance.Started || ((bool)Player.Instance && Player.Instance.transform.position.x < requiredMaxDistance))
			{
				return base.HasBasicRequirements;
			}
			return false;
		}
	}

	private void OnSecondChance(Player player)
	{
		if (requiredMaxDistance < 0f || player.transform.position.x < requiredMaxDistance)
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnSecondChance = (Player.SimplePlayerDelegate)Delegate.Combine(instance.OnSecondChance, new Player.SimplePlayerDelegate(OnSecondChance));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnSecondChance = (Player.SimplePlayerDelegate)Delegate.Remove(instance.OnSecondChance, new Player.SimplePlayerDelegate(OnSecondChance));
	}
}
