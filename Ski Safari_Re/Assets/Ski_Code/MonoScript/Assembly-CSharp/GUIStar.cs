using UnityEngine;

public class GUIStar : MonoBehaviour
{
	public GameObject activeSprite;

	public GameObject inactiveSprite;

	public GameObject activateEffectPrefab;

	[SerializeField]
	private bool m_active;

	public bool Active
	{
		get
		{
			return m_active;
		}
		set
		{
			m_active = value;
			activeSprite.SetActive(m_active);
			inactiveSprite.SetActive(!m_active);
		}
	}

	public void ShowActivateEffect()
	{
		TransformUtils.Instantiate(activateEffectPrefab, base.transform);
	}

	public void Start()
	{
		activeSprite.SetActive(m_active);
		inactiveSprite.SetActive(!m_active);
	}
}
