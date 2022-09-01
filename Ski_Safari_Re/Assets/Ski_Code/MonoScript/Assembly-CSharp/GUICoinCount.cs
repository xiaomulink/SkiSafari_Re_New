using UnityEngine;

public class GUICoinCount : MonoBehaviour
{
	public GUIDropShadowText coinCountText;

	public GUIDropShadowText addedCountText;

	public GUITransitionAnimator transitionAnimator;

	public float autoHideDelay;

	public float addedCountHideDelay = 2f;

	public float addedCountMoveDuration = 1f;

	public Vector3 addedCountPlayerOffset = new Vector3(0f, 1f, 0f);

	public Vector3 addedCountTextOffset = new Vector3(0.5f, 0.25f, 0f);

	private int m_lastCoinsCollected = -1;

	private float m_autoHideTimer;

	private float m_addedCountHideTimer;

	public void ShowAddedCoinCount(int amount)
	{
		if ((bool)addedCountText && (bool)Player.Instance && (bool)FollowCamera.Instance)
		{
			addedCountText.Text = "+" + amount.ToString("N0");
			Vector3 endValue = coinCountText.transform.localPosition + addedCountTextOffset;
			endValue.x += FontUtils.GetNumericStringWidth(coinCountText.TextMesh, GameState.CoinCount);
			Vector3 position = Player.Instance.transform.position;
			Vector3 position2 = FollowCamera.Instance.GetComponent<Camera>().WorldToViewportPoint(position);
			position2.z = coinCountText.transform.position.z;
			Vector3 position3 = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(position2);
			addedCountText.gameObject.SetActive(true);
			Go.killAllTweensWithTarget(addedCountText.gameObject.transform);
			addedCountText.transform.position = position3;
			addedCountText.transform.localScale = Vector3.one;
			Go.from(addedCountText.gameObject.transform, addedCountMoveDuration, new GoTweenConfig().scale(0.5f));
			Go.to(addedCountText.gameObject.transform, addedCountMoveDuration, new GoTweenConfig().localPosition(endValue).setEaseType(GoEaseType.QuadIn));
			m_addedCountHideTimer = addedCountHideDelay;
		}
	}

	private void OnEnable()
	{
		m_autoHideTimer = autoHideDelay;
		if ((bool)addedCountText)
		{
			m_addedCountHideTimer = 0f;
			addedCountText.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		int coinCount = GameState.CoinCount;
		if (coinCount != m_lastCoinsCollected)
		{
			coinCountText.Text = coinCount.ToString("N0");
			m_lastCoinsCollected = coinCount;
			m_autoHideTimer = autoHideDelay;
		}
		if (m_addedCountHideTimer > 0f)
		{
			m_addedCountHideTimer -= Time.deltaTime;
			if (m_addedCountHideTimer <= 0f)
			{
				addedCountText.gameObject.SetActive(false);
			}
		}
		else if (m_autoHideTimer > 0f)
		{
			m_autoHideTimer -= Time.deltaTime;
			if (m_autoHideTimer <= 0f)
			{
				transitionAnimator.Hide();
			}
		}
	}
}
