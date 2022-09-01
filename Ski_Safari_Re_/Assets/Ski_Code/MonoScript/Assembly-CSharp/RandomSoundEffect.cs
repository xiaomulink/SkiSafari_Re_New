using System;
using UnityEngine;

[AddComponentMenu("Audio/RandomSoundEffect")]
public class RandomSoundEffect : MonoBehaviour
{
	public AudioSourceSettings audioSourceSettings;

	public Sound[] sounds;

	public bool playOnAwake = true;

	public float volume = 1f;

	private AudioSource m_audio;

	private bool m_played;

	public void PlayRandomSound()
	{
		if (SoundManager.Instance.SFXEnabled && sounds.Length > 0)
		{
			m_audio.loop = false;
			m_audio.PlayOneShot(sounds[UnityEngine.Random.Range(0, sounds.Length)].clip);
		}
		m_played = true;
	}

	private void OnSFXChanged(bool sfxEnabled)
	{
		if (!sfxEnabled)
		{
			m_audio.Stop();
		}
	}

	private void Awake()
	{
		if (!audioSourceSettings)
		{
			m_audio = GetComponent<AudioSource>();
		}
		else
		{
			m_audio = audioSourceSettings.CreateSource(base.gameObject);
		}
		m_audio.volume = volume;
	}

	private void OnEnable()
	{
		m_played = false;
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Combine(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}

	private void OnDisable()
	{
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Remove(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}

	private void Update()
	{
		if (playOnAwake && !m_played)
		{
			PlayRandomSound();
		}
	}
}
