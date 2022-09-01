public class FreezeTrigger : Hazard_Rect
{
	protected override void HitPlayer(Player player)
	{
		player.Freeze();
	}
}
