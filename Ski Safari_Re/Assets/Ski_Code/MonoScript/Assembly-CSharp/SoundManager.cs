using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public delegate void OnSFXChangedDelegate(bool sfxEnabled);

	public static SoundManager Instance;

	public float musicVolume = 0.35f;

	public float stingVolume = 0.5f;

	public float fadeInDuration = 1f;

	public float fadeOutDuration = 1f;

	public float crossFadeDuration = 2f;

	public float stingFadeOutDuration = 0.5f;

	public MusicLayer layerPrefab;

	public float minPitch = 0.75f;

	public int syncQueueSamples = 1000;

	public int syncDelaySamples = 1900;

	public static OnSFXChangedDelegate OnSFXChanged;

	private AudioSource m_audio;

	private bool m_sfxEnabled = true;

	private bool m_musicEnabled = true;

	private MusicLayer stingLayer;

	private MusicLayer m_currentMusicLayer;

	private MusicLayer m_nextMusicLayer;

	private MusicLayer m_lastMusicLayer;

	private bool m_syncNextMusicLayer;

	private float m_musicVolumeScale = 1f;

	private AudioClip m_lastMusicClip;

	public bool MusicEnabled
	{
		get
		{
			return m_musicEnabled;
		}
		set
		{
			PlayerPrefs.SetInt("MusicDisabled", (!value) ? 1 : 0);
			m_musicEnabled = value;
		}
	}

	public bool SFXEnabled
	{
		get
		{
			return m_sfxEnabled;
		}
		set
		{
			PlayerPrefs.SetInt("SFXDisabled", (!value) ? 1 : 0);
			if (m_sfxEnabled != value)
			{
				m_sfxEnabled = value;
				if (OnSFXChanged != null)
				{
					OnSFXChanged(m_sfxEnabled);
				}
			}
		}
	}

	public void PlaySound(AudioClip clip)
	{
		if (clip != null)
		{
		}
		if ((bool)clip && SFXEnabled)
		{
			m_audio.loop = false;
			m_audio.PlayOneShot(clip);
		}
	}

	public void PlayRandomSound(AudioClip[] clips)
	{
		if (SFXEnabled && clips.Length > 0)
		{
			AudioClip clip = clips[Random.Range(0, clips.Length)];
			m_audio.loop = false;
			m_audio.PlayOneShot(clip);
		}
	}

	public void PlaySound(Sound sound)
	{
		if ((bool)sound && (bool)sound.clip && SFXEnabled)
		{
			m_audio.loop = false;
			m_audio.PlayOneShot(sound.clip);
		}
	}

	public void PlayRandomSound(Sound[] sounds)
	{
		if (SFXEnabled && sounds.Length > 0)
		{
			AudioClip clip = sounds[Random.Range(0, sounds.Length)].clip;
			m_audio.loop = false;
			m_audio.PlayOneShot(clip);
		}
	}

	public void UpdatePitch()
	{
		if (!m_musicEnabled)
		{
		}
	}

	public void SetMusicVolumeScale(float scale)
	{
		if (m_musicEnabled)
		{
			m_musicVolumeScale = scale;
			m_currentMusicLayer.SetVolume(musicVolume * scale);
			m_nextMusicLayer.SetVolume(musicVolume * scale);
			m_lastMusicLayer.SetVolume(musicVolume * scale);
		}
	}

	public MusicLayer GetCurrentMusicLayer()
	{
		return m_currentMusicLayer;
	}

	public void DuckMusic(float volumeRatio)
	{
		if ((bool)m_nextMusicLayer)
		{
			m_nextMusicLayer.Duck(fadeOutDuration, volumeRatio * m_musicVolumeScale);
			if ((bool)m_currentMusicLayer)
			{
				m_currentMusicLayer.Stop(fadeOutDuration);
			}
		}
		else if ((bool)m_currentMusicLayer)
		{
			m_currentMusicLayer.Duck(fadeOutDuration, volumeRatio * m_musicVolumeScale);
		}
	}

	public void StopMusic()
	{
		if ((bool)m_nextMusicLayer)
		{
			m_nextMusicLayer.Stop(fadeOutDuration);
		}
		if ((bool)m_currentMusicLayer)
		{
			m_currentMusicLayer.Stop(fadeOutDuration);
		}
		m_lastMusicClip = null;
	}

	public void PlayMusic(Sound sound, bool syncTime)
	{
		if (!m_musicEnabled)
		{
			m_lastMusicClip = sound.clip;
			return;
		}
		if (m_currentMusicLayer.IsFadingInOrPlaying() && sound.clip == m_currentMusicLayer.GetClip())
		{
			m_currentMusicLayer.SetVolume(musicVolume * m_musicVolumeScale);
			return;
		}
		m_nextMusicLayer.Preload(sound.clip, true, musicVolume * m_musicVolumeScale);
		m_syncNextMusicLayer = syncTime && !m_currentMusicLayer.IsStopped();
	}

	public void PlaySting(Sound sound, bool loop)
	{
		if (MusicEnabled)
		{
			stingLayer.Preload(sound.clip, loop, stingVolume);
		}
	}

	public void StopSting(Sound sound)
	{
		if ((bool)stingLayer && (!sound || !sound.clip || stingLayer.GetClip() == sound.clip))
		{
			stingLayer.Stop(stingFadeOutDuration);
		}
	}

	private void SwapMusicLayers()
	{
		MusicLayer currentMusicLayer = m_currentMusicLayer;
		m_currentMusicLayer = m_nextMusicLayer;
		m_nextMusicLayer = m_lastMusicLayer;
		m_lastMusicLayer = currentMusicLayer;
		if (m_lastMusicLayer.IsWaitingToPlay())
		{
			m_lastMusicLayer.Stop(0f);
		}
	}

	private void UpdateMusicLayers()
	{
		if (!MusicEnabled)
		{
			if (m_currentMusicLayer.IsFadingInOrPlaying())
			{
				m_lastMusicClip = m_currentMusicLayer.GetClip();
				m_currentMusicLayer.Stop(fadeOutDuration);
			}
		}
		else if (m_nextMusicLayer.IsWaitingToPlay())
		{
			int num = 0;
			if (m_syncNextMusicLayer)
			{
				if (m_currentMusicLayer.NumberOfPlays == 0)
				{
					return;
				}
				if ((bool)m_currentMusicLayer.GetComponent<AudioSource>().clip)
				{
					num = m_currentMusicLayer.GetComponent<AudioSource>().timeSamples;
					int num2 = m_currentMusicLayer.GetComponent<AudioSource>().clip.samples - num;
					if (num2 > syncQueueSamples)
					{
						return;
					}
					int samples = m_nextMusicLayer.GetComponent<AudioSource>().clip.samples;
					while (num > samples)
					{
						num -= samples;
					}
				}
				num += syncDelaySamples;
			}
			float fadeDuration = fadeInDuration;
			if (m_currentMusicLayer.IsFadingInOrPlaying())
			{
				fadeDuration = crossFadeDuration;
				m_currentMusicLayer.Stop(fadeDuration);
			}
			m_nextMusicLayer.Play(fadeDuration, num);
			SwapMusicLayers();
		}
		else if (m_currentMusicLayer.IsWaitingToPlay())
		{
			m_currentMusicLayer.Play(fadeInDuration, 0);
		}
		else if ((bool)m_lastMusicClip)
		{
			m_nextMusicLayer.Preload(m_lastMusicClip, true, musicVolume * m_musicVolumeScale);
			m_lastMusicClip = null;
		}
	}

	private void Awake()
	{
		Instance = this;
		m_audio = GetComponent<AudioSource>();
		stingLayer = Object.Instantiate(layerPrefab);
		m_currentMusicLayer = Object.Instantiate(layerPrefab);
		m_nextMusicLayer = Object.Instantiate(layerPrefab);
		m_lastMusicLayer = Object.Instantiate(layerPrefab);
		m_sfxEnabled = false;
		m_musicEnabled = false;
	}

	private void Start()
	{
		Transform parent = Camera.main.transform;
		stingLayer.transform.parent = parent;
		m_currentMusicLayer.transform.parent = parent;
		m_nextMusicLayer.transform.parent = parent;
		m_lastMusicLayer.transform.parent = parent;
		base.transform.parent = parent;
		m_sfxEnabled = PlayerPrefs.GetInt("SFXDisabled") == 0;
		m_musicEnabled = PlayerPrefs.GetInt("MusicDisabled") == 0;
	}

	private void Update()
	{
		if (stingLayer.IsWaitingToPlay())
		{
			m_currentMusicLayer.Mute();
			m_nextMusicLayer.Mute();
			m_lastMusicLayer.Mute();
			stingLayer.Play(0f, 0);
		}
		else if (stingLayer.IsFadingInOrPlaying())
		{
			if (!MusicEnabled)
			{
				stingLayer.Stop(fadeOutDuration);
			}
		}
		else
		{
			UpdateMusicLayers();
		}
		UpdatePitch();
	}
}
