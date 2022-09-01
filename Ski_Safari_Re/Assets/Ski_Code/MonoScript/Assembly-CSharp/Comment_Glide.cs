using System;

public class Comment_Glide : Comment
{
	public FlamePowerup.Level requiredLevel;

	private void OnGlideIgnite(Player player, FlamePowerup.Level level)
	{
		if (level >= requiredLevel)
		{
			ShowMessagesAndComplete();
		}
	}

	private void OnEnable()
	{
		Stunt_Glide.OnGlideIgnite = (Stunt_Glide.OnGlideIgniteDelegate)Delegate.Combine(Stunt_Glide.OnGlideIgnite, new Stunt_Glide.OnGlideIgniteDelegate(OnGlideIgnite));
	}

	private void OnDisable()
	{
		Stunt_Glide.OnGlideIgnite = (Stunt_Glide.OnGlideIgniteDelegate)Delegate.Remove(Stunt_Glide.OnGlideIgnite, new Stunt_Glide.OnGlideIgniteDelegate(OnGlideIgnite));
	}
}
