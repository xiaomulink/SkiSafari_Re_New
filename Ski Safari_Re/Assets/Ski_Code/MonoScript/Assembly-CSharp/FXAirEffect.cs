using UnityEngine;

public class FXAirEffect : MonoBehaviour
{
	public FXTrail glideTrail;

	public float glideTrailMaxStartWidth = 1f;

	private SoundEffect m_soundEffect;

	private bool m_gliding;

	public void SetGlidingRatio(float ratio)
	{
		if (!glideTrail)
		{
			return;
		}
		if (ratio > 0f)
		{
			if (!m_gliding)
			{
				glideTrail.gameObject.SetActive(true);
				m_gliding = true;
			}
			glideTrail.StartWidth = glideTrailMaxStartWidth * ratio;
			if ((bool)m_soundEffect)
			{
				m_soundEffect.AudioSource.volume = ratio;
			}
		}
		else if (m_gliding)
		{
			glideTrail.gameObject.SetActive(false);
			m_gliding = false;
		}
	}

	private void Awake()
	{
		m_soundEffect = GetComponent<SoundEffect>();
	}

	private void OnEnable()
	{
		m_gliding = false;
		if ((bool)glideTrail)
		{
			glideTrail.gameObject.SetActive(false);
		}
	}
}
