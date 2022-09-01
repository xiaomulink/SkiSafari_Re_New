using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PreloadedAudioSource : MonoBehaviour
{
	private float m_volume;

	public void Play()
	{
		GetComponent<AudioSource>().time = 0f;
		GetComponent<AudioSource>().volume = m_volume;
		GetComponent<AudioSource>().Play();
	}

	private void Awake()
	{
		m_volume = GetComponent<AudioSource>().volume;
		GetComponent<AudioSource>().volume = 0f;
	}
}
