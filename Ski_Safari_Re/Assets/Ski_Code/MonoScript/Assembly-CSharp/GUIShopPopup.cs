using System;
using UnityEngine;

public class GUIShopPopup : MonoBehaviour
{
	public GUITransitionAnimator transitionAnimator;

	public GUIButton okayButton;

	public Sound okayShowSound;

	public float okayShowDelay = 4f;

	public Action OnClosed;

	private float m_pulseTimer;

	private void ShowOkayButton()
	{
		okayButton.gameObject.SetActive(true);
		SoundManager.Instance.PlaySound(okayShowSound);
	}

	private void Close()
	{
		transitionAnimator.Hide();
		SkiGameManager.Instance.PopupEnabled = false;
		if (OnClosed != null)
		{
			OnClosed();
		}
	}

	protected virtual void Awake()
	{
		SkiGameManager.Instance.PopupEnabled = true;
		if ((bool)okayButton)
		{
			GUIButton gUIButton = okayButton;
			gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Close));
			Invoke("ShowOkayButton", okayShowDelay);
		}
	}

	private void Update()
	{
		bool flag = false;
		if ((bool)okayButton && okayButton.gameObject.activeInHierarchy && !flag)
		{
			m_pulseTimer += Time.deltaTime;
			float num = 1f + Mathf.Sin(m_pulseTimer * 3f) * 0.1f;
			okayButton.transform.localScale = new Vector3(num, num, num);
		}
	}
}
