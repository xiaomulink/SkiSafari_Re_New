using UnityEngine;

public class EngineSounds : MonoBehaviour
{
	public AudioSourceSettings audioSourceSettings;

	public Sound groundLoopSound;

	public Sound airLoopSound;

	public CircleCollider circleCollider;

	public float groundVolumeFilter = 30f;

	public float groundPitchMin = 1f;

	public float groundPitchMax = 1.5f;

	public float groundVolumeMin = 0.55f;

	public float groundVolumeMax = 0.85f;

	public float airLoopDelay = 1f;

	public float airPitchMin = 0.75f;

	public float airPitchMax = 1.25f;

	public float airHeightMin = 10f;

	public float airHeightMax = 100f;

	public float airPitchFilter = 5f;

	private AudioSource m_audio;

	private float m_airTimer;

	private int m_lastGroundLoopSamples;

	private void Awake()
	{
		m_audio = audioSourceSettings.CreateSource(base.gameObject);
		m_audio.loop = true;
	}

	private void OnEnable()
	{
		m_airTimer = 0f;
		m_lastGroundLoopSamples = 0;
		if ((bool)SoundManager.Instance && SoundManager.Instance.SFXEnabled)
		{
			m_audio.clip = groundLoopSound.clip;
			m_audio.Play();
		}
	}

	private void Update()
	{
		if ((bool)SoundManager.Instance && SoundManager.Instance.SFXEnabled)
		{
			if (circleCollider.OnGround)
			{
				m_airTimer = 0f;
				if (m_audio.clip != groundLoopSound.clip)
				{
					m_audio.clip = groundLoopSound.clip;
					m_audio.timeSamples = m_lastGroundLoopSamples;
					m_audio.Play();
				}
				m_audio.pitch = groundPitchMin;
				m_audio.volume = Mathf.Lerp(m_audio.volume, groundVolumeMax, groundVolumeFilter * Time.deltaTime);
				return;
			}
			m_airTimer += Time.deltaTime;
			if (m_airTimer >= airLoopDelay)
			{
				if (m_audio.clip == groundLoopSound.clip)
				{
					m_lastGroundLoopSamples = m_audio.timeSamples;
					m_audio.clip = airLoopSound.clip;
					m_audio.timeSamples = 0;
					m_audio.pitch = 1f;
					m_audio.volume = 1f;
					m_audio.Play();
				}
				float height = 0f;
				Terrain terrainForLayer = Terrain.GetTerrainForLayer(TerrainLayer.Game);
				terrainForLayer.GetHeight(base.transform.position.x, ref height);
				float num = base.transform.position.y - height;
				float t = Mathf.Clamp01((num - airHeightMin) / (airHeightMax - airHeightMin));
				float b = Mathf.Lerp(airPitchMin, airPitchMax, t);
				m_audio.pitch = Mathf.Lerp(m_audio.pitch, b, airPitchFilter);
			}
			else
			{
				float t2 = m_airTimer / airLoopDelay;
				m_audio.pitch = Mathf.Lerp(1f, groundPitchMax, t2);
				m_audio.volume = Mathf.Lerp(1f, groundVolumeMin, t2);
			}
		}
		else
		{
			m_audio.Stop();
		}
	}
}
