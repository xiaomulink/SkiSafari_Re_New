using UnityEngine;

public class GUIAutoShowChallengesButton : GUIToggleButton
{
	protected override bool Toggled
	{
		get
		{
			return PlayerPrefs.GetInt("hide_start_challenges") == 0;
		}
		set
		{
			PlayerPrefs.SetInt("hide_start_challenges", (!value) ? 1 : 0);
		}
	}
}
