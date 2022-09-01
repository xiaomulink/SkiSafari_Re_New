using System;

public class Achievement_TriggerEffectBackflip : Achievement_Count
{
	public string requiredMountCategory;

	public string requiredEffectCategory;

	private bool m_hasPassedTrigger;

	private void OnBackflipLanded(Player player, int consecutiveCount)
	{
		if (m_hasPassedTrigger && Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			IncrementCount(1);
			m_hasPassedTrigger = false;
		}
	}

	private void OnBackflipCancelled(Player player)
	{
		m_hasPassedTrigger = false;
	}

	private void OnPlayerTriggerEffect(Player player, EffectTrigger effectTrigger)
	{
		if (effectTrigger.effectCategory == requiredEffectCategory && Achievement.CheckMountCategory(player, requiredMountCategory))
		{
			m_hasPassedTrigger = true;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Stunt_Backflip.OnBackflipLanded = (Stunt_Backflip.OnBackflipLandedDelegate)Delegate.Combine(Stunt_Backflip.OnBackflipLanded, new Stunt_Backflip.OnBackflipLandedDelegate(OnBackflipLanded));
		Stunt_Backflip.OnBackflipCancelled = (Player.SimplePlayerDelegate)Delegate.Combine(Stunt_Backflip.OnBackflipCancelled, new Player.SimplePlayerDelegate(OnBackflipCancelled));
		EffectTrigger.OnPlayerTriggerEffect = (EffectTrigger.OnPlayerTriggerEffectDelegate)Delegate.Combine(EffectTrigger.OnPlayerTriggerEffect, new EffectTrigger.OnPlayerTriggerEffectDelegate(OnPlayerTriggerEffect));
		m_hasPassedTrigger = false;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Stunt_Backflip.OnBackflipLanded = (Stunt_Backflip.OnBackflipLandedDelegate)Delegate.Remove(Stunt_Backflip.OnBackflipLanded, new Stunt_Backflip.OnBackflipLandedDelegate(OnBackflipLanded));
		Stunt_Backflip.OnBackflipCancelled = (Player.SimplePlayerDelegate)Delegate.Remove(Stunt_Backflip.OnBackflipCancelled, new Player.SimplePlayerDelegate(OnBackflipCancelled));
		EffectTrigger.OnPlayerTriggerEffect = (EffectTrigger.OnPlayerTriggerEffectDelegate)Delegate.Combine(EffectTrigger.OnPlayerTriggerEffect, new EffectTrigger.OnPlayerTriggerEffectDelegate(OnPlayerTriggerEffect));
	}
}
