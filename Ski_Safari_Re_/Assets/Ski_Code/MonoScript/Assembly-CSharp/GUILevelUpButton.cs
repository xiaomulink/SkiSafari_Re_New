using UnityEngine;

public class GUILevelUpButton : GUIButton
{
	public GUITransitionAnimator transitionAnimator;

	public override void Click(Vector3 position)
	{
		transitionAnimator.Hide();
	}
}
