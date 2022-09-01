using System;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms;

public class AGSSocialAchievement : IAchievement
{
	[CompilerGenerated]
	private sealed class _003CReportProgress_003Ec__AnonStorey2
	{
		internal Action<bool> callback;

		internal void _003C_003Em__0(string a)
		{
			callback(true);
		}

		internal void _003C_003Em__1(string a, string e)
		{
			callback(false);
		}
	}

	public readonly AGSAchievement achievement;

	public string id { get; set; }

	public double percentCompleted { get; set; }

	public bool completed
	{
		get
		{
			if (achievement == null)
			{
				return false;
			}
			return achievement.isUnlocked;
		}
	}

	public bool hidden
	{
		get
		{
			if (achievement == null)
			{
				return false;
			}
			return achievement.isHidden;
		}
	}

	public DateTime lastReportedDate
	{
		get
		{
			if (achievement == null)
			{
				AGSClient.LogGameCircleError("IAchievement.lastReportedDate.get is not available, returning System.DateTime.MinValue.");
				return DateTime.MinValue;
			}
			return achievement.dateUnlocked;
		}
	}

	public AGSSocialAchievement(AGSAchievement achievement)
	{
		if (achievement == null)
		{
			AGSClient.LogGameCircleError("AGSSocialAchievement constructor \"achievement\" argument should not be null");
			return;
		}
		this.achievement = achievement;
		id = achievement.id;
		percentCompleted = achievement.progress;
	}

	public AGSSocialAchievement()
	{
		achievement = null;
	}

	public void ReportProgress(Action<bool> callback)
	{
		_003CReportProgress_003Ec__AnonStorey2 _003CReportProgress_003Ec__AnonStorey = new _003CReportProgress_003Ec__AnonStorey2();
		_003CReportProgress_003Ec__AnonStorey.callback = callback;
		if (_003CReportProgress_003Ec__AnonStorey.callback != null)
		{
			AGSAchievementsClient.UpdateAchievementSucceededEvent += _003CReportProgress_003Ec__AnonStorey._003C_003Em__0;
			AGSAchievementsClient.UpdateAchievementFailedEvent += _003CReportProgress_003Ec__AnonStorey._003C_003Em__1;
		}
		AGSAchievementsClient.UpdateAchievementProgress(id, (float)percentCompleted);
	}
}
