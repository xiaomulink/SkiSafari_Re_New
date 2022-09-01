using System;

public class Stunt_SnowballRecover : Stunt
{
	[Serializable]
	public class Descriptor
	{
		public string category = "pandarolling";

		public string description = "Righteous Panda!";

		public float score = 200f;
	}

	public Descriptor[] descriptors;

	private void OnPlayerRecover(PlayerSnowball previousPlayer, Player player)
	{
		Descriptor[] array = descriptors;
		foreach (Descriptor descriptor in array)
		{
			if (descriptor.category == previousPlayer.Category)
			{
				base.Manager.AddScore(descriptor.score, descriptor.description);
			}
		}
	}

	protected override void OnEnable()
	{
		PlayerSnowball.OnPlayerRecover = (PlayerSnowball.OnPlayerRecoverDelegate)Delegate.Combine(PlayerSnowball.OnPlayerRecover, new PlayerSnowball.OnPlayerRecoverDelegate(OnPlayerRecover));
	}

	protected override void OnDisable()
	{
		PlayerSnowball.OnPlayerRecover = (PlayerSnowball.OnPlayerRecoverDelegate)Delegate.Remove(PlayerSnowball.OnPlayerRecover, new PlayerSnowball.OnPlayerRecoverDelegate(OnPlayerRecover));
	}
}
