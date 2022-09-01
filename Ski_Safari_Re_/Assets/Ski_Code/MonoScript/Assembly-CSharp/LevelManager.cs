using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[Serializable]
	public class LevelDescriptor
	{
		public string name;

		public int pointsToComplete = 3;

		public GameObject smallBadge;

		public GameObject largeBadge;

		public Sound levelUpSound;
	}

	[Serializable]
	public class ScoreMultiplier
	{
		public int requiredLevel = 5;

		public int maxCombo;

		public GameObject guiScoreMultiplier;

		public GUIReward guiReward;
	}

	[Serializable]
	public class CoinBonus
	{
		public int requiredLevel = 4;

		public int coins = 100;

		public GUIReward guiReward;
	}

	public static LevelManager Instance;

	public LevelDescriptor[] levelDescriptors;

	public ScoreMultiplier[] scoreMultipliers;

	public CoinBonus[] coinBonuses;

	public static Action OnLoaded;

	private int m_currentLevel;

	private ScoreMultiplier m_currentScoreMultiplier;

	private int m_currentLevelPoints;

	public int CurrentLevel
	{
		get
		{
			return m_currentLevel;
		}
	}

	public int CurrentLevelPoints
	{
		get
		{
			return m_currentLevelPoints;
		}
		set
		{
			m_currentLevelPoints = value;
		}
	}

	public LevelDescriptor CurrentLevelDescriptor
	{
		get
		{
			return levelDescriptors[m_currentLevel - 1];
		}
	}

	public ScoreMultiplier CurrentScoreMultiplier
	{
		get
		{
			return m_currentScoreMultiplier;
		}
	}

	public void Load()
	{
		m_currentLevel = 1;
		m_currentLevelPoints = AchievementManager.Instance.CompletedAchievementCount;
		while (m_currentLevel < levelDescriptors.Length && m_currentLevelPoints >= levelDescriptors[m_currentLevel - 1].pointsToComplete)
		{
			m_currentLevelPoints -= levelDescriptors[m_currentLevel - 1].pointsToComplete;
			m_currentLevel++;
		}
		GameState.SetInt("current_level", m_currentLevel);//±£´æµÈ¼¶

        ScoreMultiplier[] array = scoreMultipliers;
		foreach (ScoreMultiplier scoreMultiplier in array)
		{
			if (m_currentLevel >= scoreMultiplier.requiredLevel)
			{
				m_currentScoreMultiplier = scoreMultiplier;
			}
		}
		if (OnLoaded != null)
		{
			OnLoaded();
		}
	}

	private void Awake()
	{
		Instance = this;
	}
}
