using System;
using System.Collections;
using UnityEngine;

public class GUIUpdateNews4_1 : GUIUpdateNews
{
	public AudioClip showSound;

	public AudioClip punchSound;

	public GUIButton closeButton;

	public Vector3 hidePos = new Vector3(0.5f, 0.4f, 5f);

	public GameObject updateContent;

	public GameObject[] dreamBubbles;

	public GameObject dreamCloud;

	public AudioClip popSound;

	public GameObject popEffect;

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
		SkiGameManager.Instance.StopAmbientSpawns();
		yield return new WaitForSeconds(m_initialDelay);
		SoundManager.Instance.PlaySound(showSound);
		Go.to(updateContent.transform, 0.5f, new GoTweenConfig().scale(Vector3.one));
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
		Go.to(updateContent.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero));
		Go.to(updateContent.transform, 0.5f, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(hidePos)));
		yield return new WaitForSeconds(1f);
		GameObject[] array = dreamBubbles;
		foreach (GameObject dreamBubble in array)
		{
			dreamBubble.SetActive(true);
			Go.from(dreamBubble.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero));
			yield return new WaitForSeconds(0.1f);
		}
		dreamCloud.SetActive(true);
		Go.from(dreamCloud.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero));
		float dreamTimer2 = 0f;
		float cloudRotationSpeed = 0.5f;
		float cloudRotationAmount = 5f;
		while (dreamTimer2 < 1.75f)
		{
			dreamCloud.transform.localRotation = Quaternion.AngleAxis(Mathf.Sin(dreamTimer2 * (float)Math.PI * 2f * cloudRotationSpeed) * cloudRotationAmount, Vector3.forward);
			dreamTimer2 += Time.deltaTime;
			yield return new WaitForSeconds(0f);
		}
		SkiGameManager.Instance.SpawnStartingAmbientAnimal(1);
		dreamTimer2 = 0f;
		while (dreamTimer2 < 0.75f)
		{
			dreamCloud.transform.localRotation = Quaternion.AngleAxis(Mathf.Sin(dreamTimer2 * (float)Math.PI * 2f * cloudRotationSpeed) * cloudRotationAmount, Vector3.forward);
			dreamTimer2 += Time.deltaTime;
			yield return new WaitForSeconds(0f);
		}
		popEffect.SetActive(true);
		SoundManager.Instance.PlaySound(popSound);
		dreamCloud.SetActive(false);
		GameObject[] array2 = dreamBubbles;
		foreach (GameObject dreamBubble2 in array2)
		{
			dreamBubble2.SetActive(false);
		}
		yield return new WaitForSeconds(1f);
		UnityEngine.Object.Destroy(base.gameObject);
		GameState.Synchronise();
		GUITutorials.Instance.UpdateAutoShow();
	}

	private void Hide()
	{
		m_readyToHide = true;
	}

	private void Start()
	{
		updateContent.transform.localScale = Vector3.zero;
		closeButton.gameObject.SetActive(false);
		GUIButton gUIButton = closeButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Hide));
		m_readyToHide = false;
		StartCoroutine(Animate());
	}
}
