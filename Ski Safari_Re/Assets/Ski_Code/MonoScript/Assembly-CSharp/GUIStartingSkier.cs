using UnityEngine;

public class GUIStartingSkier : MonoBehaviour
{
	public GameObject activeSprite;

	public GameObject inactiveSprite;

	public GameObject newSprite;

	private bool m_skierActive;

	public StartingSkier StartingSkier { get; set; }

	public void SetActive(bool state)
	{
		m_skierActive = state;
		activeSprite.SetActive(m_skierActive);
		inactiveSprite.SetActive(!m_skierActive);
	}

	public void SetNew(bool state)
	{
		newSprite.SetActive(state);
	}

	protected void Start()
	{
		activeSprite.SetActive(m_skierActive);
		inactiveSprite.SetActive(!m_skierActive);
	}
}
