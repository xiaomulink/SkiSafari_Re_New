using System.Collections.Generic;
using UnityEngine;

public class GUIAchievementList : MonoBehaviour
{
	public enum ShowPosition
	{
		BottomLeft = 0,
		MiddleLeft = 1,
		MiddleCenter = 2,
		TitleMiddleCenter = 3
	}

	public static GUIAchievementList Instance;

	public Vector3 middleCenterScreenPos;

	public Vector3 titleMiddleCenterScreenPos;

	public Vector3 bottomLeftScreenPos;

	public Vector3 middleLeftScreenPos;

	public Vector3 offScreenPos;

	public float transitionInTime = 0.5f;

	public float transitionOutTime = 0.5f;

	public GUIAchievementEntry entryPrefab;

	public Transform entryStartNode;

	public float entrySeparation = 1.5f;

	public float entryMoveDuration = 0.5f;

	public Vector3 newEntryInitialOffset = new Vector3(0f, -10f, 0f);

	public Color nameInactiveColor;

	public Color nameCompleteColor;

	public GameObject allComplete;

	public GameObject allCompleteSpeechBubble;

	public GameObject allCompleteFace;

	public Sound allCompleteSound;

	private bool m_showing;

	private bool m_colorCompleteAchievements;

	private List<GUIAchievementEntry> m_entries = new List<GUIAchievementEntry>();

	public bool IsShowing
	{
		get
		{
			return m_showing;
		}
	}

	public List<GUIAchievementEntry> Entries
	{
		get
		{
			return m_entries;
		}
	}

	public void RelinkAchievementEntries()
	{
		int num = 0;
		while (num < m_entries.Count)
		{
			if (RelinkAchievementEntry(m_entries[num]))
			{
				num++;
			}
			else
			{
				RemoveAchievementEntry(m_entries[num]);
			}
		}
	}

	public GUIAchievementEntry GetNextCompletedAchievement()
	{
		foreach (GUIAchievementEntry entry in m_entries)
		{
			if (entry.Achievement.IsComplete)
			{
				return entry;
			}
		}
		return null;
	}

	public bool CheckNewAchievements()
	{
		List<Achievement> activeAchievements = AchievementManager.Instance.ActiveAchievements;
		for (int i = 0; i < activeAchievements.Count; i++)
		{
			if (!IsAchievementActive(activeAchievements[i]))
			{
				Vector3 localPosition = entryStartNode.localPosition;
				localPosition.y -= entrySeparation * (float)m_entries.Count;
				GUIAchievementEntry gUIAchievementEntry = CreateAchievementEntry(activeAchievements[i], localPosition);
				GoTweenConfig config = new GoTweenConfig().localPosition(localPosition + newEntryInitialOffset);
				Go.from(gUIAchievementEntry.gameObject.transform, entryMoveDuration, config);
				return true;
			}
		}
		if (activeAchievements.Count == 0 && !AchievementManager.Instance.IsDemo)
		{
			allComplete.SetActive(true);
			allCompleteFace.SetActive(true);
			allCompleteSpeechBubble.SetActive(true);
			GoTweenConfig config2 = new GoTweenConfig().scale(new Vector3(0.2f, 0.2f, 0.2f), true).setEaseType(GoEaseType.ElasticPunch);
			Go.to(allCompleteSpeechBubble.transform, 0.5f, config2);
			SoundManager.Instance.PlaySound(allCompleteSound);
		}
		return false;
	}

	private GUIAchievementEntry CreateAchievementEntry(Achievement achievement, Vector3 pos)
	{
		GUIAchievementEntry gUIAchievementEntry = Object.Instantiate(entryPrefab, base.transform.position, base.transform.rotation) as GUIAchievementEntry;
		gUIAchievementEntry.transform.parent = base.transform;
		gUIAchievementEntry.transform.localPosition += pos;
		gUIAchievementEntry.transform.localScale = Vector3.one;
		gUIAchievementEntry.Achievement = achievement;
		gUIAchievementEntry.starSprite.SetActive(achievement.IsComplete);
		gUIAchievementEntry.newSprite.SetActive(achievement.IsNew);
		achievement.IsNew = false;
		UpdateAchievementEntry(gUIAchievementEntry);
		m_entries.Add(gUIAchievementEntry);
		return gUIAchievementEntry;
	}

	private void UpdateAchievementEntry(GUIAchievementEntry entry)
	{
		entry.nameText.Text = entry.Achievement.ToString();
		if (m_colorCompleteAchievements && entry.Achievement.IsComplete)
		{
			entry.nameText.GetComponent<Renderer>().material.color = nameCompleteColor;
		}
		else if (!entry.Achievement.HasBasicRequirements)
		{
			entry.nameText.GetComponent<Renderer>().material.color = nameInactiveColor;
		}
		entry.nameText.TextScale = entry.Achievement.descriptionTextScale;
	}

