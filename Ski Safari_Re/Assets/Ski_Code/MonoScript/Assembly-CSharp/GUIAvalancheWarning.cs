using UnityEngine;

public class GUIAvalancheWarning : MonoBehaviour
{
	public static GUIAvalancheWarning Instance;

	public Vector3 offScreenPos;

	public Vector3 onScreenPos;

	public float moveTime = 0.5f;

	public float cullWarningDistance = 100f;

	public float farWarningDistance = 75f;

	public float closeWarningDistance = 25f;

	public float farScale = 1.5f;

	public float closeScale = 3f;

	public float alertScaleMin = 3.5f;

	public float alertScaleMax = 4f;

	public float alertPulseSpeed = 1f;

	public GameObject alertGlow;

	public Transform alertPivot;

	public float alertShakeMagnitude = 0.1f;

	public float alertShakeRotationMagnitude = 5f;

	public float alertShakeDuration = 1f;

	public float scaleFilter = 5f;

	public float distanceFilter = 2f;

	public float toggleCooldown = 1f;

	public float initialDelay = 5f;

	private Vector3 m_lastTargetPos;

	private float m_filteredDistance;

	private bool m_firstUpdate;

	private bool m_visible;

	private bool m_alert;

	private float m_alertTimer;

	private Material m_alertGlowMaterial;

	private float m_toggleCooldownTimer;

	private Vector3 m_offScreenPosition;

	private Vector3 m_onScreenPosition;

	public void DebugSnap()
	{
		m_offScreenPosition = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
		m_onScreenPosition = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos);
		base.transform.position = ((!m_visible) ? m_offScreenPosition : m_onScreenPosition);
	}

	private void Hide()
	{
		Go.killAllTweensWithTarget(base.gameObject.transform);
		Go.to(base.gameObject.transform, moveTime, new GoTweenConfig().position(m_offScreenPosition));
		m_visible = false;
		m_toggleCooldownTimer = toggleCooldown;
	}

	private void Initialise()
	{
		m_offScreenPosition = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
		m_onScreenPosition = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(onScreenPos);
		base.transform.position = m_offScreenPosition;
		base.transform.localScale = new Vector3(farScale, farScale, farScale);
		m_alertGlowMaterial = alertGlow.GetComponent<Renderer>().material;
		alertGlow.SetActive(false);
		m_filteredDistance = farWarningDistance;
		m_toggleCooldownTimer = initialDelay;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		m_firstUpdate = true;
	}

	private void Update()
	{
		if (m_firstUpdate)
		{
			Initialise();
			m_firstUpdate = false;
		}
		if (Time.deltaTime == 0f || CreditsSpawner.Instance.EnableSpawning)
		{
			return;
		}
		if (m_toggleCooldownTimer > 0f)
		{
			m_toggleCooldownTimer -= Time.deltaTime;
		}
		if (!Avalanche.Instance)
		{
			return;
		}
		Vector3 vector = m_lastTargetPos;
		if ((bool)Player.Instance)
		{
			vector = Player.Instance.transform.position;
		}
		else
		{
			if (SkiGameManager.Instance.Finished || SkiGameManager.Instance.ShowShop)
			{
				if (m_visible)
				{
					Hide();
				}
				return;
			}
			if (!SkiGameManager.Instance.Started)
			{
				vector = SkiGameManager.Instance.StartPos;
			}
		}
		float b = Vector3.Distance(vector, Avalanche.Instance.transform.position);
		m_filteredDistance = Mathf.Lerp(m_filteredDistance, b, distanceFilter * Time.deltaTime);
		if (m_filteredDistance <= farWarningDistance)
		{
			if (!m_visible && m_toggleCooldownTimer <= 0f)
			{
				base.transform.position = m_offScreenPosition;
				Go.killAllTweensWithTarget(base.gameObject.transform);
				Go.to(base.gameObject.transform, moveTime, new GoTweenConfig().position(m_onScreenPosition));
				m_visible = true;
				m_toggleCooldownTimer = toggleCooldown;
			}
			if (m_filteredDistance <= closeWarningDistance)
			{
				if (!m_alert)
				{
					m_alertTimer = 0f;
					m_alert = true;
					alertGlow.SetActive(true);
				}
				else
				{
					m_alertTimer += Time.deltaTime;
				}
				float num = Mathf.Sin(m_alertTimer * alertPulseSpeed) * 0.5f + 0.5f;
				float num2 = Mathf.Lerp(alertScaleMin, alertScaleMax, num);
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(num2, num2, num2), scaleFilter * Time.deltaTime);
				m_alertGlowMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, num));
				Vector3 vector2 = new Vector3(Random.Range(0f - alertShakeMagnitude, alertShakeMagnitude), Random.Range(0f - alertShakeMagnitude, alertShakeMagnitude));
				base.transform.position = m_onScreenPosition + vector2;
				alertPivot.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f - alertShakeRotationMagnitude, alertShakeRotationMagnitude));
			}
			else
			{
				if (m_alert)
				{
					m_alert = false;
					alertGlow.SetActive(false);
				}
				float t = Mathf.Clamp01((m_filteredDistance - closeWarningDistance) / (farWarningDistance - closeWarningDistance));
				float num3 = Mathf.Lerp(closeScale, farScale, t);
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(num3, num3, num3), scaleFilter * Time.deltaTime);
				alertPivot.localRotation = Quaternion.identity;
			}
		}
		else if (m_filteredDistance > cullWarningDistance && m_visible && m_toggleCooldownTimer <= 0f)
		{
			Hide();
		}
		m_lastTargetPos = vector;
	}
}
