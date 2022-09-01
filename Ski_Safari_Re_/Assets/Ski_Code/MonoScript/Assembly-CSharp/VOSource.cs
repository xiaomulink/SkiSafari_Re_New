using UnityEngine;

[AddComponentMenu("Audio/VOSource")]
public class VOSource : MonoBehaviour
{
	public AudioSourceSettings audioSourceSettings;

	public Sound[] chatterSounds;

	public float minTimeBetweenChatter = 3f;

	public float maxTimeBetweenChatter = 10f;

	private AudioSource m_audio;

	private float m_chatterTimer;

	public void PlayRandomVO(Sound[] sounds, bool force)
	{
		if ((force || !m_audio.isPlaying) && sounds.Length > 0 && SoundManager.Instance.SFXEnabled)
		{
			m_audio.clip = sounds[Random.Range(0, sounds.Length)].clip;
			m_audio.loop = false;
			m_audio.Play();
		}
	}

	public void PlayRandomVO(Sound[] sounds)
	{
		PlayRandomVO(sounds, false);
	}

	public void PlayVO(Sound sound, bool force)
	{
		if ((bool)sound && (bool)sound.clip && (force || !m_audio.isPlaying) && SoundManager.Instance.SFXEnabled)
		{
			m_audio.clip = sound.clip;
			m_audio.loop = false;
			m_audio.Play();
		}
	}

	public void PlayVO(Sound sound)
	{
		PlayVO(sound, false);
	}

	public void PlayOneShotVO(Sound sound, bool force)
	{
		if ((bool)sound && (bool)sound.clip && (force || !m_audio.isPlaying) && SoundManager.Instance.SFXEnabled)
		{
			m_audio.PlayOneShot(sound.clip);
		}
	}

	private void Awake()
	{
		m_audio = audioSourceSettings.CreateSource(base.gameObject);
	}

	protected void OnDisable()
	{
		m_audio.Stop();
		m_audio.playOnAwake = false;
	}

	protected void Start()
	{
		m_chatterTimer = Random.Range(minTimeBetweenChatter, maxTimeBetweenChatter);
	}

	protected void Update()
	{
		if (chatterSounds.Length > 0)
		{
			m_chatterTimer -= Time.deltaTime;
			if (m_chatterTimer <= 0f)
			{
				PlayRandomVO(chatterSounds);
				m_chatterTimer = Random.Range(minTimeBetweenChatter, maxTimeBetweenChatter);
			}
		}
	}
}
