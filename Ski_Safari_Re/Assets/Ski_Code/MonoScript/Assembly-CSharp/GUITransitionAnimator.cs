using UnityEngine;

public class GUITransitionAnimator : MonoBehaviour
{
	public enum InitialState
	{
		Hidden = 0,
		Visible = 1,
		TransitionIn = 2
	}

	public Vector3 onScreenPos;

	public Vector3 offScreenPos;

	public float transitionInTime = 1f;

	public float transitionOutTime = 1f;

	public Sound transitionInSound;

	public Sound transitionOutSound;

	public InitialState initialState = InitialState.Visible;

	public bool destroyOnDeactivate;

	public bool disallowDeactivate;

	private bool m_showing;

	private bool m_initialising = true;

	public bool IsShowing
	{
		get
		{
			return m_showing;
		}
	}

	public void Show()
	{
		if (m_showing)
		{
			return;
		}
		m_initialising = false;
		CancelInvoke("Initialise");
		SoundManager.Instance.PlaySound(transitionInSound);
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
			if (initialState != InitialState.Visible)
			{
				base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
			}
		}
		Go.killAllTweensWithTarget(base.transform);
		Go.to(base.transform, transitionInTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos)));
		CancelInvoke("Deactivate");
		m_showing = true;
	}

	public void SnapShow()
	{
		if (!m_showing)
		{
			m_initialising = false;
			CancelInvoke("Initialise");
			if (!base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(true);
			}
			Go.killAllTweensWithTarget(base.transform);
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos);
			m_showing = true;
		}
	}

	public void Hide()
	{
		if (m_showing)
		{
			m_initialising = false;
			CancelInvoke("Initialise");
			SoundManager.Instance.PlaySound(transitionOutSound);
			Go.killAllTweensWithTarget(base.transform);
			Go.to(base.transform, transitionOutTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos)));
			Invoke("Deactivate", transitionOutTime);
			m_showing = false;
		}
	}

	public void SnapHide()
	{
		m_initialising = false;
		CancelInvoke("Initialise");
		Go.killAllTweensWithTarget(base.transform);
		base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
		Deactivate();
		m_showing = false;
	}

	public void DebugSnap()
	{
		if (m_showing)
		{
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos);
		}
	}

	public void MoveTo(Vector3 screenPos)
	{
		if (m_showing)
		{
			Go.killAllTweensWithTarget(base.transform);
			Go.to(base.transform, transitionInTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(screenPos)));
		}
	}

	private void Deactivate()
	{
		if (destroyOnDeactivate)
		{
			Object.Destroy(base.gameObject);
		}
		else if (!disallowDeactivate)
		{
			Go.killAllTweensWithTarget(base.transform);
			base.gameObject.SetActive(false);
		}
	}

	private void Initialise()
	{
		switch (initialState)
		{
		case InitialState.Hidden:
			if ((bool)SkiGameManager.Instance)
			{
				base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
			}
			if (!disallowDeactivate)
			{
				base.gameObject.SetActive(false);
			}
			m_showing = false;
			break;
		case InitialState.Visible:
			Go.killAllTweensWithTarget(base.transform);
			if ((bool)SkiGameManager.Instance)
			{
				base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos);
			}
			m_showing = true;
			break;
		case InitialState.TransitionIn:
			if ((bool)SkiGameManager.Instance)
			{
				base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
			}
			Go.to(base.transform, transitionInTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos)));
			m_showing = true;
			break;
		}
	}

	private void OnEnable()
	{
		CancelInvoke("Deactivate");
		m_showing = false;
		if (m_initialising)
		{
			Invoke("Initialise", 0f);
		}
	}

	private void OnDisable()
	{
		m_initialising = true;
	}
   
}
