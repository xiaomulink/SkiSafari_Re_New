using UnityEngine;
using UnityEngine.UI;

public class GUILives : MonoBehaviour
{
	public GameObject lifePrefab;

	public Text lifeCountText;

	public int maxLifeIconCount = 3;

	public float separation = 10f;

	public Camera guiCamera;

	private GameObject[] m_lifeInstances;

	private int m_lastNumberOfLives;

	private void Start()
	{
		if (SkiGameManager.Instance.mode == SkiGameManager.Mode.Avalanche)
		{
			base.gameObject.SetActive(false);
			return;
		}
		Vector3 position = guiCamera.ViewportToWorldPoint(base.transform.position);
		m_lifeInstances = new GameObject[maxLifeIconCount];
		for (int i = 0; i < maxLifeIconCount; i++)
		{
			m_lifeInstances[i] = Object.Instantiate(lifePrefab, position, Quaternion.identity) as GameObject;
			position.x -= separation;
		}
		lifeCountText.gameObject.SetActive(false);
	}

	private void Update()
	{
		int lives = SkiGameManager.Instance.lives;
		if (lives == m_lastNumberOfLives)
		{
			return;
		}
		if (lives <= maxLifeIconCount)
		{
			for (int i = 0; i < lives; i++)
			{
				m_lifeInstances[i].SetActive(true);
			}
			for (int j = lives; j < m_lifeInstances.Length; j++)
			{
				m_lifeInstances[j].SetActive(false);
			}
			lifeCountText.gameObject.SetActive(false);
		}
		else
		{
			m_lifeInstances[0].SetActive(true);
			for (int k = 1; k < m_lifeInstances.Length; k++)
			{
				m_lifeInstances[k].SetActive(false);
			}
			lifeCountText.gameObject.SetActive(true);
			lifeCountText.text = lives.ToString();
		}
		m_lastNumberOfLives = lives;
	}
}
