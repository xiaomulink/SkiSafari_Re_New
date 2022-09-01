using System;
using System.Collections;
using UnityEngine;

public class GUIUpdateNews2 : GUIUpdateNews
{
	public GameObject yetiContent;

	public GameObject yetiSpeechBubble;

	public GameObject yetiSilentIcon;

	public GameObject yetiTalkingIcon;

	public AudioClip yetiSound;

	public GameObject dynamicContent;

	public GameObject slopeBackground;

	public GameObject slopeText;

	public GameObject slopeNameText;

	public GameObject slopeGlow;

	public GameObject slopeIcon;

	public AudioClip slopeSound;

	public GameObject shopBackground;

	public GameObject costumesText;

	public GameObject costumesIcon;

	public AudioClip costumesSound;

	public GameObject upgradesText;

	public GameObject upgradesIcon;

	public AudioClip upgradesSound;

	public GameObject snowmobilesText;

	public GameObject snowmobilesIcon;

	public AudioClip snowmobilesSound;

	public AudioClip starsSound;

	public GameObject[] stars;

	public Transform starEffectNode;

	public GameObject starEffectPrefab;

	public AudioClip punchSound;

	public GameObject challengesBackground;

	public GameObject challengesText;

	public AudioClip challengesSound;

	public GameObject moreText;

	public AudioClip moreSound;

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
		SlideFadeElement(slopeText, -2f);
		slopeBackground.SetActive(true);
		yield return new WaitForSeconds(1f);
		SlideFadeElement(slopeNameText, 2f);
		yield return new WaitForSeconds(0.25f);
		PunchElement(slopeGlow, null);
		PunchElement(slopeIcon, slopeSound);
		yield return new WaitForSeconds(2f);
		shopBackground.SetActive(true);
		SlideFadeElement(costumesText, -2f);
		yield return new WaitForSeconds(0.25f);
		PunchElement(costumesIcon, costumesSound);
		yield return new WaitForSeconds(1.5f);
		SlideFadeElement(upgradesText, -2f);
		yield return new WaitForSeconds(0.25f);
		PunchElement(upgradesIcon, upgradesSound);
		yield return new WaitForSeconds(1.5f);
		SlideFadeElement(snowmobilesText, 2f);
		yield return new WaitForSeconds(0.25f);
		PunchElement(snowmobilesIcon, snowmobilesSound);
		yield return new WaitForSeconds(2f);
		SoundManager.Instance.PlaySound(starsSound);
		GameObject[] array = stars;
		foreach (GameObject star in array)
		{
			star.SetActive(true);
			Go.from(star.transform, 0.5f, new GoTweenConfig().scale(0f));
			yield return new WaitForSeconds(0.15f);
		}
		GameObject starEffect = UnityEngine.Object.Instantiate(starEffectPrefab, starEffectNode.position, starEffectNode.rotation) as GameObject;
		starEffect.transform.parent = starEffectNode;
		SlideFadeElement(challengesText, 4f);
		challengesBackground.SetActive(true);
		SoundManager.Instance.PlaySound(challengesSound);
		yield return new WaitForSeconds(2f);
		SlideFadeElement(moreText, -2f);
		SoundManager.Instance.PlaySound(moreSound);
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
		GameState.Synchronise();
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
