using System;

public class Stunt_BreakLine : Stunt
{
	[Serializable]
	public class MountDescriptor
	{
		public string mountCategory;

		public string description = "Slide!";

		public float score = 50f;
	}

	[Serializable]
	public class LineDescriptor
	{
		public string colliderCategory;

		public MountDescriptor[] mountDescriptors;

		public MountDescriptor defaultDescriptor;
	}

	public LineDescriptor[] lineDescriptors;

	private void OnBreakLine(Player player, GeometryUtils.ContactInfo contactInfo)
	{
		string category = contactInfo.collider.category;
		LineDescriptor lineDescriptor = null;
		LineDescriptor[] array = lineDescriptors;
		foreach (LineDescriptor lineDescriptor2 in array)
		{
			if (lineDescriptor2.colliderCategory == category)
			{
				lineDescriptor = lineDescriptor2;
				break;
			}
		}
		if (lineDescriptor == null)
		{
			return;
		}
		MountDescriptor mountDescriptor = lineDescriptor.defaultDescriptor;
		PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
		if ((bool)playerSkierMounted)
		{
			MountDescriptor[] mountDescriptors = lineDescriptor.mountDescriptors;
			foreach (MountDescriptor mountDescriptor2 in mountDescriptors)
			{
				if (playerSkierMounted.mountCategory == mountDescriptor2.mountCategory)
				{
					mountDescriptor = mountDescriptor2;
					break;
				}
			}
		}
		base.Manager.AddScore(mountDescriptor.score, mountDescriptor.description);
	}

	protected override void OnEnable()
	{
		Player.OnPlayerBreakLine = (Player.OnCollisionDelegate)Delegate.Combine(Player.OnPlayerBreakLine, new Player.OnCollisionDelegate(OnBreakLine));
	}

	protected override void OnDisable()
	{
		Player.OnPlayerBreakLine = (Player.OnCollisionDelegate)Delegate.Remove(Player.OnPlayerBreakLine, new Player.OnCollisionDelegate(OnBreakLine));
	}
}
