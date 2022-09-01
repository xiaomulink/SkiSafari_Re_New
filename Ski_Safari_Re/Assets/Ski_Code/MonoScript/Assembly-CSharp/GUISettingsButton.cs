public class GUISettingsButton : GUIRolloutElementButton
{
	protected override bool Toggled
	{
		get
		{
			return SkiGameManager.Instance.ShowSettingsGUI;
		}
		set
		{
			if (value)
			{
				SkiGameManager.Instance.ShowSocialSettingsGUI = false;
				SkiGameManager.Instance.ShowOtherGamesGUI = false;
				SkiGameManager.Instance.ShowLeaderboardGUI = false;
				SkiGameManager.Instance.ShowFacebookGUI = false;
				SkiGameManager.Instance.ShowAchievementsGUI = false;
				GUITutorials.Instance.Hide();
			}
			SkiGameManager.Instance.ShowSettingsGUI = value;
		}
	}

	private void Update()
	{
		bool showSettingsGUI = SkiGameManager.Instance.ShowSettingsGUI;
		activeSprite.SetActive(showSettingsGUI);
		inactiveSprite.SetActive(!showSettingsGUI);
	}
}
