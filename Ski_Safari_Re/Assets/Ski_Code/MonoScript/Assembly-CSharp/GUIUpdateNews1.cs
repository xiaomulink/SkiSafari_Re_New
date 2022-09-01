using System;
using System.Collections;
using UnityEngine;

public class GUIUpdateNews1 : GUIUpdateNews
{
	public GameObject yetiContent;

	public GameObject yetiSpeechBubble;

	public GameObject yetiSilentIcon;

	public GameObject yetiTalkingIcon;

	public AudioClip yetiSound;

	public GameObject dynamicContent;

	public AudioClip starsSound;

	public GameObject[] stars;

	public AudioClip punchSound;

	public GameObject challengesBackground;

	public GameObject challengesText;

	public AudioClip challengesSound;

	public GameObject shopBackground;

	public GameObject shopText;

	public GameObject shopIcon;

	public GameObject costumesText;

	public GameObject costumesIcon;

	public AudioClip costumesSound;

	public GameObject gizmosText;

	public GameObject gizmosIcon;

	public AudioClip gizmosSound;

	public GameObject powerupsText;

	public GameObject powerupsIcon;

	public AudioClip powerupsSound;

	public GameObject slopeBackground;

	public GameObject slopeText;

	public AudioClip slopeSound;

	public GameObject moreText;

	public GUIButton closeButton;

	private bool m_readyToHide;

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
		SoundManager.Instance.PlaySound(starsSound);
		GameObject[] array = stars;
		foreach (GameObject star in array)
		{
			star.SetActive(true);
			Go.from(star.transform, 0.5f, new GoTweenConfig().scale(0f));
			yield return new WaitForSeconds(0.15f);
		}
		SlideFadeElement(challengesText, 4f);
		challengesBackground.SetActive(true);
		SoundManager.Instance.PlaySound(challengesSound);
		yield return new WaitForSeconds(1f);
		PunchElement(shopIcon, punchSound);
		yield return new WaitForSeconds(0.25f);
		SlideFadeElement(shopText, 2f);
		shopBackground.SetActive(true);
		yield return new WaitForSeconds(1f);
		SlideFadeElement(costumesText, -2f);
		yield return new WaitForSeconds(0.25f);
		PunchElement(costumesIcon, costumesSound);
		yield return new WaitForSeconds(1f);
		SlideFadeElement(gizmosText, 2f);
		yield return new WaitForSeconds(0.25f);
		PunchElement(gizmosIcon, gizmosSound);
		yield return new WaitForSeconds(1f);
		SlideFadeElement(powerupsText, -2f);
		yield return new WaitForSeconds(0.25f);
		PunchElement(powerupsIcon, powerupsSound);
		yield return new WaitForSeconds(2f);
		SlideFadeElement(slopeText, -2f);
		slopeBackground.SetActive(true);
		yield return new WaitForSeconds(0.25f);
		SoundManager.Instance.PlaySound(slopeSound);
		yield return new WaitForSeconds(0.25f);
		SlideFadeElement(moreText, 2f);
		yield return new WaitForSeconds(2f);
		PunchElement(closeButton.gameObject, punchSound);
		float pulseTimer = 0f;
		while (!m_readyToHide)
		{
			pulseTimer += Time.deltaTime;
			float scale = 1f + Mathf.Sin(pulseTimer * 3f) * 0.1f;
			closeButton.transform.localScale = new Vector3(scale, scale, scale);
			yield return new WaitForSeconds(0f);
		}
		closeButton.gameObject.SetActive(false);
		Go.to(base.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero));
		yield return new WaitForSeconds(0.5f);
		UnityEngine.Object.Destroy(base.gameObject);
		GUITutorials.Instance.UpdateAutoShow();
	}

	private void Hide()
	{
		m_readyToHide = true;
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
		m_readyToHide = false;
		StartCoroutine(Animate());
	}
}
