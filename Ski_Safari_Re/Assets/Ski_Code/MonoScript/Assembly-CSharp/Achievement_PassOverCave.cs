public class Achievement_PassOverCave : Achievement_Count
{
	public string requiredMountCategory;

	private bool m_wasOverCave;

	private bool m_wasOnCorrectMount;

	protected void FixedUpdate()
	{
		Player instance = Player.Instance;
		if ((bool)instance)
		{
			if (Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPointAboveCave(instance.transform.position))
			{
				if (!m_wasOverCave)
				{
					m_wasOverCave = true;
					m_wasOnCorrectMount = Achievement.CheckMountCategory(instance, requiredMountCategory);
				}
				return;
			}
			if (m_wasOverCave && m_wasOnCorrectMount && Achievement.CheckMountCategory(instance, requiredMountCategory))
			{
				IncrementCount(1);
			}
		}
		m_wasOverCave = false;
		m_wasOnCorrectMount = false;
	}
}
