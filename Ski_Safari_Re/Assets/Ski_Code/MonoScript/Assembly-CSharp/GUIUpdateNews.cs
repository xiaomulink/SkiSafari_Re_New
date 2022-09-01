using UnityEngine;

public class GUIUpdateNews : MonoBehaviour
{
	public string version;

	protected float m_initialDelay = 3f;

	private void Awake()
	{
		if (!SkiGameManager.Instance.Initialising)
		{
			m_initialDelay = 1f;
		}
	}
}
