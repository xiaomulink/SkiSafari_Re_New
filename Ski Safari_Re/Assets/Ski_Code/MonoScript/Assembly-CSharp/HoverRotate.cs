using System;
using UnityEngine;

public class HoverRotate : Hover
{
	public float rotateMagnitude = 30f;

	public float rotateFrequencyMin = 0.25f;

	public float rotateFrequencyMax = 0.5f;

	public Vector3 rotateAxis = Vector3.forward;

	private Quaternion m_initialLocalRotation;

	private float m_rotateOffset;

	private float m_rotateFrequency;

	protected override void Start()
	{
		m_initialLocalRotation = base.transform.localRotation;
		m_rotateFrequency = UnityEngine.Random.Range(hoverFrequencyMin, hoverFrequencyMax);
		float num = 1f / m_rotateFrequency;
		m_rotateOffset = UnityEngine.Random.Range(0f - num, num);
		base.Start();
	}

	protected override void LateUpdate()
	{
		if (rotateMagnitude > 0f)
		{
			float angle = Mathf.Sin((Time.realtimeSinceStartup + m_rotateOffset) * (float)Math.PI * m_rotateFrequency) * rotateMagnitude;
			Quaternion quaternion = Quaternion.AngleAxis(angle, rotateAxis);
			base.transform.localRotation = m_initialLocalRotation * quaternion;
		}
		base.LateUpdate();
	}
}
