using System;

public class Stunt_TriggerEffect : Stunt
{
	[Serializable]
	public class TriggerDescriptor
	{
		public string category = "snowman_hat";

		public string description = "Hat Trick!";

		public float score = 200f;
	}

	public delegate void OnCollectableBoostDelegate(Player player, Collectable collectable);

	public TriggerDescriptor[] triggerDescriptors;

	public static OnCollectableBoostDelegate OnCollectableBoost;

	protected void OnPlayerTriggerEffect(Player player, EffectTrigger effectTrigger)
	{
		TriggerDescriptor[] array = triggerDescriptors;
		foreach (TriggerDescriptor triggerDescriptor in array)
		{
			if (triggerDescriptor.category == effectTrigger.effectCategory)
			{
				base.Manager.AddScore(triggerDescriptor.score, triggerDescriptor.description);
			}
		}
	}

	protected override void OnEnable()
	{
		EffectTrigger.OnPlayerTriggerEffect = (EffectTrigger.OnPlayerTriggerEffectDelegate)Delegate.Combine(EffectTrigger.OnPlayerTriggerEffect, new EffectTrigger.OnPlayerTriggerEffectDelegate(OnPlayerTriggerEffect));
	}

	protected override void OnDisable()
	{
		EffectTrigger.OnPlayerTriggerEffect = (EffectTrigger.OnPlayerTriggerEffectDelegate)Delegate.Remove(EffectTrigger.OnPlayerTriggerEffect, new EffectTrigger.OnPlayerTriggerEffectDelegate(OnPlayerTriggerEffect));
	}
}
