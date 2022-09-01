using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Serializable]
	public class SetInfo
	{
		public float maxDistance = 1000f;

		public GameObject setPrefab;
	}

	public static GameManager Instance;

	public SetInfo[] sets;

	public Text distanceText;

	public GameObject gameOverPrefab;

	private Vector3 m_startPos;

	private bool m_finished;

	private GameObject m_currentSet;

	private GameObject m_lastSet;

	public void Restart()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

	public void Finish()
	{
		TransformUtils.Instantiate(gameOverPrefab, base.transform);
		m_finished = true;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		m_startPos = Player.Instance.transform.position;
	}

	private void Update()
	{
		if ((bool)Player.Instance && !m_finished)
		{
			UpdateInput();
			float num = Mathf.FloorToInt(Player.Instance.transform.position.x - m_startPos.x);
			distanceText.text = num.ToString();
		}
	}

	private void UpdateInput()
	{
		if (Input.GetMouseButton(0))
		{
			Player.Instance.LiftInput = ((!(Input.mousePosition.x < (float)(Screen.width / 2))) ? (-1f) : 1f);
		}
		else
		{
			Player.Instance.LiftInput = 0f;
		}
	}
}
