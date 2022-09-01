public class SnowballTrigger : Hazard_Rect
{
	protected override void HitPlayer(Player player)
	{
		if (!player.Snowball(this))
		{
			base.HitPlayer(player);
		}
	}
}
