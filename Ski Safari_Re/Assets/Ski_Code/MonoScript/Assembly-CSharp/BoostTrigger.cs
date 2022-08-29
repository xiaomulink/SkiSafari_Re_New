public class BoostTrigger : PlayerTrigger
{
	public delegate void OnPlayerBoostDelegate(Player player);

	public FlamePowerup.Level ignitionLevel = FlamePowerup.Level.Yellow;

	public static OnPlayerBoostDelegate OnPlayerBoost;

	protected override void OnPlayerTrigger(Player player)
	{
		player.Ignite(ignitionLevel);
		if (OnPlayerBoost != null && PlayerManager.IsHumanPlayer(player))
		{
			OnPlayerBoost(player);
		}
	}
}
