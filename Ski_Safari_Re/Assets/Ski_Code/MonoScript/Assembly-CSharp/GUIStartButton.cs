using UnityEngine;

public class GUIStartButton : GUIButton
{
	public GUITransitionAnimator transitionAnimator;

    void Update()
    {
        if (SkiGameManager.Instance.isOnline)
        {
            transitionAnimator.Hide();
        }
    }

    public override void Click(Vector3 position)
	{
		base.Click(position);
		Deactivate();
		SkiGameManager.Instance.StartSkiing();
		transitionAnimator.initialState = GUITransitionAnimator.InitialState.Visible;
	}
}
