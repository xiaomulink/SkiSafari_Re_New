public class Achievement_PassThroughCave : Achievement_Count
{
	public string requiredMountCategory;

	private bool m_wasInCave;

	private bool m_wasOnCorrectMount;

	protected void FixedUpdate()
	{
		Player instance = Player.Instance;
		if ((bool)instance)
		{
			if (Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPointInCave(instance.transform.position))
			{
				if (!m_wasInCave)
				{
					m_wasInCave = true;
					m_wasOnCorrectMount = Achievement.CheckMountCategory(instance, requiredMountCategory);
				}
				return;
			}
			if (m_wasInCave && m_wasOnCorrectMount && Achievement.CheckMountCategory(instance, requiredMountCategory))
			{
				IncrementCount(1);
			}
		}
		m_wasInCave = false;
		m_wasOnCorrectMount = false;
	}
}
