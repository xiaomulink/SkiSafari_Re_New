using System;
using UnityEngine;

public class GUILicenseCheckPopup : MonoBehaviour
{
	public GUIButton okayButton;

	public GUIButton helpButton;

	public string helpLink = "http://defiantdev.com/ski-safari.php";

	public GUITransitionAnimator transitionAnimator;

	private static string LicensePopupShownKey = "license_popup_shown";

	public bool ShouldShow()
	{
		if (LicenseManager.Instance.IsRetrying && SkiGameManager.Instance.NumRestarts > 0 && PlayerPrefs.GetInt(LicensePopupShownKey) < (LicenseManager.Instance.AllowAccess() ? 1 : 2))
		{
			return true;
		}
		return false;
	}

	private void Okay()
	{
		Hide();
	}

	private void Help()
	{
		Hide();
		Application.OpenURL(helpLink);
	}

	private void Show()
	{
		transitionAnimator.Show();
		PlayerPrefs.SetInt(LicensePopupShownKey, LicenseManager.Instance.AllowAccess() ? 1 : 2);
	}

	private void Hide()
	{
		CancelInvoke("Show");
		if (transitionAnimator.IsShowing)
		{
			transitionAnimator.Hide();
			GUITutorials.Instance.UpdateAutoShow();
		}
	}

	private void Awake()
	{
		GUIButton gUIButton = okayButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Okay));
		if ((bool)helpButton)
		{
			GUIButton gUIButton2 = helpButton;
			gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(Help));
		}
		if (SkiGameManager.Instance.Initialising)
		{
			Invoke("Show", 3f);
		}
		else
		{
			Invoke("Show", 1f);
		}
	}
}
