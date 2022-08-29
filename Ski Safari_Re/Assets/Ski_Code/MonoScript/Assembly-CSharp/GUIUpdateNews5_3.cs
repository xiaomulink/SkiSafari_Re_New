using System;
using System.Collections;
using UnityEngine;

public class GUIUpdateNews5_3 : GUIUpdateNews
{
	public AudioClip showSound;

	public GUIButton imageButton;

	public string iOSUrl;

	public string iOSProductId;

	public string googlePlayUrl;

	public GUIButton closeButton;

	public AudioClip punchSound;

	public Vector3 hidePos = new Vector3(0.1f, 0.9f, 5f);

	private bool m_readyToHide;

	private void SlideFadeElement(GameObject obj, float amount, AudioClip sound)
	{
		obj.SetActive(true);
		Go.from(obj.transform, 0.5f, new GoTweenConfig().localPosition(obj.transform.localPosition + new Vector3(amount, 0f, 0f), true));
		Color color = obj.GetComponent<Renderer>().material.GetColor("_TintColor");
		color.a = 0f;
		Go.from(obj.GetComponent<Renderer>().material, 0.5f, new GoTweenConfig().materialColor(color, GoMaterialColorType.TintColor));
		SoundManager.Instance.PlaySound(sound);
	}

	private void PunchElement(GameObject obj, AudioClip sound)
	{
		obj.SetActive(true);
		Go.to(obj.transform, 0.5f, new GoTweenConfig().scale(0.5f, true).setEaseType(GoEaseType.ElasticPunch));
		SoundManager.Instance.PlaySound(sound);
	}

	private IEnumerator Animate()
	{
		yield return new WaitForSeconds(m_initialDelay);
		SoundManager.Instance.PlaySound(showSound);
		Go.to(base.transform, 0.5f, new GoTweenConfig().scale(Vector3.one));
		yield return new WaitForSeconds(2f);
		PunchElement(closeButton.gameObject, punchSound);
		float pulseTimer = 0f;
		while (!m_readyToHide)
		{
			yield return new WaitForSeconds(0f);
		}
		closeButton.gameObject.SetActive(false);
		Go.to(base.transform, 0.5f, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(hidePos)).scale(Vector3.zero));
		yield return new WaitForSeconds(0.5f);
		GameState.Synchronise();
		UnityEngine.Object.Destroy(base.gameObject);
		GUITutorials.Instance.UpdateAutoShow();
	}

	private void OpenLink()
	{
		Application.OpenURL(googlePlayUrl);
		m_readyToHide = true;
	}

	private void Hide()
	{
		m_readyToHide = true;
	}

	private void Start()
	{
		base.transform.localScale = Vector3.zero;
		GUIButton gUIButton = imageButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(OpenLink));
		closeButton.gameObject.SetActive(false);
		GUIButton gUIButton2 = closeButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(Hide));
		m_readyToHide = false;
		StartCoroutine(Animate());
	}
}
