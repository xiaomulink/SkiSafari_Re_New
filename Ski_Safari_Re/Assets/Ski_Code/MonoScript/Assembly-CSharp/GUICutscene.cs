using UnityEngine;

public class GUICutscene : MonoBehaviour
{
	public static GUICutscene Instance;

	public string showAnim = "cutscene_show";

	public string hideAnim = "cutscene_hide";

	private bool m_showing;

	private AnimationState m_currentAnimState;

	private float m_lastUpdateTime;

	public void Show()
	{
		if (!m_showing)
		{
			base.gameObject.SetActive(true);
			PlayAnim(showAnim);
			m_showing = true;
		}
	}

	public void Hide()
	{
		if (m_showing)
		{
			PlayAnim(hideAnim);
			m_showing = false;
		}
	}

	private void PlayAnim(string anim)
	{
		GetComponent<Animation>().Play(anim);
		m_currentAnimState = GetComponent<Animation>()[anim];
		m_currentAnimState.time = 0f;
		GetComponent<Animation>().Sample();
		m_lastUpdateTime = Time.realtimeSinceStartup;
		m_showing = true;
	}

	private void Awake()
	{
		Instance = this;
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (!m_currentAnimState)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - m_lastUpdateTime;
		m_currentAnimState.time += num;
		if (m_currentAnimState.time >= m_currentAnimState.length)
		{
			m_currentAnimState = null;
			if (!m_showing)
			{
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			m_lastUpdateTime = realtimeSinceStartup;
		}
	}
}
