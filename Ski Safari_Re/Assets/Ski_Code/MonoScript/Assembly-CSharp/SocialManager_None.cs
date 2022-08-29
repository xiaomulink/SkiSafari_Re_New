public class SocialManager_None : SocialManager
{
	public override bool CanAuthenticate
	{
		get
		{
			return false;
		}
	}

	public override bool IsAuthenticated
	{
		get
		{
			return false;
		}
	}

	public override string PlayerName
	{
		get
		{
			return string.Empty;
		}
	}

	public override string PlayerIdentifier
	{
		get
		{
			return string.Empty;
		}
	}

	protected override void DoAuthenticate(bool force)
	{
		HandleSignInFailed("Social functions not supported");
	}

	protected override void DoSignOut()
	{
	}

	protected override void DoSubmitScore(string leaderboardId, long score)
	{
		HandleSubmitScoreFailed("Social functions not supported");
	}

	protected override void DoSetAchievementProgress(string achievementId, int progress)
	{
	}

	protected override void DoUnlockAchievement(string achievementId)
	{
	}

	protected override void DoLoadScores(string leaderboardId, LeaderboardUserScope userScope)
	{
		HandleLoadScoresFailed("Social functions not supported");
	}

	protected override void DoShowSettings()
	{
	}

	protected override void DoShowLeaderboards()
	{
	}

	protected override void DoShowLeaderboard(string leaderboardId)
	{
	}

	protected override void DoShowAchievements()
	{
	}
}
