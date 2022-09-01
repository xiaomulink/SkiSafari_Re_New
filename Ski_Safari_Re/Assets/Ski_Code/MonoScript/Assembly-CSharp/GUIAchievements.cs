using System.Collections.Generic;
using UnityEngine;

public class GUIAchievements : MonoBehaviour
{
	private enum State
	{
		Hidden = 0,
		Showing = 1,
		ShowingAchievementCompletion = 2,
		ShowingNewStar = 3,
		ShowingLevelUp = 4,
		ShowingLevelProgression = 5,
		AddingAchievement = 6,
		Idle = 7
	}

	public static GUIAchievements Instance;

	public Vector3 defaultScreenPos = new Vector3(0.5f, 0.5f, 9f);

	public Vector3 titleScreenPos = new Vector3(0.35f, 0.5f, 9f);

	public Transform badgeNode;

	public GUIDropShadowText levelNameText;

	public GameObject nextRank;

	public GUIStar starPrefab;

	public Transform starStartNode;

	public float starSeparation = 1.5f;

	public Transform comboLimitNode;

	public Transform startingSkierStartNode;

	public Vector3 startingSkierSeparation = new Vector3(1f, 0f, 0.05f);

	public GameObject startingSkierSelectionPrefab;

	public float checkAchievementsDelay = 2f;

	public float achievementCompletionShowDuration = 0.5f;

	public Sound achievementCompleteSound;

	public float starBlingDuration = 0.5f;

	public float levelProgressionShowDuration = 0.5f;

	public GUILevelUp guiLevelUpPrefab;

	public GUIUpsellWindow guiUpsellWindowPrefab;

	public GUITransitionAnimator transitionAnimator;

	private State m_state;

	private float m_stateTimer;

	private GUIAchievementEntry m_completingEntry;

	private GUIStar m_completedStar;

	private bool m_completingAchievements;

	private bool m_autoShow;

	private GameObject m_badge;

	private List<GUIStar> m_stars = new List<GUIStar>();

	private List<GUIStartingSkier> m_guiStartingSkiers = new List<GUIStartingSkier>();

	private GameObject m_comboLimit;

	private GameObject m_lastComboLimitPrefab;

	private GUILevelUp m_guiLevelUp;

	private GUIUpsellWindow m_guiUpsellWindow;

	public bool AutoShow
	{
		get
		{
			return m_autoShow;
		}
	}

	public bool IsShowing
	{
		get
		{
			return m_state != State.Hidden;
		}
	}

	public void UpdateAutoShow()
	{
		m_autoShow = AchievementManager.Instance.CompletedActiveAchievementCount > 0;
	}

	public void Show()
	{
		if (!base.gameObject.activeInHierarchy || m_state == State.Hidden)
		{
			if (AchievementManager.Instance.CompletedActiveAchievementCount > 0)
			{
				m_completingAchievements = true;
			}
			else
			{
				m_completingAchievements = false;
			}
			ShowAchievementListOrUpsellWindow();
			transitionAnimator.Show();
			UpdateLevelName();
			UpdateStars();
			UpdateScoreMultiplier();
			UpdateStartingSkiers();
			SetState(State.Showing);
		}
	}

	public void Hide()
	{
		if (m_state == State.Showing || m_state == State.Idle)
		{
			GameState.Save();
			transitionAnimator.Hide();
			SetState(State.Hidden);
			GUIAchievementList.Instance.Hide(true);
			if ((bool)m_guiUpsellWindow)
			{
				m_guiUpsellWindow.transitionAnimator.Hide();
			}
			if (m_autoShow && !SkiGameManager.Instance.ShowShop)
			{
				GUITutorials.Instance.UpdateAutoShow();
			}
			m_autoShow = false;
		}
	}

	private void UpdateLevelName()
	{
		levelNameText.Text = LevelManager.Instance.CurrentLevelDescriptor.name;
		if ((bool)m_badge)
		{
			Object.Destroy(m_badge);
		}
		m_badge = TransformUtils.Instantiate(LevelManager.Instance.CurrentLevelDescriptor.largeBadge, base.transform);
		m_badge.transform.localPosition = badgeNode.localPosition;
	}

	private void UpdateStars()
	{
		foreach (GUIStar star in m_stars)
		{
			Object.Destroy(star.gameObject);
		}
		m_stars.Clear();
		Vector3 localPosition = starStartNode.localPosition;
		int currentLevelPoints = LevelManager.Instance.CurrentLevelPoints;
		for (int i = 0; i < LevelManager.Instance.CurrentLevelDescriptor.pointsToComplete; i++)
		{
			GUIStar gUIStar = Object.Instantiate(starPrefab, base.transform.position, base.transform.rotation) as GUIStar;
			gUIStar.transform.parent = base.transform;
			gUIStar.transform.localPosition += localPosition;
			gUIStar.transform.localScale = Vector3.one;
			gUIStar.Active = i <= currentLevelPoints - 1;
			m_stars.Add(gUIStar);
			localPosition.x += starSeparation;
		}
		bool flag = m_stars.Count == 0;
		nextRank.SetActive(!flag);
	}

