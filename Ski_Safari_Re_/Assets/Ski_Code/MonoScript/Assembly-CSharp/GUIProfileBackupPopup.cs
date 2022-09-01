using System;
using System.Collections;
using UnityEngine;

public class GUIProfileBackupPopup : MonoBehaviour
{
	public int minLevelBeforeShowingNewUsers = 5;

	public int daysUntilReminder = 7;

	public Sound showSound;

	public GUIButton okayButton;

	public Sound okayShowSound;

	public GUIButton socialButton;

	public Sound socialSuccessSound;

	public Sound socialFailSound;

	public GUITransitionAnimator transitionAnimator;

	private static string s_dateKey = "next_backup_prompt_date";

	private static string s_countKey = "backup_prompt_count";

	private static DateTime s_remindDate;

	private bool m_readyToHide;

	public int ShowCount
	{
		get
		{
			return GameState.GetInt(s_countKey);
		}
		set
		{
			GameState.SetInt(s_countKey, value);
		}
	}

	public bool ShouldShow()
	{
		return false;
	}

	private void ClickSocialButton()
	{
		SocialManager instance = SocialManager.Instance;
		instance.OnAuthenticationFinished = (Action<bool>)Delegate.Combine(instance.OnAuthenticationFinished, new Action<bool>(OnAuthenticationFinished));
		SocialManager.Instance.Authenticate(true);
	}

	private void OnAuthenticationFinished(bool isAuthenticated)
	{
		if (isAuthenticated)
		{
			SoundManager.Instance.PlaySound(socialSuccessSound);
			Okay();
		}
		else
		{
			Vector3 shakeMagnitude = new Vector3(0f, 0f, 25f);
			float duration = 0.3f;
			Go.killAllTweensWithTarget(socialButton.transform);
			socialButton.transform.localRotation = Quaternion.identity;
			Go.to(socialButton.transform, duration, new GoTweenConfig().shake(shakeMagnitude, GoShakeType.Eulers));
			SoundManager.Instance.PlaySound(socialFailSound);
		}
		SocialManager instance = SocialManager.Instance;
		instance.OnAuthenticationFinished = (Action<bool>)Delegate.Remove(instance.OnAuthenticationFinished, new Action<bool>(OnAuthenticationFinished));
	}

	private void Okay()
	{
		m_readyToHide = true;
		s_remindDate = DateTime.UtcNow;
		s_remindDate = s_remindDate.AddDays(daysUntilReminder);
		GameState.SetString(s_dateKey, s_remindDate.ToString());
		GameState.Save();
		ShowCount++;
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
		SkiGameManager.Instance.guiTitle.transitionAnimator.Hide();
		transitionAnimator.Show();
		SoundManager.Instance.PlaySound(showSound);
		yield return new WaitForSeconds(3f);
		okayButton.gameObject.SetActive(true);
		SoundManager.Instance.PlaySound(okayShowSound);
		float pulseTimer = 0f;
		while (!m_readyToHide)
		{
			pulseTimer += Time.deltaTime;
			float scale = 1f + Mathf.Sin(pulseTimer * 3f) * 0.1f;
			okayButton.transform.localScale = new Vector3(scale, scale, scale);
			yield return new WaitForSeconds(0f);
		}
		okayButton.gameObject.SetActive(false);
		transitionAnimator.Hide();
		yield return new WaitForSeconds(0.5f);
		SkiGameManager.Instance.guiTitle.transitionAnimator.Show();
		SkiGameManager.Instance.guiTitle.SnapToIdle();
		GUITutorials.Instance.UpdateAutoShow();
	}

	private void Awake()
	{
		GUIButton gUIButton = okayButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Okay));
		GUIButton gUIButton2 = socialButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(ClickSocialButton));
		okayButton.gameObject.SetActive(false);
		StartCoroutine(Animate());
	}
}
