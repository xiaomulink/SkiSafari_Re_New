using System;
using System.Collections;
using UnityEngine;

public class GUIUpdateNews3 : GUIUpdateNews
{
	public AudioClip showSound;

	public AudioClip punchSound;

	public GUIButton closeButton;

	public Vector3 hidePos = new Vector3(0.1f, 0.9f, 5f);

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
		SoundManager.Instance.PlaySound(showSound);
		Go.to(base.transform, 0.5f, new GoTweenConfig().scale(Vector3.one));
		yield return new WaitForSeconds(3f);
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
		Go.to(base.transform, 0.5f, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(hidePos)).scale(Vector3.zero));
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
		closeButton.gameObject.SetActive(false);
		GUIButton gUIButton = closeButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Hide));
		m_readyToHide = false;
		StartCoroutine(Animate());
	}
}
