using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SocialManager_GooglePlay : SocialManager
{
	[CompilerGenerated]
	private sealed class _003CDoSetAchievementProgress_003Ec__AnonStorey22
	{
		internal string achievementId;

		internal bool _003C_003Em__19()
		{
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CLookupGooglePlayLeaderboardId_003Ec__AnonStorey23
	{
		internal string leaderboardId;

		internal bool _003C_003Em__1A(LeaderboardMapping m)
		{
			return m.leaderboardId == leaderboardId;
		}
	}

	public string clientId;

	public List<LeaderboardMapping> leaderboardMappings;

	public GUISocialSignInPopup signInPopup;


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
            return true;
		}
	}

	public override string PlayerName
	{
		get
		{
            return "Player";
		}
	}

	public override string PlayerIdentifier
	{
		get
		{
            return "D";
		}
	}

	

	protected override void DoSignOut()
	{
		HandleSignOutSucceeded();
	}

	
	protected override void DoSetAchievementProgress(string achievementId, int progress)
	{
		_003CDoSetAchievementProgress_003Ec__AnonStorey22 _003CDoSetAchievementProgress_003Ec__AnonStorey = new _003CDoSetAchievementProgress_003Ec__AnonStorey22();
		_003CDoSetAchievementProgress_003Ec__AnonStorey.achievementId = achievementId;
	}

	

	private void OnAuthenticationSucceeded(string googlePlayId)
	{
		HandleSignInSucceeded();
	}

	private void OnAuthenticationFailed(string reason)
	{
		HandleSignInFailed(reason);
	}

	private void OnSubmitScoreSucceeded(string googlePlayId, object obj)
	{
		HandleSubmitScoreSucceeded();
	}

	private void OnSubmitScoreFailed(string googlePlayId, string reason)
	{
		HandleSubmitScoreFailed(reason);
	}

	
	
	private void OnLoadScoresFailed(string googlePlayId, string reason)
	{
		HandleLoadScoresFailed(reason);
	}

	private string LookupGooglePlayLeaderboardId(string leaderboardId)
	{
		_003CLookupGooglePlayLeaderboardId_003Ec__AnonStorey23 _003CLookupGooglePlayLeaderboardId_003Ec__AnonStorey = new _003CLookupGooglePlayLeaderboardId_003Ec__AnonStorey23();
		_003CLookupGooglePlayLeaderboardId_003Ec__AnonStorey.leaderboardId = leaderboardId;
		LeaderboardMapping leaderboardMapping = leaderboardMappings.Find(_003CLookupGooglePlayLeaderboardId_003Ec__AnonStorey._003C_003Em__1A);
		if (leaderboardMapping != null)
		{
			return leaderboardMapping.googlePlayId;
		}
		return _003CLookupGooglePlayLeaderboardId_003Ec__AnonStorey.leaderboardId;
	}

	private void Start()
	{
		
	}

    protected override void DoAuthenticate(bool force)
    {
        throw new System.NotImplementedException();
    }

    protected override void DoSubmitScore(string leaderboardId, long score)
    {
        //throw new System.NotImplementedException();
    }

    protected override void DoUnlockAchievement(string achievementId)
    {
        throw new System.NotImplementedException();
    }

    protected override void DoLoadScores(string leaderboardId, LeaderboardUserScope userScope)
    {
        throw new System.NotImplementedException();
    }

    protected override void DoShowSettings()
    {
        throw new System.NotImplementedException();
    }

    protected override void DoShowLeaderboards()
    {
        throw new System.NotImplementedException();
    }

    protected override void DoShowLeaderboard(string leaderboardId)
    {
        throw new System.NotImplementedException();
    }

    protected override void DoShowAchievements()
    {
        throw new System.NotImplementedException();
    }
}
