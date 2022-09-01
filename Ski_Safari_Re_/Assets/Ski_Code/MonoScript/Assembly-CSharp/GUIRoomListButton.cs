using UnityEngine;

public class GUIRoomListButton : GUIButton
{
    public static GUIRoomListButton match;
    private void Start()
    {
        match = this;
    }

    public GUITransitionAnimator transitionAnimator;

	public override void Click(Vector3 position)
	{
		base.Click(position);
        PanelManager.Open<RoomListPanel>();
        
	}

    public void SkiStart()
    {
        Deactivate();
        SkiGameManager.Instance.StartSkiing();
        transitionAnimator.initialState = GUITransitionAnimator.InitialState.Visible;
    }
        
}