	public void RemoveAchievementEntry(GUIAchievementEntry entryToRemove)
	{
		Vector3 localPosition = entryStartNode.localPosition;
		bool flag = false;
		for (int i = 0; i < m_entries.Count; i++)
		{
			GUIAchievementEntry gUIAchievementEntry = m_entries[i];
			if (gUIAchievementEntry == entryToRemove)
			{
				flag = true;
			}
			else if (flag)
			{
				gUIAchievementEntry.transform.position += new Vector3(0f, entrySeparation, 0f);
				GoTweenConfig config = new GoTweenConfig().localPosition(localPosition);
				Go.from(gUIAchievementEntry.gameObject.transform, entryMoveDuration, config);
			}
			localPosition.y -= entrySeparation;
		}
		m_entries.Remove(entryToRemove);
		Object.Destroy(entryToRemove.gameObject);
	}

	private bool RelinkAchievementEntry(GUIAchievementEntry entry)
	{
		foreach (Achievement activeAchievement in AchievementManager.Instance.ActiveAchievements)
		{
			if (entry.AchievementName == activeAchievement.name)
			{
				entry.Achievement = activeAchievement;
				return true;
			}
		}
		return false;
	}

	private bool IsAchievementActive(Achievement achievement)
	{
		foreach (GUIAchievementEntry entry in m_entries)
		{
			if (entry.AchievementName == achievement.name)
			{
				return true;
			}
		}
		return false;
	}

	private void MoveToPosition(Vector3 screenPos)
	{
		Vector3 endValue = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(screenPos);
		if (!m_showing)
		{
			Vector3 position = base.transform.position;
			position.y = endValue.y;
			base.transform.position = position;
		}
		Go.to(base.gameObject.transform, transitionInTime, new GoTweenConfig().position(endValue));
	}

	public void Show(ShowPosition showPosition, bool force = false)
	{
		if (!force && AchievementManager.Instance.ActiveAchievements.Count == 0)
		{
			return;
		}
		CancelInvoke("Deactivate");
		Go.killAllTweensWithTarget(base.transform);
		switch (showPosition)
		{
		case ShowPosition.BottomLeft:
			MoveToPosition(bottomLeftScreenPos);
			m_colorCompleteAchievements = true;
			break;
		case ShowPosition.MiddleLeft:
			MoveToPosition(middleLeftScreenPos);
			m_colorCompleteAchievements = false;
			break;
		case ShowPosition.MiddleCenter:
			MoveToPosition(middleCenterScreenPos);
			m_colorCompleteAchievements = false;
			break;
		case ShowPosition.TitleMiddleCenter:
			MoveToPosition(titleMiddleCenterScreenPos);
			m_colorCompleteAchievements = false;
			break;
		}
		if (GUIAchievementComplete.Instance.IsShowing)
		{
			GUIAchievementComplete.Instance.ClearAndHide();
		}
		if (m_showing)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
			foreach (GUIAchievementEntry entry in m_entries)
			{
				Object.Destroy(entry.gameObject);
			}
			m_entries.Clear();
			Vector3 localPosition = entryStartNode.localPosition;
			foreach (Achievement activeAchievement in AchievementManager.Instance.ActiveAchievements)
			{
				CreateAchievementEntry(activeAchievement, localPosition);
				localPosition.y -= entrySeparation;
			}
			if (AchievementManager.Instance.ActiveAchievements.Count == 0)
			{
				allComplete.SetActive(true);
				allCompleteSpeechBubble.SetActive(false);
				allCompleteFace.SetActive(false);
			}
			else
			{
				allComplete.SetActive(false);
			}
		}
		m_showing = true;
	}

	public void Hide(bool deactivate)
	{
		if (m_showing)
		{
			Go.killAllTweensWithTarget(base.gameObject.transform);
			Vector3 endValue = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
			endValue.y = base.transform.position.y;
			Go.to(base.gameObject.transform, transitionOutTime, new GoTweenConfig().position(endValue));
			if (allComplete.activeInHierarchy)
			{
				allCompleteFace.SetActive(false);
				allCompleteSpeechBubble.SetActive(false);
			}
			if (deactivate)
			{
				Invoke("Deactivate", transitionOutTime);
			}
			m_showing = false;
		}
	}

	private void Deactivate()
	{
		base.gameObject.SetActive(false);
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if ((bool)SkiGameManager.Instance)
		{
			base.transform.position = SkiGameManager.Instance.guiCamera.ViewportToWorldPoint(offScreenPos);
		}
		base.gameObject.SetActive(false);
		m_showing = false;
	}

	private void Update()
	{
		if (!m_showing || !SkiGameManager.Instance || SkiGameManager.Instance.Paused)
		{
			return;
		}
		foreach (GUIAchievementEntry entry in m_entries)
		{
			UpdateAchievementEntry(entry);
		}
	}
}
