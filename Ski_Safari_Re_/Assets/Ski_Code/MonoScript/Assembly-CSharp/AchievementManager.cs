using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
	public static AchievementManager Instance;

	public Achievement[] achievements;

	public int maxActiveAchievements = 3;

	public int maxActiveItemAchievements = 1;

	public int maxDemoAchievements = 12;

	public ParticleSystem incrementEffectPrefab;

	public float incrementEffectDuration = 0.75f;

	public GUIAchievementList guiAchievementListPrefab;

	public GUIAchievementComplete guiAchievementCompletePrefab;

	public bool isDemo;

	private List<Achievement> m_activeAchievements = new List<Achievement>();

	private GUIAchievementList m_guiAchievementList;

	private GUIAchievementComplete m_guiAchievementComplete;

	private ParticleSystem m_incrementEffect;

	private bool m_showAchievementList;

	private int m_completedAchievementCount;

	private AchievementFlags m_flags;

	public bool ShowAchievementList
	{
		get
		{
			return m_showAchievementList;
		}
		set
		{
			m_showAchievementList = value;
			if (value)
			{
				if (SkiGameManager.Instance.Paused)
				{
					m_guiAchievementList.Show(GUIAchievementList.ShowPosition.MiddleLeft);
				}
				else
				{
					m_guiAchievementList.Show(GUIAchievementList.ShowPosition.BottomLeft);
				}
			}
			else
			{
				m_guiAchievementList.Hide(true);
			}
		}
	}

	public bool IsDemo
	{
		get
		{
			return isDemo;
		}
	}

	public bool HasPendingDemoStateChange
	{
		get
		{
            try
            {
                return isDemo != !LicenseManager.Instance.AllowAccess();
            }
            catch
            {
                return true;
            }
		}
	}

	public AchievementFlags AchievementFlags
	{
		get
		{
			return m_flags;
		}
	}

	public List<Achievement> ActiveAchievements
	{
		get
		{
			return m_activeAchievements;
		}
	}

	public int CompletedActiveAchievementCount
	{
		get
		{
			int num = 0;
			foreach (Achievement activeAchievement in m_activeAchievements)
			{
				if (activeAchievement.IsComplete)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int CompletedAchievementCount
	{
		get
		{
			return m_completedAchievementCount;
		}
	}

	public void OnComplete(Achievement achievement)
	{
		if (!SkiGameManager.Instance.TitleScreenActive && !SkiGameManager.Instance.ShowShop)
		{
			m_guiAchievementList.Hide(true);
			m_guiAchievementComplete.Show(achievement);
		}
		AnalyticsManager.Instance.SendEvent("achievement_complete", "name", achievement.name);
	}

	public bool IsAchievementActive(Achievement achievement)
	{
		foreach (Achievement activeAchievement in m_activeAchievements)
		{
			if (activeAchievement.name == achievement.name)
			{
				return true;
			}
		}
		return false;
	}

	public void OnIncrement(Achievement achievement)
	{
		if ((bool)Player.Instance)
		{
			m_incrementEffect.Clear();
			m_incrementEffect.transform.position = Player.Instance.transform.position;
			m_incrementEffect.enableEmission = true;
			CancelInvoke("DisableIncrementEffect");
			Invoke("DisableIncrementEffect", incrementEffectDuration);
		}
	}

    private void DisableIncrementEffect()
    { 
        m_incrementEffect.enableEmission = false;
	}

	private void CreateActiveAchievements(Achievement[] achievementList, Item item, int count, int maxCount, bool ignoreRequirements)
	{
		if (achievementList.Length == 0 || m_activeAchievements.Count >= maxCount)
		{
			return;
		}
		int num = count;
		foreach (Achievement achievement in achievementList)
		{
			if (achievement.IsComplete || (!ignoreRequirements && !achievement.HasBasicRequirements))
			{
				continue;
			}
			if (!IsAchievementActive(achievement))
			{
				Achievement achievement2 = TransformUtils.Instantiate(achievement, base.transform);
				achievement2.name = achievement.name;
				achievement2.Manager = this;
				achievement2.Load();
				if (!achievement.HasBasicRequirements)
				{
					achievement2.enabled = false;
				}
				m_activeAchievements.Add(achievement2);
				if (m_activeAchievements.Count >= maxCount)
				{
					break;
				}
			}
			if (--num <= 0)
			{
				break;
			}
		}
	}

	private void CreateActiveAchievementsForItemsExcludingCurrent(ItemSet itemSet, int maxCount)
	{
		Item[] items = itemSet.items;
		foreach (Item item in items)
		{
			if (item != itemSet.CurrentItem)
			{
				CreateActiveAchievements(item.achievements, item, maxActiveAchievements, maxCount, true);
			}
		}
	}

	public void ReloadActiveAchievements()
	{
		foreach (Achievement activeAchievement in m_activeAchievements)
		{
			activeAchievement.Load();
		}
	}

	public void RemoveCompletedAchievement(Achievement achievement)
	{
		achievement.IsDone = true;
		m_activeAchievements.Remove(achievement);
		Object.Destroy(achievement.gameObject);
		m_completedAchievementCount++;
	}

	public void ClearActiveAchievements()
	{
		int num = 0;
		while (num < m_activeAchievements.Count)
		{
			if (!m_activeAchievements[num].IsComplete)
			{
				Object.Destroy(m_activeAchievements[num].gameObject);
				m_activeAchievements.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public void PopulateActiveAchievements()
	{
		UpdateAchievementFlags();
		int maxCount = maxActiveAchievements;
		if (isDemo)
		{
			maxCount = Mathf.Clamp(maxDemoAchievements - m_completedAchievementCount, 0, maxActiveAchievements);
		}
		ItemSet[] itemSets = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet in itemSets)
		{
			if ((bool)itemSet.CurrentItem)
			{
				CreateActiveAchievements(itemSet.CurrentItem.achievements, itemSet.CurrentItem, maxActiveItemAchievements, maxCount, false);
			}
		}
		CreateActiveAchievements(achievements, null, maxActiveAchievements, maxCount, false);
		CreateActiveAchievements(achievements, null, maxActiveAchievements, maxCount, true);
		if (m_activeAchievements.Count >= maxActiveAchievements)
		{
			return;
		}
		ItemSet[] itemSets2 = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet2 in itemSets2)
		{
			if ((bool)itemSet2.CurrentItem)
			{
				CreateActiveAchievements(itemSet2.CurrentItem.achievements, itemSet2.CurrentItem, maxActiveAchievements, maxCount, true);
			}
		}
		ItemSet[] itemSets3 = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet3 in itemSets3)
		{
			CreateActiveAchievementsForItemsExcludingCurrent(itemSet3, maxCount);
		}
	}

	public void RefreshAfterSynchronise()
	{
		if (!SkiGameManager.Instance)
		{
			return;
		}
		isDemo = !LicenseManager.Instance.AllowAccess();
		int num = 0;
		int num2 = 0;
		while (num2 < m_activeAchievements.Count)
		{
			if (m_activeAchievements[num2].IsDone)
			{
				Object.Destroy(m_activeAchievements[num2].gameObject);
				m_activeAchievements.RemoveAt(num2);
				continue;
			}
			if (m_activeAchievements[num2].IsComplete)
			{
				num++;
			}
			num2++;
		}
		CountCompletedAchievements();
		m_completedAchievementCount -= num;
		LevelManager.Instance.Load();
		PopulateActiveAchievements();
	}

	private void UpdateAchievementFlags()
	{
		m_flags = AchievementFlags.None;
		if (!SkiGameManager.Instance)
		{
			return;
		}
		SpawnParams[] spawnParamsList = SpawnManager.ForegroundInstance.spawnParamsList;
		foreach (SpawnParams spawnParams in spawnParamsList)
		{
			m_flags |= spawnParams.achievementFlags;
		}
		TerrainCurveParams[] curveParamsList = Terrain.GetTerrainForLayer(TerrainLayer.Game).curveParamsList;
		foreach (TerrainCurveParams terrainCurveParams in curveParamsList)
		{
			m_flags |= terrainCurveParams.achievementFlags;
		}
		ItemSet[] itemSets = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet in itemSets)
		{
			if ((bool)itemSet.CurrentItem)
			{
				m_flags |= itemSet.CurrentItem.achievementFlags;
			}
		}
	}

	private void CountCompletedAchievements(Achievement[] achievements)
	{
		foreach (Achievement achievement in achievements)
		{
			if (achievement.IsComplete)
			{
				m_completedAchievementCount++;
			}
		}
	}

	private void CountCompletedAchievements(ItemSet itemSet)
	{
		Item[] items = itemSet.items;
		foreach (Item item in items)
		{
			CountCompletedAchievements(item.achievements);
		}
	}

	public void CountCompletedAchievements()
	{
		m_completedAchievementCount = 0;
		ItemSet[] itemSets = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet in itemSets)
		{
			CountCompletedAchievements(itemSet);
		}
		CountCompletedAchievements(achievements);
	}

	private void DebugCompleteAchievements(Achievement[] achievements)
	{
		foreach (Achievement achievement in achievements)
		{
			if (!achievement.IsComplete)
			{
				achievement.Complete();
				achievement.IsDone = true;
				m_completedAchievementCount++;
			}
		}
	}

	private void DebugCompleteAchievements(ItemSet itemSet)
	{
		Item[] items = itemSet.items;
		foreach (Item item in items)
		{
			DebugCompleteAchievements(item.achievements);
		}
	}

	private void DebugCompleteAchievements()
	{
		m_completedAchievementCount = 0;
		ItemSet[] itemSets = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet in itemSets)
		{
			DebugCompleteAchievements(itemSet);
		}
		DebugCompleteAchievements(achievements);
	}

	public void DebugCompleteAll()
	{
		ClearActiveAchievements();
		DebugCompleteAchievements();
		DebugRefresh();
	}

	public void DebugCompleteLevel()
	{
		int num = LevelManager.Instance.CurrentLevelDescriptor.pointsToComplete - LevelManager.Instance.CurrentLevelPoints;
		int num2 = 1000;
		PopulateActiveAchievements();
		while (num > 0 && m_activeAchievements.Count > 0 && --num2 > 0)
		{
			if (!m_activeAchievements[0].IsComplete)
			{
				m_activeAchievements[0].Complete();
			}
			m_activeAchievements.RemoveAt(0);
			PopulateActiveAchievements();
			m_completedAchievementCount++;
			num--;
		}
		DebugRefresh();
	}

	public void DebugRefresh()
	{
		CountCompletedAchievements();
		if (SkiGameManager.Instance.ShowAchievementsGUI)
		{
			SkiGameManager.Instance.ShowAchievementsGUI = false;
		}
		LevelManager.Instance.Load();
		PopulateActiveAchievements();
		if ((bool)SkiGameManager.Instance)
		{
			SkiGameManager.Instance.Reset();
		}
	}

	private void Awake()
	{
		Instance = this;
		m_guiAchievementList = TransformUtils.Instantiate(guiAchievementListPrefab, base.transform);
		m_guiAchievementComplete = TransformUtils.Instantiate(guiAchievementCompletePrefab, base.transform);
        try
        {
            m_incrementEffect = Object.Instantiate(incrementEffectPrefab);
        }
        catch(System.Exception ex)
        {
            Debug.LogError("GameObject:" + incrementEffectPrefab.name+"\n"+ex);
        }
		m_incrementEffect.transform.parent = base.transform;
		m_incrementEffect.enableEmission = false;
        try
        {
            if (!LicenseManager.Instance.AllowAccess())
            {
                isDemo = true;
            }
        }
        catch
        {
            isDemo = true;
        }
    }
}
