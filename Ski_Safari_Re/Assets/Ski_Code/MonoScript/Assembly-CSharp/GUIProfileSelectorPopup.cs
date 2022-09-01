using System;
using System.Runtime.CompilerServices;

public class GUIProfileSelectorPopup : GUIPopup
{
	public GUIProfileSummary currentProfileSummary;

	public GUIProfileSummary cloudProfileSummary;

	public Sound selectSound;

	public GUIButton okayButton;

	public GUIButton signOutButton;

	private GUIProfileSummary m_selectedProfileSummary;

	private static bool s_hasShown;

	public override bool ShouldShow()
	{
		return !s_hasShown;
	}

	protected override void OnShow()
	{
		s_hasShown = true;
	}

	protected override void OnHide()
	{
		currentProfileSummary.enabled = false;
		cloudProfileSummary.enabled = false;
		okayButton.enabled = false;
		signOutButton.enabled = false;
	}

	private void SelectProfile(GUIProfileSummary profileSummary)
	{
		if (m_selectedProfileSummary != profileSummary)
		{
			SoundManager.Instance.PlaySound(selectSound);
			m_selectedProfileSummary = profileSummary;
			currentProfileSummary.SetSelected(currentProfileSummary == profileSummary);
			cloudProfileSummary.SetSelected(cloudProfileSummary == profileSummary);
		}
	}

	private void Okay()
	{
		base.ReadyToHide = true;
	}

	private void SignOut()
	{
		SocialManager.Instance.SignOut();
		SelectProfile(currentProfileSummary);
		base.ReadyToHide = true;
	}

	protected override void Awake()
	{
		GUIProfileSummary gUIProfileSummary = currentProfileSummary;
		gUIProfileSummary.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIProfileSummary.OnClick, new GUIButton.OnClickDelegate(_003CAwake_003Em__13));
		GUIProfileSummary gUIProfileSummary2 = cloudProfileSummary;
		gUIProfileSummary2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIProfileSummary2.OnClick, new GUIButton.OnClickDelegate(_003CAwake_003Em__14));
		GUIButton gUIButton = okayButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Okay));
		GUIButton gUIButton2 = signOutButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(SignOut));
		currentProfileSummary.Profile = GameState.PersistentProfile;
		currentProfileSummary.SetSelected(true);
		cloudProfileSummary.Profile = GameState.PersistentProfile;
		cloudProfileSummary.SetSelected(false);
		base.Awake();
	}

	[CompilerGenerated]
	private void _003CAwake_003Em__13()
	{
		SelectProfile(currentProfileSummary);
	}

	[CompilerGenerated]
	private void _003CAwake_003Em__14()
	{
		SelectProfile(cloudProfileSummary);
	}
}
