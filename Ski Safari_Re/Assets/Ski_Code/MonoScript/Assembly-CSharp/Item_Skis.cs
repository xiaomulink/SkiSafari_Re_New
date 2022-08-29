public class Item_Skis : Item
{
	public float skierLateralDragRatio = 1f;

	public float skierGroundSuctionRatio = 1f;

	protected override void OnEnable()
	{
		base.OnEnable();
		PlayerSkier.skierLateralDragRatio = skierLateralDragRatio;
		PlayerSkier.skierGroundSuctionRatio = skierGroundSuctionRatio;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		PlayerSkier.skierLateralDragRatio = 1f;
		PlayerSkier.skierGroundSuctionRatio = 1f;
	}
}
