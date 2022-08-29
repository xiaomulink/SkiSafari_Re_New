using System;

public class GUISocialSignInPopup : GUIPopup
{
	public GUIButton signInButton;

	public override bool ShouldShow()
	{
		return false;
	}

	protected override void OnShow()
	{
	}

	protected override void OnHide()
	{
		signInButton.enabled = false;
	}

	private void SignIn()
	{
		base.ReadyToHide = true;
	}

	protected override void Awake()
	{
		GUIButton gUIButton = signInButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(SignIn));
		base.Awake();
	}
}
