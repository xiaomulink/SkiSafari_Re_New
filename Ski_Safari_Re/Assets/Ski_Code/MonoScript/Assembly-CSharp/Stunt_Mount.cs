using System;

public class Stunt_Mount : Stunt
{
	[Serializable]
	public class MountScore
	{
		public string description = "Mounted!";

		public float score = 100f;
	}

	[Serializable]
	public class MountDescriptor
	{
		public string mountCategory;

		public MountScore ground;

		public MountScore groundTransfer;

		public MountScore air;

		public MountScore airTransfer;
	}

	public MountDescriptor[] mountDescriptors;

	protected void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		MountDescriptor mountDescriptor = null;
		MountDescriptor[] array = mountDescriptors;
		foreach (MountDescriptor mountDescriptor2 in array)
		{
			if (mountedPlayer.Category == mountDescriptor2.mountCategory)
			{
				mountDescriptor = mountDescriptor2;
				break;
			}
		}
		if (mountDescriptor != null)
		{
			bool flag = previousPlayer.Collider.OnGround || mountedPlayer.Collider.OnGround;
			if (previousPlayer is PlayerSkierMounted)
			{
				MountScore mountScore = ((!flag) ? mountDescriptor.airTransfer : mountDescriptor.groundTransfer);
				base.Manager.AddScore(mountScore.score, mountScore.description);
			}
			else
			{
				MountScore mountScore2 = ((!flag) ? mountDescriptor.air : mountDescriptor.ground);
				base.Manager.AddScore(mountScore2.score, mountScore2.description);
			}
		}
	}

	protected override void OnEnable()
	{
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
	}

	protected override void OnDisable()
	{
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
	}
}
