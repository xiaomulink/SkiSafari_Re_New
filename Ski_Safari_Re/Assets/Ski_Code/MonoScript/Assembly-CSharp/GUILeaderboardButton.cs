public class GUILeaderboardButton : GUIRolloutElementButton
{
	protected override bool Toggled
	{
		get
		{
			return SkiGameManager.Instance.ShowLeaderboardGUI;
		}
		set
		{
			if (value)
			{
				GameState.Synchronise();
				SkiGameManager.Instance.ShowAchievementsGUI = false;
				SkiGameManager.Instance.ShowSettingsGUI = false;
				SkiGameManager.Instance.ShowSocialSettingsGUI = false;
				SkiGameManager.Instance.ShowOtherGamesGUI = false;
				SkiGameManager.Instance.ShowFacebookGUI = false;
				GUITutorials.Instance.Hide();
			}
			SkiGameManager.Instance.ShowLeaderboardGUI = value;
		}
	}

	protected void Update()
	{
		bool showLeaderboardGUI = SkiGameManager.Instance.ShowLeaderboardGUI;
		activeSprite.SetActive(showLeaderboardGUI);
		inactiveSprite.SetActive(!showLeaderboardGUI);
	}
}
