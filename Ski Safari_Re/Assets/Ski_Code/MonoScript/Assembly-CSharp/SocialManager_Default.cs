using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SocialManager_Default : SocialManager
{
	private ILeaderboard m_currentLeaderboard;

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
			return Social.localUser.authenticated;
		}
	}

	public override string PlayerName
	{
		get
		{
			return Social.localUser.userName;
		}
	}

	public override string PlayerIdentifier
	{
		get
		{
			return Social.localUser.id;
		}
	}

	protected override void DoAuthenticate(bool force)
	{
		Social.localUser.Authenticate(OnAuthenticateFinished);
	}

	protected override void DoSignOut()
	{
	}

	protected override void DoSubmitScore(string leaderboardId, long score)
	{
		Social.ReportScore(score, leaderboardId, OnReportScoreFinished);
	}

	protected override void DoSetAchievementProgress(string achievementId, int progress)
	{
		Social.ReportProgress(achievementId, progress, OnReportProgressFinished);
	}

	protected override void DoUnlockAchievement(string achievementId)
	{
		Social.ReportProgress(achievementId, 1.0, OnReportProgressFinished);
	}

	protected override void DoLoadScores(string leaderboardId, LeaderboardUserScope userScope)
	{
		m_currentLeaderboard = Social.CreateLeaderboard();
		m_currentLeaderboard.id = leaderboardId;
		m_currentLeaderboard.timeScope = TimeScope.AllTime;
		if (userScope == LeaderboardUserScope.Friends)
		{
			m_currentLeaderboard.userScope = UserScope.FriendsOnly;
		}
		else
		{
			m_currentLeaderboard.userScope = UserScope.Global;
		}
		m_currentLeaderboard.LoadScores(OnLoadScoresFinished);
	}

	protected override void DoShowSettings()
	{
	}

	protected override void DoShowLeaderboards()
	{
	}

	protected override void DoShowLeaderboard(string leaderboardId)
	{
		Social.ShowLeaderboardUI();
	}

	protected override void DoShowAchievements()
	{
		Social.ShowAchievementsUI();
	}

	private void OnAuthenticateFinished(bool success)
	{
		if (success)
		{
			HandleSignInSucceeded();
		}
		else
		{
			HandleSignInFailed("Failed to authenticate");
		}
	}

	private void OnReportScoreFinished(bool success)
	{
		if (success)
		{
			HandleSubmitScoreSucceeded();
		}
		else
		{
			HandleSubmitScoreFailed("Failed to report score");
		}
	}

	private void OnReportProgressFinished(bool success)
	{
	}

	private void OnLoadScoresFinished(bool success)
	{
		if (success)
		{
			List<LeaderboardScore> list = new List<LeaderboardScore>();
			IScore[] scores = m_currentLeaderboard.scores;
			foreach (IScore score in scores)
			{
				LeaderboardScore leaderboardScore = new LeaderboardScore();
				leaderboardScore.rank = score.rank.ToString();
				leaderboardScore.name = score.userID;
				leaderboardScore.score = score.value;
				list.Add(leaderboardScore);
			}
			HandleLoadScoresSucceeded(list);
		}
		else
		{
			HandleSubmitScoreFailed("Failed to load scores");
		}
	}
}
