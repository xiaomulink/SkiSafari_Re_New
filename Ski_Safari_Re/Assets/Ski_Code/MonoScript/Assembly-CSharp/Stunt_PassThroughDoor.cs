using System;

public class Stunt_PassThroughDoor : Stunt
{
	[Serializable]
	public class MountDescriptor
	{
		public string mountCategory;

		public string description = "Mounted!";

		public float score = 100f;
	}

	public delegate void OnPassThroughDoorDelegate(Player player, MountDescriptor mountDescriptor);

	public MountDescriptor[] mountDescriptors;

	public static OnPassThroughDoorDelegate OnPassThroughDoor;

	protected void OnPlayerOpenDoor(Player player, DoorTrigger doorTrigger)
	{
		if (SkiGameManager.Instance.CurrentDistance < 10f)
		{
			return;
		}
		PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
		if (!playerSkierMounted)
		{
			return;
		}
		MountDescriptor mountDescriptor = null;
		MountDescriptor[] array = mountDescriptors;
		foreach (MountDescriptor mountDescriptor2 in array)
		{
			if (playerSkierMounted.mountCategory == mountDescriptor2.mountCategory)
			{
				mountDescriptor = mountDescriptor2;
				break;
			}
		}
		if (mountDescriptor != null)
		{
			if (OnPassThroughDoor != null)
			{
				OnPassThroughDoor(player, mountDescriptor);
			}
			base.Manager.AddScore(mountDescriptor.score, mountDescriptor.description);
		}
	}

	protected override void OnEnable()
	{
		DoorTrigger.OnPlayerOpenDoor = (DoorTrigger.OnPlayerOpenDoorDelegate)Delegate.Combine(DoorTrigger.OnPlayerOpenDoor, new DoorTrigger.OnPlayerOpenDoorDelegate(OnPlayerOpenDoor));
	}

	protected override void OnDisable()
	{
		DoorTrigger.OnPlayerOpenDoor = (DoorTrigger.OnPlayerOpenDoorDelegate)Delegate.Remove(DoorTrigger.OnPlayerOpenDoor, new DoorTrigger.OnPlayerOpenDoorDelegate(OnPlayerOpenDoor));
	}
}
