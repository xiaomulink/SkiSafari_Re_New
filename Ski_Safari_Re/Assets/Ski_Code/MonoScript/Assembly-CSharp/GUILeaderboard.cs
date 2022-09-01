using System;
using System.Collections.Generic;
using UnityEngine;

public class GUILeaderboard : MonoBehaviour
{
	public static GUILeaderboard Instance;

	public GUILeaderboardScore scorePrefab;

	public GUILeaderboardScore recentScorePrefab;

	public GUIDropShadowText titleText;

	public Renderer titleIcon;

	public int maxScores = 5;

	public Vector3 scoreStartPos;

	public float scoreSeparation = 1.5f;

	public GUITransitionAnimator transitionAnimator;

	public GameObject lastScoreWindow;

	public GUIDropShadowText lastScoreText;

	public float badgeScale = 0.5f;

	public GUIButton leftButton;

	public GUIButton rightButton;

	public GameObject noScoresObject;

	public GameObject onlineLeaderboardPanel;

	public string defaultName = "noname";

	private List<GUILeaderboardScore> m_scores = new List<GUILeaderboardScore>();

	private Profile.LeaderboardEntry m_recentEntry;

	private bool m_showing;

	private ItemSet m_slopes;

	private int m_currentSlopeIndex;

	public Profile.LeaderboardEntry RecentEntry
	{
		get
		{
			return m_recentEntry;
		}
		set
		{
			m_recentEntry = value;
		}
	}

	public Slope SelectedSlope
	{
		get
		{
			return m_slopes.items[m_currentSlopeIndex] as Slope;
		}
	}

	public bool Showing
	{
		get
		{
			return transitionAnimator.IsShowing;
		}
	}

	public void Show()
	{
		transitionAnimator.Show();
	}

	public void Hide()
	{
		transitionAnimator.Hide();
	}

	private void SetupScores()
	{
		Slope slope = m_slopes.items[m_currentSlopeIndex] as Slope;
		titleText.Text = slope.displayName;
		titleText.TextScale = slope.leaderboardTextScale;
		titleIcon.material.mainTexture = AssetManager.LoadAsset<Texture>(slope.iconTextureName);
		bool active = false;
		for (int num = m_currentSlopeIndex - 1; num >= 0; num--)
		{
			if (m_slopes.items[num].Unlocked)
			{
				active = true;
				break;
			}
		}
		bool active2 = false;
		for (int i = m_currentSlopeIndex + 1; i < m_slopes.items.Length; i++)
		{
			if (m_slopes.items[i].Unlocked)
			{
				active2 = true;
				break;
			}
		}
		leftButton.gameObject.SetActive(active);
		rightButton.gameObject.SetActive(active2);
		foreach (GUILeaderboardScore score in m_scores)
		{
			UnityEngine.Object.Destroy(score.gameObject);
		}
		m_scores.Clear();
		List<Profile.LeaderboardEntry> highScoreEntries = GameState.GetHighScoreEntries(slope.leaderboardId);
		if (highScoreEntries != null)
		{
			Vector3 vector = scoreStartPos;
			int num2 = 1;
			foreach (Profile.LeaderboardEntry item in highScoreEntries)
			{
				GUILeaderboardScore gUILeaderboardScore = UnityEngine.Object.Instantiate((m_recentEntry != item) ? scorePrefab : recentScorePrefab, base.transform.position, base.transform.rotation) as GUILeaderboardScore;
				gUILeaderboardScore.transform.parent = base.transform;
				gUILeaderboardScore.transform.localPosition += vector;
				gUILeaderboardScore.transform.localScale = Vector3.one;
				gUILeaderboardScore.scoreText.Text = Mathf.FloorToInt(item.score).ToString("N0");
				gUILeaderboardScore.nameText.Text = ((!string.IsNullOrEmpty(item.name)) ? item.name : defaultName);
				LevelManager.LevelDescriptor levelDescriptor = LevelManager.Instance.levelDescriptors[Mathf.Clamp(item.rank - 1, 0, LevelManager.Instance.levelDescriptors.Length - 1)];
				GameObject prefab = ((!levelDescriptor.smallBadge) ? levelDescriptor.largeBadge : levelDescriptor.smallBadge);
				GameObject gameObject = TransformUtils.Instantiate(prefab, gUILeaderboardScore.badgeNode);
				gameObject.transform.localScale *= badgeScale;
				m_scores.Add(gUILeaderboardScore);
				vector.y -= scoreSeparation;
				if (++num2 > maxScores)
				{
					break;
				}
			}
		}
		noScoresObject.SetActive(m_scores.Count == 0);
	}

	private void SelectPreviousSlope()
	{
		for (int num = m_currentSlopeIndex - 1; num >= 0; num--)
		{
			if (m_slopes.items[num].Unlocked)
			{
				m_currentSlopeIndex = num;
				SetupScores();
				break;
			}
		}
	}

	private void SelectNextSlope()
	{
		for (int i = m_currentSlopeIndex + 1; i < m_slopes.items.Length; i++)
		{
			if (m_slopes.items[i].Unlocked)
			{
				m_currentSlopeIndex = i;
				SetupScores();
				break;
			}
		}
	}

	private void Deactivate()
	{
		base.gameObject.SetActive(false);
	}

	private void Awake()
	{
		Instance = this;
		GUIButton gUIButton = leftButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(SelectPreviousSlope));
		GUIButton gUIButton2 = rightButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(SelectNextSlope));
	}

	private void Start()
	{
		m_slopes = ItemManager.Instance.GetItemSet("slope");
	}

	private void OnEnable()
	{
		if (m_slopes != null)
		{
			m_currentSlopeIndex = ItemManager.Instance.GetItemSet("slope").CurrentIndex;
			SetupScores();
			if (m_recentEntry == null && !SkiGameManager.Instance.TitleScreenActive && !SkiGameManager.Instance.ShowShop)
			{
				lastScoreText.Text = SkiGameManager.Instance.CurrentScore.ToString("N0");
			}
			else
			{
				lastScoreWindow.SetActive(false);
			}
			if (!SocialManager.Instance.IsAuthenticated)
			{
				onlineLeaderboardPanel.SetActive(false);
			}
		}
	}
}
