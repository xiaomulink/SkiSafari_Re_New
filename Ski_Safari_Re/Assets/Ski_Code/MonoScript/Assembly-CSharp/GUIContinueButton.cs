using UnityEngine;

public class GUIContinueButton : GUIButton
{
	public GUITransitionAnimator transitionAnimator;

	public bool IsShowing
	{
		get
		{
			return transitionAnimator.IsShowing;
		}
	}

	public void Show(OnClickDelegate onClick)
	{
		OnClick = onClick;
		transitionAnimator.Show();
	}

	public override void Click(Vector3 position)
	{
		if (OnClick != null)
		{
			OnClick();
			OnClick = null;
			transitionAnimator.Hide();
		}
	}
}
