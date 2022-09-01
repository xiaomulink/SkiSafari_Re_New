using UnityEngine;

public class GUIHighScoreButton : GUIButton
{
	public GUIHighScore highScore;

	public override void Click(Vector3 position)
	{
		base.Click(position);
		highScore.Submit();
	}
}
