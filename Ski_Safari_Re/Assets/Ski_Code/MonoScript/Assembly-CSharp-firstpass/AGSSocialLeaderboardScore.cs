using System;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms;

public class AGSSocialLeaderboardScore : IScore
{
	[CompilerGenerated]
	private sealed class _003CReportScore_003Ec__AnonStorey3
	{
		internal Action<bool> callback;

		internal void _003C_003Em__2(string a)
		{
			callback(true);
		}

		internal void _003C_003Em__3(string a, string e)
		{
			callback(false);
		}
	}

	private readonly AGSScore score;

	public string leaderboardID { get; set; }

	public long value { get; set; }

	public DateTime date
	{
		get
		{
			AGSClient.LogGameCircleError("IScore.date.get is not available for GameCircle");
			return DateTime.MinValue;
		}
	}

	public string formattedValue
	{
		get
		{
			if (score == null)
			{
				return null;
			}
			return score.scoreString;
		}
	}

	public string userID
	{
		get
		{
			if (score == null)
			{
				return null;
			}
			return score.playerAlias;
		}
	}

	public int rank
	{
		get
		{
			if (score == null)
			{
				return 0;
			}
			return score.rank;
		}
	}

	public AGSSocialLeaderboardScore(AGSScore score, AGSLeaderboard leaderboard)
	{
		if (score == null)
		{
			AGSClient.LogGameCircleError("AGSSocialLeaderboardScore constructor \"score\" argument should not be null");
			return;
		}
		if (leaderboard == null)
		{
			AGSClient.LogGameCircleError("AGSSocialLeaderboardScore constructor \"leaderboard\" argument should not be null");
			return;
		}
		this.score = score;
		leaderboardID = leaderboard.id;
		value = score.scoreValue;
	}

	public AGSSocialLeaderboardScore()
	{
		score = null;
		leaderboardID = null;
	}

	public void ReportScore(Action<bool> callback)
	{
		_003CReportScore_003Ec__AnonStorey3 _003CReportScore_003Ec__AnonStorey = new _003CReportScore_003Ec__AnonStorey3();
		_003CReportScore_003Ec__AnonStorey.callback = callback;
		if (_003CReportScore_003Ec__AnonStorey.callback != null)
		{
			AGSLeaderboardsClient.SubmitScoreSucceededEvent += _003CReportScore_003Ec__AnonStorey._003C_003Em__2;
			AGSLeaderboardsClient.SubmitScoreFailedEvent += _003CReportScore_003Ec__AnonStorey._003C_003Em__3;
		}
		AGSLeaderboardsClient.SubmitScore(leaderboardID, value);
	}
}
