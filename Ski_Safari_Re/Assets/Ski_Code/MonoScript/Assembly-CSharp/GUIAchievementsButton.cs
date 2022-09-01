public class GUIAchievementsButton : GUIRolloutElementButton
{
	protected override bool Toggled
	{
		get
		{
			return SkiGameManager.Instance.ShowAchievementsGUI;
		}
		set
		{
			if (value)
			{
				GameState.Synchronise();
				SkiGameManager.Instance.ShowSettingsGUI = false;
				SkiGameManager.Instance.ShowSocialSettingsGUI = false;
				SkiGameManager.Instance.ShowLeaderboardGUI = false;
				SkiGameManager.Instance.ShowOtherGamesGUI = false;
				SkiGameManager.Instance.ShowFacebookGUI = false;
				GUITutorials.Instance.Hide();
			}
			SkiGameManager.Instance.ShowAchievementsGUI = value;
		}
	}

	protected void Update()
	{
		bool showAchievementsGUI = SkiGameManager.Instance.ShowAchievementsGUI;
		activeSprite.SetActive(showAchievementsGUI);
		inactiveSprite.SetActive(!showAchievementsGUI);
	}
}
