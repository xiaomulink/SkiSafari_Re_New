using System;
using UnityEngine;

public class Hover : MonoBehaviour
{
	public float hoverMagnitude = 0.2f;

	public float hoverFrequencyMin = 0.25f;

	public float hoverFrequencyMax = 0.5f;

	private Vector3 m_initialLocalPosition;

	private float m_hoverOffset1;

	private float m_hoverFrequency1;

	private float m_hoverOffset2;

	private float m_hoverFrequency2;

	protected virtual void Start()
	{
		m_initialLocalPosition = base.transform.localPosition;
		m_hoverFrequency1 = UnityEngine.Random.Range(hoverFrequencyMin, hoverFrequencyMax);
		float num = 1f / m_hoverFrequency1;
		m_hoverOffset1 = UnityEngine.Random.Range(0f - num, num);
		m_hoverFrequency2 = UnityEngine.Random.Range(hoverFrequencyMin, hoverFrequencyMax);
		float num2 = 1f / m_hoverFrequency2;
		m_hoverOffset2 = UnityEngine.Random.Range(0f - num2, num2);
	}

	protected virtual void LateUpdate()
	{
		if (hoverMagnitude > 0f)
		{
			Vector3 vector = new Vector3(Mathf.Sin((Time.realtimeSinceStartup + m_hoverOffset1) * (float)Math.PI * m_hoverFrequency1) * hoverMagnitude, Mathf.Sin((Time.realtimeSinceStartup + m_hoverOffset2) * (float)Math.PI * m_hoverFrequency2) * hoverMagnitude, 0f);
			base.transform.localPosition = m_initialLocalPosition + vector;
		}
	}
}
