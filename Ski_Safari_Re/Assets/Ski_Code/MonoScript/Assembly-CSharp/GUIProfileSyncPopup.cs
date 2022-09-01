using System;
using System.Collections;
using UnityEngine;

public class GUIProfileSyncPopup : MonoBehaviour
{
	public Sound showSound;

	public GUIButton okayButton;

	public Sound okayShowSound;

	public GUITransitionAnimator transitionAnimator;

	public int maxProfilesBeforeIgnoring = 2;

	private bool m_readyToHide;

	private void Okay()
	{
		m_readyToHide = true;
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
		okayButton.gameObject.SetActive(false);
		StartCoroutine(Animate());
	}
}
