using System;
using System.Collections;
using UnityEngine;

public class GUIRatingPopup : MonoBehaviour
{
	[Serializable]
	public class PlatformInfo
	{
		public string url;

		public string appId;

		public string line1;

		public string line2;
	}

	public int numRestartsBeforeShowing = 3;

	public int minLevelBeforeShowing = 3;

	public int hoursUntilReminder = 23;

	public Sound showSound;

	public Sound okaySound;

	public Sound remindMeSound;

	public Sound noThanksSound;

	public PlatformInfo iOSInfo;

	public PlatformInfo googlePlayInfo;

	public PlatformInfo amazonInfo;

	public PlatformInfo nookInfo;

	public GUIButton okayButton;

	public GUIButton noThanksButton;

	public GUIButton remindMeButton;

	public TextMesh textMesh1;

	public TextMesh textMesh2;

	public GUITransitionAnimator transitionAnimator;

	private static string s_versionKey = "last_rated_version";

	private static string s_dateKey = "next_rating_request_date";

	private static DateTime s_remindDate;

	private static bool s_hasRemindDate;

	private string m_url;

	private string m_appId;

	private bool m_readyToHide;

	public bool ShouldShow()
	{
		if (SkiGameManager.Instance.NumRestarts < numRestartsBeforeShowing)
		{
			return false;
		}
		if (LevelManager.Instance.CurrentLevel < minLevelBeforeShowing)
		{
			return false;
		}
		if (!s_hasRemindDate)
		{
			string @string = GameState.GetString(s_dateKey);
			if (!DateTime.TryParse(@string, out s_remindDate))
			{
				s_remindDate = DateTime.UtcNow;
			}
			s_hasRemindDate = true;
		}
		return GameState.GetString(s_versionKey) != AppInfo.Version && DateTime.UtcNow >= s_remindDate;
	}

	private void Okay()
	{
		if (transitionAnimator.IsShowing)
		{
			m_readyToHide = true;
			SoundManager.Instance.PlaySound(okaySound);
			GameState.SetString(s_versionKey, AppInfo.Version);
			GameState.Save();
			Application.OpenURL(m_url);
		}
	}

	private void NoThanks()
	{
		if (transitionAnimator.IsShowing)
		{
			m_readyToHide = true;
			SoundManager.Instance.PlaySound(noThanksSound);
			GameState.SetString(s_versionKey, AppInfo.Version);
			GameState.Save();
		}
	}

	private void RemindMe()
	{
		if (transitionAnimator.IsShowing)
		{
			m_readyToHide = true;
			SoundManager.Instance.PlaySound(remindMeSound);
			s_remindDate = DateTime.UtcNow;
			s_remindDate = s_remindDate.AddHours(hoursUntilReminder);
			GameState.SetString(s_dateKey, s_remindDate.ToString());
			GameState.Save();
		}
	}

	private IEnumerator Animate()
	{
		if (SkiGameManager.Instance.Initialising)
		{
			yield return new WaitForSeconds(3f);
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		transitionAnimator.Show();
		Go.to(base.transform, 1.5f, new GoTweenConfig().rotation(new Vector3(0f, 0f, 20f), true).setEaseType(GoEaseType.ElasticPunch));
		SoundManager.Instance.PlaySound(showSound);
		while (!m_readyToHide)
		{
			yield return new WaitForSeconds(0f);
		}
		transitionAnimator.Hide();
		GUITutorials.Instance.UpdateAutoShow();
	}

	private void SetupPlatform(PlatformInfo platformInfo)
	{
		m_url = platformInfo.url;
		m_appId = platformInfo.appId;
		textMesh1.text = platformInfo.line1;
		textMesh2.text = platformInfo.line2;
	}

	private void Awake()
	{
		GUIButton gUIButton = okayButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Okay));
		GUIButton gUIButton2 = noThanksButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(NoThanks));
		GUIButton gUIButton3 = remindMeButton;
		gUIButton3.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton3.OnClick, new GUIButton.OnClickDelegate(RemindMe));
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			SetupPlatform(iOSInfo);
			break;
		case RuntimePlatform.Android:
			SetupPlatform(googlePlayInfo);
			break;
		}
		StartCoroutine(Animate());
	}
}
