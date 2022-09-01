using System;
using UnityEngine.SocialPlatforms;

public class AGSSocialLeaderboard : ILeaderboard
{
	private readonly AGSLeaderboard leaderboard;

	public bool loading
	{
		get
		{
			AGSClient.LogGameCircleError("ILeaderboard.loading.get is not available for GameCircle");
			return false;
		}
	}

	public string id { get; set; }

	public UserScope userScope { get; set; }

	public System.Range range { get; set; }

	public TimeScope timeScope { get; set; }

	public IScore localUserScore
	{
		get
		{
			if (!ScoresAvailable())
			{
				return null;
			}
			return new AGSSocialLeaderboardScore(leaderboard.scores[0], leaderboard);
		}
	}

	public uint maxRange
	{
		get
		{
			AGSClient.LogGameCircleError("ILeaderboard.maxRange.get is not available for GameCircle");
			return 0u;
		}
	}

	public IScore[] scores
	{
		get
		{
			if (!ScoresAvailable())
			{
				return null;
			}
			AGSSocialLeaderboardScore[] array = new AGSSocialLeaderboardScore[leaderboard.scores.Count];
			for (int i = 0; i < leaderboard.scores.Count; i++)
			{
				array[i] = new AGSSocialLeaderboardScore(leaderboard.scores[i], leaderboard);
			}
			return array;
		}
	}

	public string title
	{
		get
		{
			if (leaderboard == null)
			{
				return null;
			}
			return leaderboard.name;
		}
	}

    UnityEngine.SocialPlatforms.Range ILeaderboard.range { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public AGSSocialLeaderboard(AGSLeaderboard leaderboard)
	{
		if (leaderboard == null)
		{
			AGSClient.LogGameCircleError("AGSSocialLeaderboard constructor \"leaderboard\" argument should not be null");
			return;
		}
		this.leaderboard = leaderboard;
		id = leaderboard.id;
	}

	public AGSSocialLeaderboard()
	{
		leaderboard = null;
	}

	public bool ScoresAvailable()
	{
		return leaderboard != null && leaderboard.scores != null && leaderboard.scores.Count > 0;
	}

	public void SetUserFilter(string[] userIDs)
	{
		AGSClient.LogGameCircleError("ILeaderboard.SetUserFilter is not available for GameCircle");
	}

	public void LoadScores(Action<bool> callback)
	{
		if (callback != null)
		{
			if (leaderboard == null)
			{
				callback(false);
			}
			callback(true);
		}
	}
}
