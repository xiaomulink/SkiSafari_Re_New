using System;
using UnityEngine;

public class GUISocialSettings : MonoBehaviour
{
	public static GUISocialSettings Instance;

	public GUITransitionAnimator transitionAnimator;

	public Transform pivot;

	public Vector3 signedOutPivotOffset = new Vector3(0f, 2.25f, 0f);

	public GameObject signedInContainer;

	public GameObject signedOutContainer;

	public GUIButton signInButton;

	public GUIButton signOutButton;

	public GUIButton leaderboardsButton;

	public GUIButton achievementsButton;

	public GameObject playerNamePanel;

	public GUIDropShadowText playerNameText;

	public Sound signedInSound;

	public Sound signedOutSound;

	private static string s_shownKey = "social_settings_shown";

	private bool m_wasSignedIn;

	public bool ShouldAutoShow()
	{
		return GameState.GetInt(s_shownKey) == 0 && !SocialManager.Instance.IsAuthenticated && LevelManager.Instance.CurrentLevel >= 4;
	}

	private void SignIn()
	{
		SocialManager.Instance.Authenticate(true);
	}

	private void SignOut()
	{
		SocialManager.Instance.SignOut();
	}

	private bool TrySetPlayerName()
	{
		string playerName = SocialManager.Instance.PlayerName;
		playerNameText.Text = string.Empty;
		Font font = playerNameText.TextMesh.font;
		string text = playerName;
		foreach (char c in text)
		{
			if (!font.HasCharacter(c))
			{
				return false;
			}
		}
		playerNameText.Text = playerName;
		return true;
	}

	private void OnAuthenticationFinished(bool signedIn)
	{
		CancelInvoke("InitialisePlayerNamePanel");
		signedInContainer.SetActive(signedIn);
		signedOutContainer.SetActive(!signedIn);
		if (signedIn)
		{
			if (TrySetPlayerName())
			{
				playerNamePanel.SetActive(true);
				Go.to(pivot, 0.5f, new GoTweenConfig().localPosition(Vector3.zero));
			}
			else
			{
				playerNamePanel.SetActive(false);
			}
			if (!m_wasSignedIn)
			{
				GoTweenConfig config = new GoTweenConfig().scale(1.05f, true).setEaseType(GoEaseType.ElasticPunch);
				Go.to(signedInContainer.transform, 0.5f, config);
				SoundManager.Instance.PlaySound(signedInSound);
			}
		}
		else
		{
			playerNamePanel.SetActive(false);
			Go.to(pivot, 0.5f, new GoTweenConfig().localPosition(signedOutPivotOffset));
			GoTweenConfig config2 = new GoTweenConfig().shake(new Vector3(0f, 0f, 10f), GoShakeType.Eulers);
			Go.to(signedOutContainer.transform, 0.5f, config2);
			SoundManager.Instance.PlaySound(signedOutSound);
		}
		m_wasSignedIn = signedIn;
	}

	private void InitialisePlayerNamePanel()
	{
		if (TrySetPlayerName())
		{
			playerNamePanel.SetActive(true);
			Go.to(pivot, 0.5f, new GoTweenConfig().localPosition(Vector3.zero));
		}
	}

	private void ShowContinueButton()
	{
		SkiGameManager.Instance.continueButton.Show(Continue);
	}

	private void Continue()
	{
		GameState.SetInt(s_shownKey, 1);
		GameState.Save();
		transitionAnimator.Hide();
		GUITutorials.Instance.UpdateAutoShow();
	}

	protected void OnEnable()
	{
		bool isAuthenticated = SocialManager.Instance.IsAuthenticated;
		signedInContainer.SetActive(isAuthenticated);
		signedOutContainer.SetActive(!isAuthenticated);
		playerNamePanel.SetActive(true);
		playerNamePanel.SetActive(false);
		pivot.localPosition = signedOutPivotOffset;
		m_wasSignedIn = isAuthenticated;
		if (isAuthenticated)
		{
			Invoke("InitialisePlayerNamePanel", 0f);
		}
		if ((bool)GUITutorials.Instance && GUITutorials.Instance.AutoShow)
		{
			Invoke("ShowContinueButton", 2f);
		}
		SocialManager instance = SocialManager.Instance;
		instance.OnAuthenticationFinished = (Action<bool>)Delegate.Combine(instance.OnAuthenticationFinished, new Action<bool>(OnAuthenticationFinished));
	}

	protected void OnDisable()
	{
		CancelInvoke("InitialisePlayerNamePanel");
		SocialManager instance = SocialManager.Instance;
		instance.OnAuthenticationFinished = (Action<bool>)Delegate.Remove(instance.OnAuthenticationFinished, new Action<bool>(OnAuthenticationFinished));
	}

	protected void Awake()
	{
		Instance = this;
		GUIButton gUIButton = signInButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(SignIn));
		GUIButton gUIButton2 = signOutButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(SignOut));
		GUIButton gUIButton3 = leaderboardsButton;
		gUIButton3.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton3.OnClick, new GUIButton.OnClickDelegate(SocialManager.Instance.ShowLeaderboards));
		GUIButton gUIButton4 = achievementsButton;
		gUIButton4.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton4.OnClick, new GUIButton.OnClickDelegate(SocialManager.Instance.ShowAchievements));
	}
}
