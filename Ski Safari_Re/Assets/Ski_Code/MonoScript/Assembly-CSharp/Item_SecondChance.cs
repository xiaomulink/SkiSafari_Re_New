public class Item_SecondChance : Item
{
	public Player secondChancePlayerPrefab;

	protected override void OnEnable()
	{
		base.OnEnable();
		SkiGameManager.Instance.secondChancePlayerPrefab = secondChancePlayerPrefab;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SkiGameManager.Instance.secondChancePlayerPrefab = null;
	}
}
