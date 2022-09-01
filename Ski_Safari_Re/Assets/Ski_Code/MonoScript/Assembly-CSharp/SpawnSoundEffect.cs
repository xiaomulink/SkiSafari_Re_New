using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Audio/SpawnSoundEffect")]
public class SpawnSoundEffect : MonoBehaviour
{
	private void OnSFXChanged(bool sfxEnabled)
	{
		if (!sfxEnabled)
		{
			GetComponent<AudioSource>().Stop();
		}
	}

	private void OnEnable()
	{
		if ((bool)SoundManager.Instance && SoundManager.Instance.SFXEnabled && base.transform.position.x > Camera.main.transform.position.x)
		{
			GetComponent<AudioSource>().Play();
		}
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Combine(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}

	private void OnDisable()
	{
		SoundManager.OnSFXChanged = (SoundManager.OnSFXChangedDelegate)Delegate.Remove(SoundManager.OnSFXChanged, new SoundManager.OnSFXChangedDelegate(OnSFXChanged));
	}
}
