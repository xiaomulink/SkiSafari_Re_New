using System;

public class Achievement_Backflip : Achievement_Count
{
	public string requiredMountCategory;

	public int requiredConsecutiveCount = 1;

	public bool requiresCaves;

	public string requiredLineCategory;

	private string m_lastLineCategory;

	private string m_nextLineCategory;

	private void OnBackflipLanded(Player player, int consecutiveCount)
	{
		if (consecutiveCount >= requiredConsecutiveCount && Achievement.CheckMountCategory(player, requiredMountCategory) && (!requiresCaves || Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPointInCave(Player.Instance.transform.position)) && (string.IsNullOrEmpty(requiredLineCategory) || m_lastLineCategory == requiredLineCategory))
		{
			IncrementCount(consecutiveCount / requiredConsecutiveCount);
		}
	}

	private void OnContact(Player player, GeometryUtils.ContactInfo contactInfo)
	{
		m_nextLineCategory = ((!contactInfo.collider) ? string.Empty : contactInfo.collider.category);
	}

	private void OnTakeoff(Player player)
	{
		m_lastLineCategory = m_nextLineCategory;
		m_nextLineCategory = string.Empty;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Stunt_Backflip.OnBackflipLanded = (Stunt_Backflip.OnBackflipLandedDelegate)Delegate.Combine(Stunt_Backflip.OnBackflipLanded, new Stunt_Backflip.OnBackflipLandedDelegate(OnBackflipLanded));
		if (!string.IsNullOrEmpty(requiredLineCategory))
		{
			Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Combine(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
			Player.OnPlayerTakeoff = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerTakeoff, new Player.SimplePlayerDelegate(OnTakeoff));
			m_lastLineCategory = (m_nextLineCategory = string.Empty);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Stunt_Backflip.OnBackflipLanded = (Stunt_Backflip.OnBackflipLandedDelegate)Delegate.Remove(Stunt_Backflip.OnBackflipLanded, new Stunt_Backflip.OnBackflipLandedDelegate(OnBackflipLanded));
		if (!string.IsNullOrEmpty(requiredLineCategory))
		{
			Player.OnPlayerContact = (Player.OnCollisionDelegate)Delegate.Remove(Player.OnPlayerContact, new Player.OnCollisionDelegate(OnContact));
			Player.OnPlayerTakeoff = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerTakeoff, new Player.SimplePlayerDelegate(OnTakeoff));
		}
	}
}
