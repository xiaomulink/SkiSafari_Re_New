using System;

public class Achievement_PassThroughDoor : Achievement_Count
{
	public string requiredMountCategory;

	public AchievementPlayerType requiredPlayerType;

	public string requiredDoorCategory;

	private void OnPlayerOpenDoor(Player player, DoorTrigger doorTrigger)
	{
		if (!(SkiGameManager.Instance.CurrentDistance < 10f) && Achievement.CheckMountCategory(player, requiredMountCategory) && Achievement.CheckPlayerType(player, requiredPlayerType) && (string.IsNullOrEmpty(requiredDoorCategory) || doorTrigger.doorCategory == requiredDoorCategory))
		{
			IncrementCount(1);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		DoorTrigger.OnPlayerOpenDoor = (DoorTrigger.OnPlayerOpenDoorDelegate)Delegate.Combine(DoorTrigger.OnPlayerOpenDoor, new DoorTrigger.OnPlayerOpenDoorDelegate(OnPlayerOpenDoor));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		DoorTrigger.OnPlayerOpenDoor = (DoorTrigger.OnPlayerOpenDoorDelegate)Delegate.Remove(DoorTrigger.OnPlayerOpenDoor, new DoorTrigger.OnPlayerOpenDoorDelegate(OnPlayerOpenDoor));
	}
}
