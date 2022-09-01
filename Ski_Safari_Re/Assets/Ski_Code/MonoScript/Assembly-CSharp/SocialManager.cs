using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SocialManager : MonoBehaviour
{
	protected class SubmitScoreRequest
	{
		public string leaderboardId;

		public int score;

		public Action<bool> callback;
	}

	public static SocialManager Instance;

	public SocialAchievementWithId[] achievements;

	public Action<bool> OnAuthenticationFinished;

	private static string m_bestDistanceLeaderboardId = "best_distance";

	private List<SubmitScoreRequest> m_queuedSubmitScoreRequests = new List<SubmitScoreRequest>();

	private SubmitScoreRequest m_activeSubmitScoreRequest;

	private Action<bool, List<LeaderboardScore>> m_loadScoresCallback;

	private List<SocialAchievement> m_achievementInstances = new List<SocialAchievement>();

	public abstract bool CanAuthenticate { get; }

	public abstract bool IsAuthenticated { get; }

	public abstract string PlayerName { get; }

	public abstract string PlayerIdentifier { get; }

	public void Authenticate(bool force)
	{
		if (IsAuthenticated)
		{
			HandleSignInSucceeded();
		}
		else
		{
			DoAuthenticate(force);
		}
	}

	public void SignOut()
	{
		DoSignOut();
	}

	public void SubmitScore(string leaderboardId, int score, Action<bool> callback)
	{
		m_queuedSubmitScoreRequests.Add(new SubmitScoreRequest
		{
			leaderboardId = leaderboardId,
			score = score,
			callback = callback
		});
		if (m_activeSubmitScoreRequest == null)
		{
			ProcessNextSubmitScoreRequest();
		}
	}

	public void SubmitDistance(int distance)
	{
		SubmitScore(m_bestDistanceLeaderboardId, distance, null);
	}

	public void SetAchievementProgress(SocialAchievement achievement, int progress)
	{
		DoSetAchievementProgress(achievement.Id, progress);
	}

	public void UnlockAchievement(SocialAchievement achievement)
	{
		//DoUnlockAchievement(achievement.Id);
		achievement.Unload();
	}

	public void LoadScores(string leaderboardId, LeaderboardUserScope userScope, Action<bool, List<LeaderboardScore>> callback)
	{
		if (m_loadScoresCallback != null && callback != m_loadScoresCallback)
		{
			m_loadScoresCallback(false, null);
		}
		m_loadScoresCallback = callback;
		DoLoadScores(leaderboardId, userScope);
	}

	public void ShowSettings()
	{
		DoShowSettings();
	}

	public void ShowLeaderboards()
	{
		DoShowLeaderboards();
	}

	public void ShowLeaderboard(string leaderboardId)
	{
		DoShowLeaderboard(leaderboardId);
	}

	public void ShowAchievements()
	{
		DoShowAchievements();
	}

	public void LoadAchievements()
	{
		foreach (SocialAchievement achievementInstance in m_achievementInstances)
		{
			achievementInstance.Load();
		}
	}

	protected SocialAchievement FindAchievement(string achievementId)
	{
		foreach (SocialAchievement achievementInstance in m_achievementInstances)
		{
			if (achievementInstance.Id == achievementId)
			{
				return achievementInstance;
			}
		}
		return null;
	}

	protected abstract void DoAuthenticate(bool force);

	protected abstract void DoSignOut();

	protected abstract void DoSubmitScore(string leaderboardId, long score);

	protected abstract void DoSetAchievementProgress(string achievementId, int progress);

	protected abstract void DoUnlockAchievement(string achievementId);

	protected abstract void DoLoadScores(string leaderboardId, LeaderboardUserScope userScope);

	protected abstract void DoShowSettings();

	protected abstract void DoShowLeaderboards();

	protected abstract void DoShowLeaderboard(string leaderboardId);

	protected abstract void DoShowAchievements();

	protected void HandleSignInSucceeded()
	{
		if (GameState.IsInstantiated)
		{
			GameState.SetString("social_user_name", PlayerName);
		}
		if (OnAuthenticationFinished != null)
		{
			OnAuthenticationFinished(true);
		}
	}

	protected void HandleSignInFailed(string reason)
	{
		if (OnAuthenticationFinished != null)
		{
			OnAuthenticationFinished(false);
		}
	}

	protected void HandleSignOutSucceeded()
	{
		if (OnAuthenticationFinished != null)
		{
			OnAuthenticationFinished(false);
		}
	}

	protected void HandleSubmitScoreSucceeded()
	{
		if (m_activeSubmitScoreRequest != null && m_activeSubmitScoreRequest.callback != null)
		{
			m_activeSubmitScoreRequest.callback(true);
		}
		ProcessNextSubmitScoreRequest();
	}

	protected void HandleSubmitScoreFailed(string reason)
	{
		if (m_activeSubmitScoreRequest.callback != null)
		{
			m_activeSubmitScoreRequest.callback(false);
		}
		ProcessNextSubmitScoreRequest();
	}

	protected void HandleLoadScoresSucceeded(List<LeaderboardScore> scores)
	{
		if (m_loadScoresCallback != null)
		{
			m_loadScoresCallback(true, scores);
		}
	}

	protected void HandleLoadScoresFailed(string reason)
	{
		if (m_loadScoresCallback != null)
		{
			m_loadScoresCallback(false, null);
		}
	}

	private void ProcessNextSubmitScoreRequest()
	{
		m_activeSubmitScoreRequest = null;
		if (m_queuedSubmitScoreRequests.Count > 0)
		{
			m_activeSubmitScoreRequest = m_queuedSubmitScoreRequests[0];
			m_queuedSubmitScoreRequests.RemoveAt(0);
			DoSubmitScore(m_activeSubmitScoreRequest.leaderboardId, m_activeSubmitScoreRequest.score);
		}
	}

	protected virtual void Awake()
	{
		Instance = this;
		SocialAchievementWithId[] array = achievements;
		foreach (SocialAchievementWithId socialAchievementWithId in array)
		{
			SocialAchievement socialAchievement = TransformUtils.Instantiate(socialAchievementWithId.achievement, base.transform);
			socialAchievement.name = socialAchievementWithId.achievement.name;
			socialAchievement.Id = socialAchievementWithId.id;
			m_achievementInstances.Add(socialAchievement);
		}
	}
}
