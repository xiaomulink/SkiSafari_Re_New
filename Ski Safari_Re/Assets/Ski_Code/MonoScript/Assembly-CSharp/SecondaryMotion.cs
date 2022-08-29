using UnityEngine;

public class SecondaryMotion : MonoBehaviour
{
	public float movementThreshold = 0.25f;

	public float rotationLimit = -1f;

	public float rotationFilter = 5f;

	private Vector3 m_lastPosition;

	private Quaternion m_lastRotation;

	private Quaternion m_targetRotation;

	private bool m_frozen;

	public void Freeze()
	{
		m_frozen = true;
	}

	private void OnEnable()
	{
		m_lastPosition = base.transform.position;
		m_lastRotation = (m_targetRotation = Quaternion.identity);
		m_frozen = false;
	}

	private void LateUpdate()
	{
		if (m_frozen)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = position - m_lastPosition;
		float magnitude = vector.magnitude;
		if (magnitude >= movementThreshold)
		{
			Vector3 lhs = vector / (0f - magnitude);
			Vector3 normalized = Vector3.Cross(lhs, Vector3.forward).normalized;
			m_targetRotation = Quaternion.LookRotation(Vector3.forward, normalized);
			m_lastPosition = position;
		}
		if (rotationLimit > 0f)
		{
			float z = base.transform.parent.eulerAngles.z;
			float num = m_targetRotation.eulerAngles.z - z;
			if (num > 180f)
			{
				num -= 360f;
			}
			if (num > rotationLimit)
			{
				m_targetRotation = Quaternion.Euler(0f, 0f, z + rotationLimit);
			}
			else if (num < 0f - rotationLimit)
			{
				m_targetRotation = Quaternion.Euler(0f, 0f, z - rotationLimit);
			}
		}
		m_lastRotation = Quaternion.Lerp(m_lastRotation, m_targetRotation, rotationFilter * Time.deltaTime);
		base.transform.rotation = m_lastRotation;
	}
}
