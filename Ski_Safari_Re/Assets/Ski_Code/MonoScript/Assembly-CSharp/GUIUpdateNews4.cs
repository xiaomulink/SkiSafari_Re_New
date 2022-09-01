using System;
using System.Collections;
using UnityEngine;

public class GUIUpdateNews4 : GUIUpdateNews
{
	public GameObject yetiContent;

	public GameObject yetiSpeechBubble;

	public GameObject yetiSilentIcon;

	public GameObject yetiTalkingIcon;

	public AudioClip yetiSound;

	public GameObject dynamicContent;

	public GameObject elfIcon;

	public AudioClip elfSound;

	public GUIContinueButton continueButton;

	public AudioClip punchSound;

	public GUIButton closeButton;

	public Booster pocketRockets;

	public int pocketRocketCount = 5;

	public Booster superBlueBoosts;

	public int superBlueBoostCount = 5;

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
		yield return new WaitForSeconds(0.5f);
		PunchElement(elfIcon, elfSound);
		m_continue = false;
		continueButton.Show(Continue);
		while (!m_continue)
		{
			yield return new WaitForSeconds(0f);
		}
		continueButton.gameObject.SetActive(false);
		pocketRockets.CurrentCount += pocketRocketCount;
		superBlueBoosts.CurrentCount += superBlueBoostCount;
		GameState.Synchronise();
		Go.to(base.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero));
		yield return new WaitForSeconds(0.5f);
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
