using System;

public class SocialAchievement_AttainRank : SocialAchievement
{
	public int requiredRank;

	public override void Load()
	{
		LevelManager.OnLoaded = (Action)Delegate.Combine(LevelManager.OnLoaded, new Action(CheckRank));
		CheckRank();
	}

	public override void Unload()
	{
		LevelManager.OnLoaded = (Action)Delegate.Remove(LevelManager.OnLoaded, new Action(CheckRank));
	}

	protected void CheckRank()
	{
		int currentLevel = LevelManager.Instance.CurrentLevel;
		if (currentLevel >= requiredRank)
		{
			SocialManager.Instance.UnlockAchievement(this);
		}
	}
}
