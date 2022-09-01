using UnityEngine;

public class GUIHighScore : MonoBehaviour
{
	public int maxPlayerNameLength = 10;

	public TextMesh nameText;

	public GameObject namePrompt;

	public GUIDropShadowText scoreText;

	public GUITransitionAnimator transitionAnimator;

	public float blinkDuration = 0.75f;

	public float resumeButtonDelay = 2f;

	private Profile.LeaderboardEntry m_highScoreEntry;

	private float m_blinkTimer;

	private bool m_blinkOn;

	public Profile.LeaderboardEntry HighScoreEntry
	{
		get
		{
			return m_highScoreEntry;
		}
		set
		{
			m_highScoreEntry = value;
			nameText.text = m_highScoreEntry.name;
			scoreText.Text = Mathf.FloorToInt(m_highScoreEntry.score).ToString("N0");
			if (string.IsNullOrEmpty(m_highScoreEntry.name))
			{
				namePrompt.SetActive(true);
			}
			else
			{
				namePrompt.SetActive(false);
			}
		}
	}

	public string PlayerName
	{
		get
		{
			return m_highScoreEntry.name;
		}
		set
		{
			m_highScoreEntry.name = value;
			nameText.text = value;
			if (string.IsNullOrEmpty(m_highScoreEntry.name))
			{
				namePrompt.SetActive(true);
			}
			else
			{
				namePrompt.SetActive(false);
			}
		}
	}

	public float Score
	{
		get
		{
			return m_highScoreEntry.score;
		}
	}

	public void Submit()
	{
		PlayerPrefs.SetString("LastPlayerName", m_highScoreEntry.name);
		GameState.Save();
		transitionAnimator.Hide();
	}

	private void OnEnable()
	{
		m_blinkTimer = blinkDuration;
	}

	private void Update()
	{
		m_blinkTimer -= Time.deltaTime;
		if (m_blinkTimer <= 0f)
		{
			m_blinkTimer = blinkDuration;
			m_blinkOn = !m_blinkOn;
			if (m_highScoreEntry.name.Length < maxPlayerNameLength && m_blinkOn)
			{
				nameText.text = m_highScoreEntry.name + "_";
			}
			else
			{
				nameText.text = m_highScoreEntry.name;
			}
		}
	}
}
