public class GUIFacebookButton : GUIRolloutElementButton
{
	protected override bool Toggled
	{
		get
		{
			return SkiGameManager.Instance.ShowFacebookGUI;
		}
		set
		{
			if (value)
			{
				SkiGameManager.Instance.ShowOtherGamesGUI = false;
				SkiGameManager.Instance.ShowSettingsGUI = false;
				SkiGameManager.Instance.ShowSocialSettingsGUI = false;
				SkiGameManager.Instance.ShowLeaderboardGUI = false;
				SkiGameManager.Instance.ShowAchievementsGUI = false;
				GUITutorials.Instance.Hide();
			}
			SkiGameManager.Instance.ShowFacebookGUI = value;
		}
	}

	private void Update()
	{
		bool showFacebookGUI = SkiGameManager.Instance.ShowFacebookGUI;
		activeSprite.SetActive(showFacebookGUI);
		inactiveSprite.SetActive(!showFacebookGUI);
	}
}
