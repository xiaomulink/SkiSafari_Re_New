using System;

public class Achievement_Piledrive : Achievement_Count
{
	public enum GroundType
	{
		Any = 0,
		InsideCave = 1,
		AboveCave = 2
	}

	public GroundType groundType;

	private bool CheckGroundType(Player player)
	{
		switch (groundType)
		{
		case GroundType.InsideCave:
			return Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPointInCave(player.transform.position);
		case GroundType.AboveCave:
			return Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPointAboveCave(player.transform.position);
		default:
			return true;
		}
	}

	private void OnPlayerTakeDamage(Player previousPlayer, Player player)
	{
		if (player is PlayerPiledrive && CheckGroundType(player))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Combine(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnPlayerTakeDamage));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Remove(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnPlayerTakeDamage));
	}
}
