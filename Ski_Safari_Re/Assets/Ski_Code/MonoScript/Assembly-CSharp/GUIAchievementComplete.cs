using System.Collections.Generic;
using UnityEngine;

public class GUIAchievementComplete : MonoBehaviour
{
	public static GUIAchievementComplete Instance;

	public GUIDropShadowText nameText;

	public float showTransitionDuration = 0.5f;

	public float showDuration = 3f;

	public float hideTransitionDuration = 0.5f;

	public Vector3 onScreenPos;

	public Vector3 offScreenPos;

	private bool m_showing;

	private List<Achievement> m_queuedAchievements = new List<Achievement>();

	public bool IsShowing
	{
		get
		{
			return m_showing;
		}
	}

	public void Show(Achievement achievement)
	{
		m_queuedAchievements.Add(achievement);
		if (!m_showing)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(true);
			}
			ShowNext();
			SkiGameManager.Instance.OnShowAchievementComplete();
		}
	}

	public void ClearAndHide()
	{
		m_queuedAchievements.Clear();
		Hide();
	}

	public void Hide()
	{
		if (m_showing)
		{
			Go.killAllTweensWithTarget(base.gameObject);
			Go.to(base.gameObject.transform, hideTransitionDuration, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos)));
			Invoke("ShowNext", hideTransitionDuration);
			m_showing = false;
		}
	}

	private void ShowNext()
	{
		if (m_queuedAchievements.Count > 0)
		{
			nameText.Text = m_queuedAchievements[0].ToString();
			nameText.TextScale = m_queuedAchievements[0].descriptionTextScale;
			base.transform.rotation = Quaternion.identity;
			m_queuedAchievements.RemoveAt(0);
			Go.killAllTweensWithTarget(base.gameObject);
			Go.to(base.gameObject.transform, showTransitionDuration, new GoTweenConfig().position(SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos)));
			GoTweenConfig config = new GoTweenConfig().rotation(new Vector3(0f, 0f, 10f)).setEaseType(GoEaseType.ElasticPunch);
			Go.to(base.gameObject.transform, 1f, config);
			Invoke("Hide", showTransitionDuration + showDuration);
			m_showing = true;
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if ((bool)SkiGameManager.Instance)
		{
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
		}
		base.gameObject.SetActive(false);
		m_showing = false;
	}
}
