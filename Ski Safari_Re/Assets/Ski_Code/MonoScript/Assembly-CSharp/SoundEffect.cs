using System;
using UnityEngine;

[AddComponentMenu("Audio/SoundEffect")]
public class SoundEffect : MonoBehaviour
{
	public AudioSourceSettings audioSourceSettings;

	public Sound sound;

	public bool playOnAwake = true;

	public bool loop;

	public float volume = 1f;

	private AudioSource m_audio;

	public AudioSource AudioSource
	{
		get
		{
			return m_audio;
		}
	}

	public void PlaySound()
	{
		if ((bool)SoundManager.Instance && SoundManager.Instance.SFXEnabled && (!m_audio.loop || !m_audio.isPlaying))
		{
			m_audio.clip = sound.clip;
			m_audio.Play();
		}
	}

	public void StopSound()
	{
		m_audio.Stop();
	}

	private void OnSFXChanged(bool sfxEnabled)
	{
		if (sfxEnabled)
		{
			if (playOnAwake && m_audio.loop)
			{
				m_audio.Play();
			}
		}
		else
		{
			m_audio.Stop();
		}
	}

	private void Awake()
	{
		m_audio = audioSourceSettings.CreateSource(base.gameObject);
		m_audio.loop = loop;
		m_audio.volume = volume;
	}

	private void OnEnable()
	{
		if (playOnAwake)
		{
			PlaySound();
		}
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Combine(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}

	private void OnDisable()
	{
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Remove(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}
}
