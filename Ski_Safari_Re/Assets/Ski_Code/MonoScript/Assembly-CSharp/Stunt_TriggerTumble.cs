using System;

public class Stunt_TriggerTumble : Stunt
{
	[Serializable]
	public class Descriptor
	{
		public string category = "llama";

		public string description = "Llama bowler!";

		public float score = 200f;
	}

	public Descriptor[] descriptors;

	protected void OnTumble(PlayerInteractionTrigger trigger, Player sourcePlayer)
	{
		Descriptor[] array = descriptors;
		foreach (Descriptor descriptor in array)
		{
			if (descriptor.category == trigger.category)
			{
				base.Manager.AddScore(descriptor.score, descriptor.description);
			}
		}
	}

	protected override void OnEnable()
	{
		PlayerInteractionTrigger.OnTumble = (PlayerInteractionTrigger.OnTumbleDelegate)Delegate.Combine(PlayerInteractionTrigger.OnTumble, new PlayerInteractionTrigger.OnTumbleDelegate(OnTumble));
	}

	protected override void OnDisable()
	{
		PlayerInteractionTrigger.OnTumble = (PlayerInteractionTrigger.OnTumbleDelegate)Delegate.Remove(PlayerInteractionTrigger.OnTumble, new PlayerInteractionTrigger.OnTumbleDelegate(OnTumble));
	}
}
