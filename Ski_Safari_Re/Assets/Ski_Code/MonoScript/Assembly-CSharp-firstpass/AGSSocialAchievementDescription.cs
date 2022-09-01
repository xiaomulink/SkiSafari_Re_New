using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AGSSocialAchievementDescription : IAchievementDescription
{
	public readonly AGSAchievement achievement;

	public string id { get; set; }

	public string title
	{
		get
		{
			if (achievement == null)
			{
				return null;
			}
			return achievement.title;
		}
	}

	public Texture2D image
	{
		get
		{
			AGSClient.LogGameCircleError("IAchievementDescription.image.get is not available for GameCircle");
			return null;
		}
	}

	public string achievedDescription
	{
		get
		{
			if (achievement == null)
			{
				return null;
			}
			return achievement.decription;
		}
	}

	public string unachievedDescription
	{
		get
		{
			if (achievement == null)
			{
				return null;
			}
			return achievement.decription;
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

	public int points
	{
		get
		{
			if (achievement == null)
			{
				return 0;
			}
			return achievement.pointValue;
		}
	}

	public AGSSocialAchievementDescription(AGSAchievement achievement)
	{
		if (achievement == null)
		{
			AGSClient.LogGameCircleError("AGSSocialAchievementDescription constructor \"achievement\" argument should not be null");
			return;
		}
		this.achievement = achievement;
		id = achievement.id;
	}

	public AGSSocialAchievementDescription()
	{
		achievement = null;
	}
}
