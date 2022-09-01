using System.Collections;
using UnityEngine;

public abstract class GUIPopup : MonoBehaviour
{
	public GUITransitionAnimator transitionAnimator;

	public Transform pulseButton;

	public float inputDelay = 2f;

	private bool m_readyToHide;

	protected bool ReadyToHide
	{
		get
		{
			return m_readyToHide;
		}
		set
		{
			m_readyToHide = value;
		}
	}

	public abstract bool ShouldShow();

	protected virtual void OnShow()
	{
	}

	protected virtual void OnHide()
	{
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
		yield return new WaitForSeconds(transitionAnimator.transitionInTime + inputDelay);
		OnShow();
		float pulseTimer = 0f;
		while (!m_readyToHide)
		{
			if ((bool)pulseButton)
			{
				pulseTimer += Time.deltaTime;
				float scale = 1f + Mathf.Sin(pulseTimer * 3f) * 0.1f;
				pulseButton.localScale = new Vector3(scale, scale, scale);
			}
			yield return new WaitForSeconds(0f);
		}
		OnHide();
		transitionAnimator.Hide();
		yield return new WaitForSeconds(transitionAnimator.transitionOutTime);
		SkiGameManager.Instance.guiTitle.transitionAnimator.Show();
		SkiGameManager.Instance.guiTitle.SnapToIdle();
		GUITutorials.Instance.UpdateAutoShow();
	}

	protected virtual void Awake()
	{
		StartCoroutine(Animate());
	}
}
