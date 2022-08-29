using System;
using System.Collections;
using UnityEngine;

public class GUIUpdateNews3_1 : GUIUpdateNews
{
	public GameObject yetiContent;

	public GameObject yetiSpeechBubble;

	public GameObject yetiSilentIcon;

	public GameObject yetiTalkingIcon;

	public AudioClip yetiSound;

	public GameObject dynamicContent;

	public GameObject introducingTextBackground;

	public GameObject introducingText;

	public GameObject bugTextBackground;

	public GameObject bugText;

	public GameObject bugIcon;

	public AudioClip bugSound;

	public GameObject reportInfoGroup;

	public GameObject reportInfoBackground;

	public GameObject[] reportInfoText;

	public GameObject reportProcedureGroup;

	public GameObject reportProcedureBackground;

	public GameObject[] reportProcedureObjects;

	public GameObject giveCoinsGroup;

	public AudioClip giveCoinsSound;

	public GUIContinueButton continueButton;

	public AudioClip punchSound;

	public GUIButton closeButton;

	private bool m_continue;

	private void SlideFadeElement(GameObject obj, float amount)
	{
		obj.SetActive(true);
		Go.from(obj.transform, 0.5f, new GoTweenConfig().localPosition(new Vector3(amount, 0f, 0f), true));
		Color color = obj.GetComponent<Renderer>().material.color;
		color.a = 0f;
		Go.from(obj.GetComponent<Renderer>().material, 0.5f, new GoTweenConfig().materialColor(color));
	}

	private void PunchElement(GameObject obj, AudioClip sound)
	{
		obj.SetActive(true);
		Go.to(obj.transform, 0.5f, new GoTweenConfig().scale(0.25f, true).setEaseType(GoEaseType.ElasticPunch));
		SoundManager.Instance.PlaySound(sound);
	}

	private void Continue()
	{
		m_continue = true;
	}

	private IEnumerator Animate()
	{
		yield return new WaitForSeconds(m_initialDelay);
		Go.to(base.transform, 0.5f, new GoTweenConfig().scale(Vector3.one));
		yield return new WaitForSeconds(1f);
		PunchElement(yetiSilentIcon, null);
		yield return new WaitForSeconds(0.5f);
		yetiTalkingIcon.SetActive(true);
		PunchElement(yetiSpeechBubble, yetiSound);
		yield return new WaitForSeconds(2f);
		yetiContent.SetActive(false);
		SlideFadeElement(introducingText, -2f);
		introducingTextBackground.SetActive(true);
		yield return new WaitForSeconds(1f);
		PunchElement(bugIcon, bugSound);
		SlideFadeElement(bugText, -2f);
		bugTextBackground.SetActive(true);
		yield return new WaitForSeconds(2f);
		reportInfoBackground.SetActive(true);
		GameObject[] array = reportInfoText;
		foreach (GameObject item in array)
		{
			SlideFadeElement(item, 2f);
			yield return new WaitForSeconds(1f);
		}
		yield return new WaitForSeconds(1f);
		m_continue = false;
		continueButton.transitionAnimator.SnapHide();
		continueButton.Show(Continue);
		while (!m_continue)
		{
			yield return new WaitForSeconds(0f);
		}
		reportInfoGroup.SetActive(false);
		reportProcedureBackground.SetActive(true);
		GameObject[] array2 = reportProcedureObjects;
		foreach (GameObject item2 in array2)
		{
			SlideFadeElement(item2, 2f);
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(1f);
		m_continue = false;
		continueButton.Show(Continue);
		while (!m_continue)
		{
			yield return new WaitForSeconds(0f);
		}
		reportProcedureGroup.SetActive(false);
		PunchElement(giveCoinsGroup, giveCoinsSound);
		yield return new WaitForSeconds(2f);
		m_continue = false;
		continueButton.Show(Continue);
		while (!m_continue)
		{
			yield return new WaitForSeconds(0f);
		}
		continueButton.gameObject.SetActive(false);
		Go.to(base.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero));
		yield return new WaitForSeconds(0.5f);
		GameState.IncrementCoinCount(5000);
		GameState.Synchronise();
		UnityEngine.Object.Destroy(base.gameObject);
		GUITutorials.Instance.UpdateAutoShow();
		if (!GUITutorials.Instance.AutoShow)
		{
			SkiGameManager.Instance.rolloutButton.Expand();
		}
	}

	private void Hide()
	{
	}

	private void Start()
	{
		base.transform.localScale = Vector3.zero;
		dynamicContent.SetActive(false);
		dynamicContent.SetActive(true);
		yetiContent.SetActive(false);
		yetiContent.SetActive(true);
		GUIButton gUIButton = closeButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Hide));
		m_continue = false;
		StartCoroutine(Animate());
	}
}
