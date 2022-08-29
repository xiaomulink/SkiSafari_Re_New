using System;
using UnityEngine;

public class FXSurfaceEffect : MonoBehaviour
{
	public SurfaceType surfaceType;

	public ParticleSystem particles;

	public ParticleSystem trailParticles;

	public float trailParticlesDisableDelay = 1f;

	public FXTrail trail;

	public float trailMaxStartWidth = 2f;

	public AudioSourceSettings audioSourceSettings;

	public Sound moveSound;

	public Sound jumpSound;

	public Sound landSound;

	public float minTimeBetweenLands = 0.5f;

	private AudioSource m_audio;

	private bool m_active = true;

	private float m_lastLandTime;

	public bool Active
	{
		get
		{
			return m_active;
		}
		set
		{
			if (value == m_active)
			{
				return;
			}
			m_active = value;
			if ((bool)particles)
			{
				particles.enableEmission = value;
			}
			if ((bool)trailParticles)
			{
				CancelInvoke("DisableTrailParticles");
				if (value)
				{
					trailParticles.enableEmission = true;
				}
				else
				{
					Invoke("DisableTrailParticles", trailParticlesDisableDelay);
				}
			}
			if ((bool)trail)
			{
				trail.gameObject.SetActive(value);
			}
			if ((bool)m_audio && (bool)moveSound && (bool)moveSound.clip && SoundManager.Instance.SFXEnabled)
			{
				if (value)
				{
					m_audio.clip = moveSound.clip;
					m_audio.loop = true;
					m_audio.Play();
				}
				else
				{
					m_audio.Stop();
				}
			}
		}
	}

	public float TrailScale
	{
		set
		{
			if ((bool)trail)
			{
				trail.StartWidth = trailMaxStartWidth * value;
			}
			if ((bool)m_audio)
			{
				m_audio.volume = value;
			}
		}
	}

	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			base.transform.position = value;
		}
	}

	public void Jump()
	{
		if ((bool)jumpSound && (bool)jumpSound.clip && SoundManager.Instance.SFXEnabled)
		{
			m_audio.loop = false;
			m_audio.PlayOneShot(jumpSound.clip);
			m_audio.loop = true;
		}
	}

	public void Land(float speedTowardsContact)
	{
		if ((bool)landSound && (bool)landSound.clip && SoundManager.Instance.SFXEnabled && Time.time - m_lastLandTime > minTimeBetweenLands)
		{
			m_audio.loop = false;
			m_audio.PlayOneShot(landSound.clip);
			m_audio.loop = true;
		}
	}

	private void OnSFXChanged(bool sfxEnabled)
	{
		if (sfxEnabled)
		{
			if ((bool)m_audio && (bool)moveSound && (bool)moveSound.clip && m_active)
			{
				m_audio.clip = moveSound.clip;
				m_audio.loop = true;
				m_audio.Play();
			}
		}
		else
		{
			m_audio.Stop();
		}
	}

	private void DisableTrailParticles()
	{
		trailParticles.enableEmission = false;
	}

	private void Awake()
	{
		m_audio = audioSourceSettings.CreateSource(base.gameObject);
	}

	private void OnEnable()
	{
		m_active = true;
		Active = false;
		m_lastLandTime = 0f;
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Combine(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}

	private void OnDisable()
	{
		Active = false;
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Remove(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}
}
