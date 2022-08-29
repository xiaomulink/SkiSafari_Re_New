using UnityEngine;

public class AudioSourceSettings : ScriptableObject
{
	public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

	public float minDistance = 20f;

	public float maxDistance = 100f;

	public AudioSource CreateSource(GameObject gameObject)
	{
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = false;
		audioSource.rolloffMode = rolloffMode;
		audioSource.minDistance = minDistance;
		audioSource.maxDistance = maxDistance;
		audioSource.spatialBlend = 1f;
		return audioSource;
	}
}
