public class GUISocialSettingsButton : GUIRolloutElementButton
{
	public override bool Available
	{
		get
		{
			return true;
		}
	}

	protected override bool Toggled
	{
		get
		{
			return SkiGameManager.Instance.ShowSocialSettingsGUI;
		}
		set
		{
			if (value)
			{
				SkiGameManager.Instance.ShowAchievementsGUI = false;
				SkiGameManager.Instance.ShowLeaderboardGUI = false;
				SkiGameManager.Instance.ShowSettingsGUI = false;
				SkiGameManager.Instance.ShowOtherGamesGUI = false;
				SkiGameManager.Instance.ShowFacebookGUI = false;
				GUITutorials.Instance.Hide();
			}
			SkiGameManager.Instance.ShowSocialSettingsGUI = value;
		}
	}

	protected void Update()
	{
		bool showSocialSettingsGUI = SkiGameManager.Instance.ShowSocialSettingsGUI;
		activeSprite.SetActive(showSocialSettingsGUI);
		inactiveSprite.SetActive(!showSocialSettingsGUI);
	}
}
