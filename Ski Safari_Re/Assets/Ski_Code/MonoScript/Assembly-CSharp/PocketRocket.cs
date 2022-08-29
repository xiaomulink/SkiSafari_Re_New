using UnityEngine;

public class PocketRocket : MonoBehaviour
{
	public float launchDelay = 0.5f;

	public Sound unfoldSound;

	public PlayerSkier playerSkier;

	public GameObject rocketScaleNode;

	public GameObject effects;

	public float speed = 80f;

	private float m_launchTimer;

	private bool m_playedUnfoldSound;

	private void OnEnable()
	{
		m_launchTimer = launchDelay;
		m_playedUnfoldSound = false;
		Go.killAllTweensWithTarget(rocketScaleNode.transform);
		rocketScaleNode.transform.localScale = Vector3.zero;
		Go.to(rocketScaleNode.transform, launchDelay, new GoTweenConfig().scale(Vector3.one));
		playerSkier.maxSpeedForAirPush = 10f;
		playerSkier.maxSpeedForPush = 10f;
		effects.SetActive(false);
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (!m_playedUnfoldSound)
		{
			if (SoundManager.Instance.SFXEnabled)
			{
				GetComponent<AudioSource>().PlayOneShot(unfoldSound.clip);
			}
			m_playedUnfoldSound = true;
		}
		if (m_launchTimer > 0f)
		{
			m_launchTimer -= Time.deltaTime;
			if (m_launchTimer <= 0f)
			{
				Go.killAllTweensWithTarget(rocketScaleNode.transform);
				rocketScaleNode.transform.localScale = Vector3.one;
				playerSkier.Collider.velocity = playerSkier.Collider.velocity.normalized * speed;
				playerSkier.maxSpeedForAirPush = speed;
				playerSkier.maxSpeedForPush = speed;
				effects.SetActiveRecursively(true);
			}
		}
	}
}
