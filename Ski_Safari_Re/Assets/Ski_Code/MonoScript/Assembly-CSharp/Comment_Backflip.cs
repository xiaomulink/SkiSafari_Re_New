using System;

public class Comment_Backflip : Comment
{
	public int requiredConsecutiveCount = 1;

	private void OnBackflipLanded(Player player, int consecutiveCount)
	{
		if (consecutiveCount >= requiredConsecutiveCount)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		Stunt_Backflip.OnBackflipLanded = (Stunt_Backflip.OnBackflipLandedDelegate)Delegate.Combine(Stunt_Backflip.OnBackflipLanded, new Stunt_Backflip.OnBackflipLandedDelegate(OnBackflipLanded));
	}

	private void OnDisable()
	{
		Stunt_Backflip.OnBackflipLanded = (Stunt_Backflip.OnBackflipLandedDelegate)Delegate.Remove(Stunt_Backflip.OnBackflipLanded, new Stunt_Backflip.OnBackflipLandedDelegate(OnBackflipLanded));
	}
}
