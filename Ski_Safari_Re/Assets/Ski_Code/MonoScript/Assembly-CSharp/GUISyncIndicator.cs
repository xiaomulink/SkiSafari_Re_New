using UnityEngine;

public class GUISyncIndicator : MonoBehaviour
{
	public float showDuration = 2f;

	public GUITransitionAnimator icon;

	public GameObject refreshIcon;

	public GameObject syncIcon;

	private bool m_refreshing;

	private float m_hideTimer;

	private void OnGameStateLoadRequested()
	{
		Show(true);
	}

	private void OnGameStateSynchronise()
	{
		Show(false);
	}

	private void Show(bool refreshing)
	{
		m_hideTimer = showDuration;
		m_refreshing = refreshing;
		if (icon.IsShowing)
		{
			refreshIcon.SetActive(m_refreshing);
			syncIcon.SetActive(!m_refreshing);
		}
	}

	private void Hide()
	{
		m_hideTimer = 0f;
		icon.Hide();
	}

	private void Awake()
	{
	}

	private void Update()
	{
		if (m_hideTimer > 0f && (bool)SkiGameManager.Instance && SkiGameManager.Instance.TitleScreenActive && (bool)GUITutorials.Instance && !GUITutorials.Instance.IsShowing && !GUITutorials.Instance.AutoShow)
		{
			if (!icon.IsShowing)
			{
				icon.Show();
				refreshIcon.SetActive(m_refreshing);
				syncIcon.SetActive(!m_refreshing);
			}
			m_hideTimer -= Time.deltaTime;
		}
		else if (icon.IsShowing)
		{
			Hide();
		}
	}
}
