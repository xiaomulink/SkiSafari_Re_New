using UnityEngine;

public class GUIRolloutButton : GUIToggleButton
{
	private enum State
	{
		Hidden = 0,
		Minimised = 1,
		Expanded = 2
	}

	public Vector3 offScreenPos;

	public Vector3 minimisedScreenPos;

	public Vector3 expandedOffset;

	public float transitionTime = 0.5f;

	public Sound expandSound;

	public Sound minimiseSound;

	public GUIRolloutElementButton[] elements;

	public Vector3 elementInitialOffset = new Vector3(-1f, 1f, 0f);

	public Vector3 elementOffset = new Vector3(-1.5f, 0f, 0f);

	public Transform creditsButton;

	public Vector3 creditsButtonOffset = new Vector3(-0.5f, 0f, 0f);

	public Vector3 endPaddingOffset = new Vector3(-2f, 0f, 0f);

	public Renderer promoBreadcrumb;

	public Vector3 promoBreadcrumbMinimisedPosition;

	public Vector3 promoBreadcrumbExpandedPosition;

	private State m_state = State.Minimised;

	private string m_lastBreadcrumbTextureName;

	public bool IsShowing
	{
		get
		{
			return m_state == State.Expanded;
		}
	}

	protected override bool Toggled
	{
		get
		{
			return m_state == State.Expanded;
		}
		set
		{
			if (value)
			{
				Expand();
			}
			else
			{
				Minimise();
			}
		}
	}

	public void Expand()
	{
		if (m_state != State.Expanded)
		{
			SoundManager.Instance.PlaySound(expandSound);
			if (!base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(true);
			}
			Go.killAllTweensWithTarget(base.transform);
			Go.to(base.transform, transitionTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(minimisedScreenPos) + expandedOffset));
			CancelInvoke("FinishHide");
			if (promoBreadcrumb.gameObject.activeInHierarchy)
			{
				Go.killAllTweensWithTarget(promoBreadcrumb.transform);
				Go.to(promoBreadcrumb.transform, 0.5f, new GoTweenConfig().localPosition(promoBreadcrumbExpandedPosition));
			}
			m_state = State.Expanded;
		}
	}

	public void Minimise()
	{
		if (m_state != State.Minimised)
		{
			SoundManager.Instance.PlaySound(minimiseSound);
			HideWindows();
			if (!base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(true);
			}
			Go.killAllTweensWithTarget(base.transform);
			Go.to(base.transform, transitionTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(minimisedScreenPos)));
			CancelInvoke("FinishHide");
			MinimiseBreadcrumb();
			m_state = State.Minimised;
		}
	}

	public void Hide(bool deactivate = true)
	{
		if (m_state != 0)
		{
			CancelInvoke("Initialise");
			HideWindows();
			Go.killAllTweensWithTarget(base.transform);
			Go.to(base.transform, transitionTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos)));
			if (deactivate)
			{
				Invoke("FinishHide", transitionTime);
			}
			MinimiseBreadcrumb();
			m_state = State.Hidden;
		}
	}

	public void SnapHide()
	{
		if (m_state != 0)
		{
			CancelInvoke("Initialise");
			Go.killAllTweensWithTarget(base.transform);
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
			FinishHide();
			m_state = State.Hidden;
		}
	}

	public void Show()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		if (m_state == State.Hidden)
		{
			CancelInvoke("FinishHide");
			Go.killAllTweensWithTarget(base.transform);
			Go.to(base.transform, transitionTime, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(minimisedScreenPos)));
			m_state = State.Minimised;
			UpdateToggled();
		}
	}

	public void SnapShow()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		if (m_state == State.Hidden)
		{
			CancelInvoke("FinishHide");
			Go.killAllTweensWithTarget(base.transform);
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(minimisedScreenPos);
			m_state = State.Minimised;
			UpdateToggled();
		}
	}

	private void HideWindows()
	{
		SkiGameManager.Instance.ShowAchievementsGUI = false;
		SkiGameManager.Instance.ShowLeaderboardGUI = false;
		SkiGameManager.Instance.ShowSettingsGUI = false;
		SkiGameManager.Instance.ShowSocialSettingsGUI = false;
		SkiGameManager.Instance.ShowOtherGamesGUI = false;
		SkiGameManager.Instance.ShowFacebookGUI = false;
		GUITutorials.Instance.Hide();
	}

	private void FinishHide()
	{
		Go.killAllTweensWithTarget(base.transform);
		base.gameObject.SetActive(false);
		m_state = State.Minimised;
	}

	private void UpdateBreadcrumb()
	{
		string nextBreadcrumbTextureName = SkiGameManager.Instance.GUIOtherGames.GetNextBreadcrumbTextureName();
		if (!string.IsNullOrEmpty(nextBreadcrumbTextureName))
		{
			if (m_lastBreadcrumbTextureName != nextBreadcrumbTextureName)
			{
				if (!string.IsNullOrEmpty(m_lastBreadcrumbTextureName))
				{
					AssetManager.UnloadAsset(promoBreadcrumb.material.mainTexture);
				}
				promoBreadcrumb.material.mainTexture = AssetManager.LoadAsset<Texture>(nextBreadcrumbTextureName);
				m_lastBreadcrumbTextureName = nextBreadcrumbTextureName;
			}
			promoBreadcrumb.gameObject.SetActive(true);
		}
		else
		{
			if (!string.IsNullOrEmpty(m_lastBreadcrumbTextureName))
			{
				AssetManager.UnloadTexture(promoBreadcrumb);
			}
			promoBreadcrumb.gameObject.SetActive(false);
		}
	}

	private void MinimiseBreadcrumb()
	{
		UpdateBreadcrumb();
		if (promoBreadcrumb.gameObject.activeInHierarchy)
		{
			Go.killAllTweensWithTarget(promoBreadcrumb.transform);
			Go.to(promoBreadcrumb.transform, 0.5f, new GoTweenConfig().localPosition(promoBreadcrumbMinimisedPosition));
		}
	}

	private void Initialise()
	{
		if (m_state == State.Expanded)
		{
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(minimisedScreenPos) + expandedOffset;
		}
		else if (m_state == State.Minimised)
		{
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(minimisedScreenPos);
			m_state = State.Minimised;
		}
		UpdateBreadcrumb();
		if (promoBreadcrumb.gameObject.activeInHierarchy)
		{
			Go.killAllTweensWithTarget(promoBreadcrumb.transform);
			if (m_state == State.Expanded)
			{
				promoBreadcrumb.transform.localPosition = promoBreadcrumbExpandedPosition;
			}
			else
			{
				promoBreadcrumb.transform.localPosition = promoBreadcrumbMinimisedPosition;
			}
		}
	}

	protected void Awake()
	{
		Vector3 localPosition = elementInitialOffset;
		GUIRolloutElementButton[] array = elements;
		foreach (GUIRolloutElementButton gUIRolloutElementButton in array)
		{
			if (gUIRolloutElementButton.Available)
			{
				GUIRolloutElementButton gUIRolloutElementButton2 = TransformUtils.Instantiate(gUIRolloutElementButton, base.transform);
				gUIRolloutElementButton2.transform.localPosition = localPosition;
				localPosition += elementOffset;
			}
		}
		localPosition += creditsButtonOffset;
		creditsButton.localPosition = localPosition;
		localPosition += endPaddingOffset;
		expandedOffset.x = 0f - localPosition.x;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		CancelInvoke("Initialise");
		Invoke("Initialise", 0f);
	}
}
