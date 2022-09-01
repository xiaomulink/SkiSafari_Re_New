using System;

public class Achievement_Credits : Achievement_Count
{
	public string requiredMountCategory;

	private bool m_running;

	public override bool HasBasicRequirements
	{
		get
		{
			if (!SkiGameManager.Instance.Started || (CreditsSpawner.Instance.EnableSpawning && (bool)Player.Instance && Achievement.CheckMountCategory(Player.Instance, requiredMountCategory)))
			{
				return base.HasBasicRequirements;
			}
			return false;
		}
	}

	private void OnPlayerSpawn(Player player)
	{
		if (CreditsSpawner.Instance.EnableSpawning && Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			m_running = true;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_running = false;
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Combine(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Remove(instance.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
	}

	protected void Update()
	{
		if (m_running)
		{
			if (!Player.Instance || !Achievement.CheckMountCategory(Player.Instance, requiredMountCategory) || !SkiGameManager.Instance.Playing)
			{
				m_running = false;
			}
			else if (!CreditsSpawner.Instance.EnableSpawning)
			{
				m_running = false;
				IncrementCount(1);
			}
		}
	}
}
