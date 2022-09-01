using UnityEngine;

public class FXBackflipEffect : MonoBehaviour
{
	public FXTrail backflipTrail;

	public float backflipMaxStartWidth = 1f;

	public float backflipDecayFilter = 2f;

	private float m_backflipRatio;

	private float m_targetBackflipRatio;

	public void SetBackflipRatio(float ratio)
	{
		m_targetBackflipRatio = ratio;
	}

	private void OnEnable()
	{
		m_backflipRatio = (m_targetBackflipRatio = 0f);
		backflipTrail.gameObject.SetActive(false);
	}

	private void LateUpdate()
	{
		if (m_targetBackflipRatio > m_backflipRatio)
		{
			if (m_backflipRatio == 0f)
			{
				backflipTrail.gameObject.SetActive(true);
			}
			m_backflipRatio = m_targetBackflipRatio;
			backflipTrail.StartWidth = backflipMaxStartWidth * m_backflipRatio;
		}
		else if (m_backflipRatio > 0f)
		{
			m_backflipRatio = Mathf.Max(m_targetBackflipRatio, m_backflipRatio - backflipDecayFilter * Time.deltaTime);
			if (m_backflipRatio <= 0.01f)
			{
				m_targetBackflipRatio = (m_backflipRatio = 0f);
				backflipTrail.gameObject.SetActive(false);
			}
			else
			{
				backflipTrail.StartWidth = backflipMaxStartWidth * m_backflipRatio;
			}
		}
	}
}
