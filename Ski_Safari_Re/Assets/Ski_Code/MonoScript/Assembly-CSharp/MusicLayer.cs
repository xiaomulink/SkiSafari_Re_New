using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicLayer : MonoBehaviour
{
	private enum State
	{
		None = 0,
		WaitingToPlay = 1,
		FadingIn = 2,
		Playing = 3,
		FadingOutToStop = 4,
		FadingOutToPause = 5,
		FadingOutToDuck = 6,
		Muted = 7
	}

	private float m_volume;

	private float m_startVolume;

	private float m_fadeDuration;

	private float m_fadeStartTime;

	private State m_state;

	private int m_numberOfPlays;

	private int m_lastSamples;

	public float Volume
	{
		get
		{
			return m_volume;
		}
	}

	public int NumberOfPlays
	{
		get
		{
			return m_numberOfPlays;
		}
	}

	public void Preload(AudioClip clip, bool loop, float targetVolume)
	{
		m_volume = targetVolume;
		m_startVolume = 0f;
		m_numberOfPlays = 0;
		m_lastSamples = 0;
		GetComponent<AudioSource>().volume = 0f;
		GetComponent<AudioSource>().clip = clip;
		GetComponent<AudioSource>().loop = loop;
		GetComponent<AudioSource>().Play();
		m_state = State.WaitingToPlay;
	}

	public bool IsWaitingToPlay()
	{
		return GetComponent<AudioSource>().isPlaying && m_state == State.WaitingToPlay;
	}

	public bool IsFadingIn()
	{
		return m_state == State.FadingIn;
	}

	public bool IsFadingInOrPlaying()
	{
		return m_state == State.Playing || m_state == State.FadingIn;
	}

	public bool IsStopped()
	{
		return m_state == State.None || m_state == State.FadingOutToStop || m_state == State.FadingOutToPause || m_state == State.FadingOutToDuck || m_state == State.Muted;
	}

	public AudioClip GetClip()
	{
		return GetComponent<AudioSource>().clip;
	}

	public void SetVolume(float volume)
	{
		m_volume = volume;
		if (IsFadingInOrPlaying())
		{
			GetComponent<AudioSource>().volume = m_volume;
		}
	}

	public void Play(float fadeDuration, int timeInSamples)
	{
		GetComponent<AudioSource>().timeSamples = timeInSamples % GetComponent<AudioSource>().clip.samples;
		m_numberOfPlays = ((timeInSamples < GetComponent<AudioSource>().clip.samples / 2) ? 1 : 0);
		m_lastSamples = timeInSamples;
		Play(fadeDuration);
	}

	private void Play(float fadeDuration)
	{
		m_fadeDuration = fadeDuration;
		m_fadeStartTime = Time.realtimeSinceStartup;
		m_state = State.FadingIn;
	}

	public void Stop(float fadeDuration)
	{
		m_startVolume = GetComponent<AudioSource>().volume;
		m_fadeDuration = fadeDuration;
		m_fadeStartTime = Time.realtimeSinceStartup;
		m_state = State.FadingOutToStop;
	}

	public void Pause(float fadeDuration)
	{
		if (IsFadingInOrPlaying())
		{
			m_startVolume = GetComponent<AudioSource>().volume;
			m_fadeDuration = fadeDuration;
			m_fadeStartTime = Time.realtimeSinceStartup;
			m_state = State.FadingOutToPause;
		}
	}

	public void Duck(float fadeDuration, float duckVolume)
	{
		if (IsFadingInOrPlaying())
		{
			m_volume = duckVolume;
			m_startVolume = GetComponent<AudioSource>().volume;
			m_fadeDuration = fadeDuration;
			m_fadeStartTime = Time.realtimeSinceStartup;
			m_state = State.FadingOutToDuck;
		}
	}

	public void Mute()
	{
		m_state = State.Muted;
		GetComponent<AudioSource>().volume = 0f;
	}

	private bool UpdateFade(float targetVolume)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - m_fadeStartTime;
		if (num > m_fadeDuration)
		{
			GetComponent<AudioSource>().volume = targetVolume;
			return true;
		}
		float t = num / m_fadeDuration;
		GetComponent<AudioSource>().volume = Mathf.Lerp(m_startVolume, targetVolume, Mathf.Log10(Mathf.Lerp(1f, 10f, t)));
		return false;
	}

	private void CheckLoop()
	{
		int timeSamples = GetComponent<AudioSource>().timeSamples;
		if (GetComponent<AudioSource>().clip != null && timeSamples < m_lastSamples - GetComponent<AudioSource>().clip.samples / 2)
		{
			m_numberOfPlays++;
		}
		m_lastSamples = timeSamples;
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.FadingIn:
			if (UpdateFade(m_volume))
			{
				m_state = State.Playing;
			}
			CheckLoop();
			break;
		case State.FadingOutToStop:
			if (UpdateFade(0f))
			{
				GetComponent<AudioSource>().Stop();
				GetComponent<AudioSource>().clip = null;
				m_state = State.None;
			}
			break;
		case State.FadingOutToPause:
			if (UpdateFade(0f))
			{
				m_state = State.WaitingToPlay;
			}
			break;
		case State.FadingOutToDuck:
			if (UpdateFade(m_volume))
			{
				m_state = State.WaitingToPlay;
			}
			break;
		case State.Playing:
			if (!GetComponent<AudioSource>().isPlaying && !GetComponent<AudioSource>().loop)
			{
				m_state = State.None;
			}
			else
			{
				CheckLoop();
			}
			break;
		}
	}
}
