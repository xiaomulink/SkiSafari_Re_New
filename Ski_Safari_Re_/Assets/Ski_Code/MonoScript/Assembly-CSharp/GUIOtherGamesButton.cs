public class GUIOtherGamesButton : GUIRolloutElementButton
{
	protected override bool Toggled
	{
		get
		{
			return SkiGameManager.Instance.ShowOtherGamesGUI;
		}
		set
		{
			if (value)
			{
				SkiGameManager.Instance.ShowSettingsGUI = false;
				SkiGameManager.Instance.ShowSocialSettingsGUI = false;
				SkiGameManager.Instance.ShowLeaderboardGUI = false;
				SkiGameManager.Instance.ShowFacebookGUI = false;
				SkiGameManager.Instance.ShowAchievementsGUI = false;
				GUITutorials.Instance.Hide();
			}
			SkiGameManager.Instance.ShowOtherGamesGUI = value;
		}
	}

	private void Update()
	{
		bool showOtherGamesGUI = SkiGameManager.Instance.ShowOtherGamesGUI;
		activeSprite.SetActive(showOtherGamesGUI);
		inactiveSprite.SetActive(!showOtherGamesGUI);
	}
}
