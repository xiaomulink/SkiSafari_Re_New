using System.Collections.Generic;

public class SocialManager_Test : SocialManager
{
	public bool forceAuthenticationsOnly = true;

	public float authenticateDelay = 2f;

	public float loadScoresDelay = 1f;

	private bool m_isAuthenticated;

	public override bool CanAuthenticate
	{
		get
		{
			return true;
		}
	}

	public override bool IsAuthenticated
	{
		get
		{
			return m_isAuthenticated;
		}
	}

	public override string PlayerName
	{
		get
		{
			return "test player";
		}
	}

	public override string PlayerIdentifier
	{
		get
		{
			return "test player id";
		}
	}

	protected override void DoAuthenticate(bool force)
	{
		if (force || !forceAuthenticationsOnly)
		{
			CancelInvoke("FinishAuthenticate");
			Invoke("FinishAuthenticate", authenticateDelay);
		}
	}

	protected override void DoSignOut()
	{
		m_isAuthenticated = false;
		HandleSignOutSucceeded();
	}

	protected override void DoSubmitScore(string leaderboardId, long score)
	{
		HandleSubmitScoreSucceeded();
	}

	protected override void DoSetAchievementProgress(string achievementId, int progress)
	{
	}

	protected override void DoUnlockAchievement(string achievementId)
	{
	}

	protected override void DoLoadScores(string leaderboardId, LeaderboardUserScope userScope)
	{
		CancelInvoke("FinishLoadScores");
		Invoke("FinishLoadScores", loadScoresDelay);
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

	private void FinishAuthenticate()
	{
		m_isAuthenticated = true;
		HandleSignInSucceeded();
	}

	private void FinishLoadScores()
	{
		List<LeaderboardScore> list = new List<LeaderboardScore>();
		LeaderboardScore leaderboardScore = new LeaderboardScore();
		leaderboardScore.name = "brendan";
		leaderboardScore.rank = "1";
		leaderboardScore.score = 10000000L;
		list.Add(leaderboardScore);
		HandleLoadScoresSucceeded(list);
	}
}
