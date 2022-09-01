public class GUITutorialsButton : GUIRolloutElementButton
{
	protected override bool Toggled
	{
		get
		{
            try
            {
                return GUITutorials.Instance.transitionAnimator.IsShowing;
            }
            catch
            {

                return false;
            }
		}
		set
		{
			if (value)
			{
				SkiGameManager.Instance.ShowAchievementsGUI = false;
				SkiGameManager.Instance.ShowLeaderboardGUI = false;
				SkiGameManager.Instance.ShowSettingsGUI = false;
				SkiGameManager.Instance.ShowSocialSettingsGUI = false;
				SkiGameManager.Instance.ShowOtherGamesGUI = false;
				SkiGameManager.Instance.ShowFacebookGUI = false;
				GUITutorials.Instance.Show();
			}
			else
			{
				GUITutorials.Instance.Hide();
			}
		}
	}

	protected void Update()
	{
		bool toggled = Toggled;
		activeSprite.SetActive(toggled);
		inactiveSprite.SetActive(!toggled);
	}
}
