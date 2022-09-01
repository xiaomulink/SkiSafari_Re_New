using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	public static FollowCamera Instance;

	public float lookAheadDistance = 20f;

	public float terrainHeightMin;

	public float terrainHeightMax = 50f;

	public Vector3 groundOffsetClose = new Vector3(10f, 0f, -20f);

	public Vector3 groundOffsetFar = new Vector3(10f, 0f, -10f);

	public Vector3 airOffsetClose = new Vector3(40f, -30f, -100f);

	public Vector3 airOffsetFar = new Vector3(40f, -30f, -80f);

	public Vector3 idleOffset = new Vector3(0f, 0f, -10f);

	public Vector3 titleOffset = new Vector3(0f, 0f, -10f);

	public Vector3 shopOffset = new Vector3(0f, 10f, -10f);

	public Vector3 debugOffset = new Vector3(0f, 0f, -10f);

	public float avalancheCloseDistance = 20f;

	public float avalancheFarDistance = 100f;

	public float offsetFilter = 4f;

	public Vector3 positionFilter = new Vector3(15f, 15f, 15f);

	public AudioListener audioListener;

	public Vector3 audioListenerOffset = new Vector3(0f, 0f, -10f);

	private bool m_snapNextUpdate = true;

	private float m_lastFixedUpdateTime;

	private Vector3 m_lastTargetPos;

	private Vector3 m_filteredOffset;

	private Vector3 m_playerChangeOffset;

	private float m_followerOffset;

	private float m_shakeTimer;

	private float m_shakeDuration = 1f;

	private float m_shakeMagnitude;

	public bool IsStationary
	{
		get
		{
			return Vector3.SqrMagnitude(m_lastTargetPos + m_filteredOffset - base.transform.position) < 0.1f;
		}
	}

	public void Snap()
	{
		m_snapNextUpdate = true;
		UpdatePosition();
	}

	public void AddPlayerChangeOffset(Vector3 offset)
	{
		m_playerChangeOffset -= offset;
	}

	public void Shake(float duration, float magnitude)
	{
		float num = m_shakeMagnitude * (m_shakeTimer / m_shakeDuration);
		if (magnitude > num)
		{
			m_shakeTimer = (m_shakeDuration = duration);
			m_shakeMagnitude = magnitude;
		}
	}

	private Vector3 GetTargetPos()
	{
		if ((bool)Player.Instance)
		{
			return Player.Instance.LookAtPos;
		}
		if ((bool)SkiGameManager.Instance && !SkiGameManager.Instance.Started && !SkiGameManager.Instance.ShowShop)
		{
			return SkiGameManager.Instance.StartPos;
		}
		return m_lastTargetPos;
	}

	private void Awake()
	{
		Instance = this;
	}

	public void UpdatePosition()
	{
		Vector3 targetPos = GetTargetPos();
		float num = avalancheFarDistance;
		if ((bool)Avalanche.Instance)
		{
			num = Vector3.Distance(targetPos, Avalanche.Instance.transform.position);
		}
		Vector3 zero = Vector3.zero;
		if (SkiGameManager.Instance.TitleScreenActive)
		{
			zero = titleOffset;
		}
		else if (SkiGameManager.Instance.ShowShop)
		{
			zero = shopOffset;
		}
		else if (!Player.Instance && !SkiGameManager.Instance.Finished)
		{
			zero = idleOffset;
		}
		else if ((bool)Player.Instance && Player.Instance.UseCameraOffsetOverride)
		{
			zero = Player.Instance.CameraOffsetOverride;
		}
		else
		{
			float t = Mathf.Clamp01((num - avalancheCloseDistance) / (avalancheFarDistance - avalancheCloseDistance));
			Vector3 vector = Vector3.Lerp(groundOffsetClose, groundOffsetFar, t);
			Vector3 b = Vector3.Lerp(airOffsetClose, airOffsetFar, t);
			zero = vector;
			Terrain terrainForLayer = Terrain.GetTerrainForLayer(TerrainLayer.Game);
			if ((bool)terrainForLayer)
			{
				Vector3 pos = targetPos;
				pos.x += lookAheadDistance;
				float height = 0f;
				if (terrainForLayer.GetCameraHeight(pos, ref height))
				{
					float t2 = Mathf.Clamp01((height - terrainHeightMin) / (terrainHeightMax - terrainHeightMin));
					zero = Vector3.Lerp(vector, b, t2);
				}
			}
		}
		Vector3 zero2 = Vector3.zero;
		if (m_shakeTimer > 0f)
		{
			float num2 = m_shakeMagnitude * (m_shakeTimer / m_shakeDuration) * Time.deltaTime * 60f;
			zero2.x = Random.Range(0f - num2, num2);
			zero2.y = Random.Range(0f - num2, num2);
			m_shakeTimer -= Time.deltaTime;
		}
		if (m_snapNextUpdate)
		{
			m_filteredOffset = zero + zero2;
			m_snapNextUpdate = false;
		}
		else
		{
			m_filteredOffset = Vector3.Lerp(m_filteredOffset, zero, offsetFilter * Time.deltaTime) + zero2;
			m_playerChangeOffset.x = Mathf.Lerp(m_playerChangeOffset.x, 0f, positionFilter.x * Time.deltaTime);
			targetPos.x += m_playerChangeOffset.x;
			targetPos.y = Mathf.Lerp(m_lastTargetPos.y, targetPos.y, positionFilter.y * Time.deltaTime);
			if (!Player.Instance)
			{
				targetPos.x = Mathf.Lerp(m_lastTargetPos.x, targetPos.x, positionFilter.x * Time.deltaTime);
				targetPos.z = Mathf.Lerp(m_lastTargetPos.z, targetPos.z, positionFilter.z * Time.deltaTime);
			}
		}
		audioListener.transform.position = targetPos + audioListenerOffset;
		base.transform.position = targetPos + m_filteredOffset;
		m_lastTargetPos = targetPos;
	}

	private void LateUpdate()
	{
		UpdatePosition();
	}
}
