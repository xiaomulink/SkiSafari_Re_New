using UnityEngine;

public class ShakeEmitter : MonoBehaviour
{
	public float accelerationThreshold = 2f;

	public float emitStopDelay = 2f;

	public bool startActive;

	private ParticleSystem m_emitter;

	private float m_emitTimer;

	private void Awake()
	{
		m_emitter = GetComponent<ParticleSystem>();
	}

	private void Start()
	{
		if (startActive)
		{
			m_emitTimer = emitStopDelay;
			m_emitter.enableEmission = true;
		}
	}

	private void Update()
	{
		float magnitude = Input.acceleration.magnitude;
		if (magnitude > accelerationThreshold)
		{
			m_emitTimer = emitStopDelay;
			m_emitter.enableEmission = true;
		}
		else if (m_emitTimer > 0f)
		{
			m_emitTimer -= Time.deltaTime;
			if (m_emitTimer <= 0f)
			{
				m_emitter.enableEmission = false;
			}
		}
	}
}
