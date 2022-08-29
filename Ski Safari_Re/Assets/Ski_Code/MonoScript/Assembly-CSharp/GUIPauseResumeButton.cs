using UnityEngine;

public class GUIPauseResumeButton : GUIButton
{
	public GUIPauseButton pauseButton;

	public override void Click(Vector3 position)
	{
		base.Click(position);
		if (SkiGameManager.Instance.Paused)
		{
			pauseButton.Click(Vector3.zero);
		}
	}
}
