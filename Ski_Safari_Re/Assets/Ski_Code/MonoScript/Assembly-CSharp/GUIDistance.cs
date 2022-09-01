using UnityEngine;

public class GUIDistance : MonoBehaviour
{
	public GUIDropShadowText distanceText;

	public GUITransitionAnimator transitionAnimator;

	public float autoHideDelay;

	private int m_lastDistance = -1;

	private float m_autoHideTimer;

	private void OnEnable()
	{
		m_autoHideTimer = autoHideDelay;
	}

	private void Update()
	{
		int num = Mathf.FloorToInt(SkiGameManager.Instance.CurrentDistance);
		if (num != m_lastDistance)
		{
			distanceText.Text = num.ToString();
			m_lastDistance = num;
		}
		if (m_autoHideTimer > 0f)
		{
			m_autoHideTimer -= Time.deltaTime;
			if (m_autoHideTimer <= 0f)
			{
				transitionAnimator.Hide();
			}
		}
	}
}