	private void UpdateScoreMultiplier()
	{
		GameObject guiScoreMultiplier = LevelManager.Instance.CurrentScoreMultiplier.guiScoreMultiplier;
		if (!(m_lastComboLimitPrefab == guiScoreMultiplier))
		{
			bool flag = false;
			if ((bool)m_comboLimit)
			{
				flag = true;
				Object.Destroy(m_comboLimit);
			}
			m_comboLimit = TransformUtils.Instantiate(guiScoreMultiplier, base.transform);
			m_comboLimit.transform.localPosition = comboLimitNode.localPosition;
			m_lastComboLimitPrefab = guiScoreMultiplier;
			if (flag)
			{
				GoTweenConfig config = new GoTweenConfig().scale(1.1f, true).setEaseType(GoEaseType.ElasticPunch);
				Go.to(m_comboLimit.transform, 0.5f, config);
			}
		}
	}

	private void UpdateStartingSkiers()
	{
		int currentLevel = LevelManager.Instance.CurrentLevel;
		foreach (GUIStartingSkier guiStartingSkier in m_guiStartingSkiers)
		{
			bool flag = guiStartingSkier.StartingSkier.requiredLevel <= currentLevel;
			guiStartingSkier.SetActive(flag);
			guiStartingSkier.SetNew(flag && guiStartingSkier.StartingSkier.IsNew);
		}
	}

	private void ShowAchievementListOrUpsellWindow()
	{
		if (!AchievementManager.Instance.IsDemo || AchievementManager.Instance.ActiveAchievements.Count > 0)
		{
			if (SkiGameManager.Instance.TitleScreenActive && !m_completingAchievements && !m_autoShow)
			{
				transitionAnimator.onScreenPos = titleScreenPos;
				GUIAchievementList.Instance.Show(GUIAchievementList.ShowPosition.TitleMiddleCenter, true);
			}
			else
			{
				transitionAnimator.onScreenPos = defaultScreenPos;
				GUIAchievementList.Instance.Show(GUIAchievementList.ShowPosition.MiddleCenter, true);
			}
			if ((bool)m_guiUpsellWindow)
			{
				Object.Destroy(m_guiUpsellWindow);
			}
		}
		else
		{
			GUIAchievementList.Instance.Hide(true);
			if (!m_guiUpsellWindow)
			{
				m_guiUpsellWindow = Object.Instantiate(guiUpsellWindowPrefab);
			}
			else
			{
				m_guiUpsellWindow.transitionAnimator.Show();
			}
		}
	}

	private void UpdateLevelInfo()
	{
		UpdateLevelName();
		UpdateStars();
		UpdateScoreMultiplier();
		UpdateStartingSkiers();
		StuntManager.Instance.Reset();
	}

	private void CheckLevelUp()
	{
		int currentLevel = LevelManager.Instance.CurrentLevel;
		LevelManager.Instance.Load();
		if (LevelManager.Instance.CurrentLevel != currentLevel)
		{
			int num = 100;
			int num2 = Mathf.FloorToInt((float)GameState.CoinCount / (float)num) * num;
			AnalyticsManager.Instance.SendEvent("levelup", "rank", LevelManager.Instance.CurrentLevel.ToString(), "coins", num2.ToString());
			m_guiLevelUp = Object.Instantiate(guiLevelUpPrefab);
			if (SkiGameManager.Instance.TitleScreenActive)
			{
				StartingSkier.Instance.UpdateSign();
				Slope.Instance.UpdateSign();
			}
			transitionAnimator.disallowDeactivate = true;
			transitionAnimator.Hide();
			GUIAchievementList.Instance.Hide(false);
			SetState(State.ShowingLevelUp);
		}
		else
		{
			SetState(State.ShowingLevelProgression);
		}
	}

	private void CompleteAchievement(GUIAchievementEntry entry)
	{
		foreach (GUIStar star in m_stars)
		{
			if (!star.Active)
			{
				m_completingEntry = entry;
				m_completedStar = star;
				SoundManager.Instance.PlaySound(achievementCompleteSound);
				Go.to(m_completingEntry.starSprite.transform, achievementCompletionShowDuration, new GoTweenConfig().position(star.transform.position));
				SetState(State.ShowingAchievementCompletion);
				break;
			}
		}
	}

