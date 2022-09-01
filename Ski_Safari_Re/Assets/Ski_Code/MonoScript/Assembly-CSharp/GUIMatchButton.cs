using UnityEngine;

public class GUIMatchButton : GUIButton
{
    public static GUIMatchButton match;
    private void Start()
    {
        match = this;
    }

    public GUITransitionAnimator transitionAnimator;

	public override void Click(Vector3 position)
	{
		base.Click(position);
        PanelManager.Open<MatchPanel>();
        /*
		Deactivate();
		SkiGameManager.Instance.StartSkiing();
		transitionAnimator.initialState = GUITransitionAnimator.InitialState.Visible;
        */
	}

    public void SkiStart()
    {
        Deactivate();
        SkiGameManager.Instance.StartSkiing();
        transitionAnimator.initialState = GUITransitionAnimator.InitialState.Visible;
    }
        
}
