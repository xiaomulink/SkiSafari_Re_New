using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms;

public class GameCircleSocial : ISocialPlatform
{
	[CompilerGenerated]
	private sealed class _003CReportProgress_003Ec__AnonStorey4
	{
		internal Action<bool> callback;

		internal void _003C_003Em__6(string a)
		{
			callback(true);
		}

		internal void _003C_003Em__7(string a, string e)
		{
			callback(false);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadAchievementDescriptions_003Ec__AnonStorey5
	{
		internal Action<IAchievementDescription[]> callback;

		internal void _003C_003Em__8(string e)
		{
			callback(null);
		}

		internal void _003C_003Em__9(List<AGSAchievement> achievements)
		{
			AGSSocialAchievementDescription[] array = new AGSSocialAchievementDescription[achievements.Count];
			for (int i = 0; i < achievements.Count; i++)
			{
				array[i] = new AGSSocialAchievementDescription(achievements[i]);
			}
			callback(array);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadAchievements_003Ec__AnonStorey6
	{
		internal Action<IAchievement[]> callback;

		internal void _003C_003Em__A(string e)
		{
			callback(null);
		}

		internal void _003C_003Em__B(List<AGSAchievement> achievements)
		{
			AGSSocialAchievement[] array = new AGSSocialAchievement[achievements.Count];
			for (int i = 0; i < achievements.Count; i++)
			{
				array[i] = new AGSSocialAchievement(achievements[i]);
			}
			callback(array);
		}
	}

	[CompilerGenerated]
	private sealed class _003CReportScore_003Ec__AnonStorey7
	{
		internal Action<bool> callback;

		internal void _003C_003Em__C(string a)
		{
			callback(true);
		}

		internal void _003C_003Em__D(string a, string e)
		{
			callback(false);
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadScores_003Ec__AnonStorey8
	{
		internal string leaderboardID;

		internal Action<IScore[]> callback;

		internal void _003C_003Em__E(List<AGSLeaderboard> leaderboards)
		{
			IScore[] obj = null;
			foreach (AGSLeaderboard leaderboard in leaderboards)
			{
				if (leaderboard.id == leaderboardID)
				{
					AGSSocialLeaderboard aGSSocialLeaderboard = new AGSSocialLeaderboard(leaderboard);
					obj = aGSSocialLeaderboard.scores;
					break;
				}
			}
			callback(obj);
		}

		internal void _003C_003Em__F(string error)
		{
			callback(null);
		}
	}

	[CompilerGenerated]
	private sealed class _003CAuthenticate_003Ec__AnonStorey9
	{
		internal Action<bool> callback;

		internal void _003C_003Em__10()
		{
			callback(true);
		}

		internal void _003C_003Em__11(string error)
		{
			callback(false);
		}
	}

	private AGSSocialLocalUser gameCircleLocalUser = new AGSSocialLocalUser();

	private static GameCircleSocial socialInstance = new GameCircleSocial();

	public static GameCircleSocial Instance
	{
		get
		{
			return socialInstance;
		}
	}

	public ILocalUser localUser
	{
		get
		{
			return gameCircleLocalUser;
		}
	}

	public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
	{
		AGSClient.LogGameCircleError("ISocialPlatform.LoadUsers is not available for GameCircle");
	}

	public void ReportProgress(string achievementID, double progress, Action<bool> callback)
	{
		_003CReportProgress_003Ec__AnonStorey4 _003CReportProgress_003Ec__AnonStorey = new _003CReportProgress_003Ec__AnonStorey4();
		_003CReportProgress_003Ec__AnonStorey.callback = callback;
		if (_003CReportProgress_003Ec__AnonStorey.callback != null)
		{
			AGSAchievementsClient.UpdateAchievementSucceededEvent += _003CReportProgress_003Ec__AnonStorey._003C_003Em__6;
			AGSAchievementsClient.UpdateAchievementFailedEvent += _003CReportProgress_003Ec__AnonStorey._003C_003Em__7;
		}
		AGSAchievementsClient.UpdateAchievementProgress(achievementID, (float)progress);
	}

	public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
	{
		_003CLoadAchievementDescriptions_003Ec__AnonStorey5 _003CLoadAchievementDescriptions_003Ec__AnonStorey = new _003CLoadAchievementDescriptions_003Ec__AnonStorey5();
		_003CLoadAchievementDescriptions_003Ec__AnonStorey.callback = callback;
		if (_003CLoadAchievementDescriptions_003Ec__AnonStorey.callback == null)
		{
			AGSClient.LogGameCircleError("LoadAchievementDescriptions \"callback\" argument should not be null");
			return;
		}
		AGSAchievementsClient.RequestAchievementsFailedEvent += _003CLoadAchievementDescriptions_003Ec__AnonStorey._003C_003Em__8;
		AGSAchievementsClient.RequestAchievementsSucceededEvent += _003CLoadAchievementDescriptions_003Ec__AnonStorey._003C_003Em__9;
		AGSAchievementsClient.RequestAchievements();
	}

	public void LoadAchievements(Action<IAchievement[]> callback)
	{
		_003CLoadAchievements_003Ec__AnonStorey6 _003CLoadAchievements_003Ec__AnonStorey = new _003CLoadAchievements_003Ec__AnonStorey6();
		_003CLoadAchievements_003Ec__AnonStorey.callback = callback;
		if (_003CLoadAchievements_003Ec__AnonStorey.callback == null)
		{
			AGSClient.LogGameCircleError("LoadAchievements \"callback\" argument should not be null");
			return;
		}
		AGSAchievementsClient.RequestAchievementsFailedEvent += _003CLoadAchievements_003Ec__AnonStorey._003C_003Em__A;
		AGSAchievementsClient.RequestAchievementsSucceededEvent += _003CLoadAchievements_003Ec__AnonStorey._003C_003Em__B;
		AGSAchievementsClient.RequestAchievements();
	}

	public IAchievement CreateAchievement()
	{
		return new AGSSocialAchievement();
	}

	public void ReportScore(long score, string board, Action<bool> callback)
	{
		_003CReportScore_003Ec__AnonStorey7 _003CReportScore_003Ec__AnonStorey = new _003CReportScore_003Ec__AnonStorey7();
		_003CReportScore_003Ec__AnonStorey.callback = callback;
		if (_003CReportScore_003Ec__AnonStorey.callback != null)
		{
			AGSLeaderboardsClient.SubmitScoreSucceededEvent += _003CReportScore_003Ec__AnonStorey._003C_003Em__C;
			AGSLeaderboardsClient.SubmitScoreFailedEvent += _003CReportScore_003Ec__AnonStorey._003C_003Em__D;
		}
		AGSLeaderboardsClient.SubmitScore(board, score);
	}

	public void LoadScores(string leaderboardID, Action<IScore[]> callback)
	{
		_003CLoadScores_003Ec__AnonStorey8 _003CLoadScores_003Ec__AnonStorey = new _003CLoadScores_003Ec__AnonStorey8();
		_003CLoadScores_003Ec__AnonStorey.leaderboardID = leaderboardID;
		_003CLoadScores_003Ec__AnonStorey.callback = callback;
		if (_003CLoadScores_003Ec__AnonStorey.callback != null)
		{
			AGSLeaderboardsClient.RequestLeaderboardsSucceededEvent += _003CLoadScores_003Ec__AnonStorey._003C_003Em__E;
			AGSLeaderboardsClient.RequestLeaderboardsFailedEvent += _003CLoadScores_003Ec__AnonStorey._003C_003Em__F;
		}
		AGSLeaderboardsClient.RequestLeaderboards();
	}

	public ILeaderboard CreateLeaderboard()
	{
		return new AGSSocialLeaderboard();
	}

	public void ShowAchievementsUI()
	{
		AGSAchievementsClient.ShowAchievementsOverlay();
	}

	public void ShowLeaderboardUI()
	{
		AGSLeaderboardsClient.ShowLeaderboardsOverlay();
	}

	public void Authenticate(ILocalUser user, Action<bool> callback)
	{
		_003CAuthenticate_003Ec__AnonStorey9 _003CAuthenticate_003Ec__AnonStorey = new _003CAuthenticate_003Ec__AnonStorey9();
		_003CAuthenticate_003Ec__AnonStorey.callback = callback;
		if (_003CAuthenticate_003Ec__AnonStorey.callback != null)
		{
			AGSClient.ServiceReadyEvent += _003CAuthenticate_003Ec__AnonStorey._003C_003Em__10;
			AGSClient.ServiceNotReadyEvent += _003CAuthenticate_003Ec__AnonStorey._003C_003Em__11;
		}
		AGSClient.Init(true, true, false);
	}

	public void LoadFriends(ILocalUser user, Action<bool> callback)
	{
		if (user == null)
		{
			AGSClient.LogGameCircleError("LoadFriends \"user\" argument should not be null");
		}
		else
		{
			user.LoadFriends(callback);
		}
	}

	public void LoadScores(ILeaderboard board, Action<bool> callback)
	{
		if (board == null)
		{
			AGSClient.LogGameCircleError("LoadScores \"board\" argument should not be null");
		}
		else
		{
			board.LoadScores(callback);
		}
	}

	public bool GetLoading(ILeaderboard board)
	{
		if (board == null)
		{
			AGSClient.LogGameCircleError("GetLoading \"board\" argument should not be null");
			return false;
		}
		return board.loading;
	}

    public void Authenticate(ILocalUser user, Action<bool, string> callback)
    {
        throw new NotImplementedException();
    }
}