	private void CheckNewAchievements()
	{
		if (GUIAchievementList.Instance.CheckNewAchievements())
		{
			SetState(State.AddingAchievement);
			m_stateTimer = 0f;
		}
		else
		{
			ShowAchievementListOrUpsellWindow();
			SetState(State.Idle);
		}
	}

	private void SetState(State state)
	{
		if (state == m_state)
		{
			return;
		}
		m_state = state;
		m_stateTimer = 0f;
		State state2 = m_state;
		if (state2 == State.Idle)
		{
			if (SkiGameManager.Instance.TitleScreenActive && m_completingAchievements && !m_autoShow)
			{
				transitionAnimator.MoveTo(titleScreenPos);
				GUIAchievementList.Instance.Show(GUIAchievementList.ShowPosition.TitleMiddleCenter, true);
			}
			if (!SkiGameManager.Instance.TitleScreenActive || m_autoShow)
			{
				SkiGameManager.Instance.continueButton.Show(Hide);
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		ItemSet itemSet = ItemManager.Instance.GetItemSet("starting_skier");
		Vector3 localPosition = startingSkierStartNode.localPosition;
		for (int i = 1; i < itemSet.items.Length; i++)
		{
			StartingSkier startingSkier = itemSet.items[i] as StartingSkier;
			if ((bool)startingSkier)
			{
				GUIStartingSkier gUIStartingSkier = Object.Instantiate(startingSkier.guiStartingSkier, base.transform.position, base.transform.rotation) as GUIStartingSkier;
				gUIStartingSkier.transform.parent = base.transform;
				gUIStartingSkier.transform.localPosition += localPosition;
				gUIStartingSkier.transform.localScale = Vector3.one;
				gUIStartingSkier.StartingSkier = startingSkier;
				m_guiStartingSkiers.Add(gUIStartingSkier);
				localPosition += startingSkierSeparation;
			}
		}
	}

	private void OnEnable()
	{
		m_state = State.Hidden;
	}

	private void Update()
	{
		m_stateTimer += Time.deltaTime;
		switch (m_state)
		{
		case State.Showing:
			if (m_stateTimer >= checkAchievementsDelay)
			{
				GUIAchievementEntry nextCompletedAchievement3 = GUIAchievementList.Instance.GetNextCompletedAchievement();
				if ((bool)nextCompletedAchievement3)
				{
					CompleteAchievement(nextCompletedAchievement3);
				}
				else
				{
					CheckNewAchievements();
				}
			}
			break;
		case State.ShowingAchievementCompletion:
			if (m_stateTimer >= achievementCompletionShowDuration)
			{
				AchievementManager.Instance.RemoveCompletedAchievement(m_completingEntry.Achievement);
				GUIAchievementList.Instance.RemoveAchievementEntry(m_completingEntry);
				m_completingEntry = null;
				m_completedStar.Active = true;
				m_completedStar.ShowActivateEffect();
				SetState(State.ShowingNewStar);
			}
			break;
		case State.ShowingNewStar:
			if (!(m_stateTimer >= starBlingDuration))
			{
				break;
			}
			LevelManager.Instance.CurrentLevelPoints++;
			if (LevelManager.Instance.CurrentLevelPoints < LevelManager.Instance.CurrentLevelDescriptor.pointsToComplete)
			{
				GUIAchievementEntry nextCompletedAchievement2 = GUIAchievementList.Instance.GetNextCompletedAchievement();
				if ((bool)nextCompletedAchievement2)
				{
					CompleteAchievement(nextCompletedAchievement2);
					break;
				}
			}
			CheckLevelUp();
			break;
		case State.ShowingLevelUp:
			if (!m_guiLevelUp)
			{
				UpdateLevelInfo();
				transitionAnimator.disallowDeactivate = false;
				transitionAnimator.onScreenPos = defaultScreenPos;
				transitionAnimator.Show();
				GUIAchievementList.Instance.Show(GUIAchievementList.ShowPosition.MiddleCenter, true);
				SetState(State.ShowingLevelProgression);
			}
			break;
		case State.ShowingLevelProgression:
			if (!(m_stateTimer > levelProgressionShowDuration))
			{
				break;
			}
			if (LevelManager.Instance.CurrentLevelDescriptor.pointsToComplete > 0)
			{
				GUIAchievementEntry nextCompletedAchievement = GUIAchievementList.Instance.GetNextCompletedAchievement();
				if ((bool)nextCompletedAchievement)
				{
					CompleteAchievement(nextCompletedAchievement);
					break;
				}
			}
			AchievementManager.Instance.PopulateActiveAchievements();
			CheckNewAchievements();
			break;
		case State.AddingAchievement:
			if (m_stateTimer >= GUIAchievementList.Instance.entryMoveDuration)
			{
				CheckNewAchievements();
			}
			break;
		}
	}
}
