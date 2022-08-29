using System.Collections.Generic;
using UnityEngine;

public class GUIStatSummary : MonoBehaviour
{
	public static GUIStatSummary Instance;

	public Transform content;

	public GUIStatEntry entryPrefab;

	public Transform entryStartNode;

	public float entrySeparation = 1.5f;

	public int maxEntries = 3;

	public GUITransitionAnimator transitionAnimator;

	private List<GUIStatEntry> m_entries = new List<GUIStatEntry>();

	public bool IsShowing
	{
		get
		{
			return transitionAnimator.IsShowing;
		}
	}

	public bool Show()
	{
		if (transitionAnimator.IsShowing)
		{
			return true;
		}
		foreach (GUIStatEntry entry in m_entries)
		{
			Object.Destroy(entry.gameObject);
		}
		m_entries.Clear();
		List<StatDescriptor> list = new List<StatDescriptor>();
		StatDescriptor[] statDescriptors = StatManager.Instance.statDescriptors;
		foreach (StatDescriptor statDescriptor in statDescriptors)
		{
			if (statDescriptor.Value > statDescriptor.SessionStartValue && statDescriptor.Value >= 2)
			{
				list.Add(statDescriptor);
			}
		}
		if (list.Count == 0)
		{
			return false;
		}
		Vector3 localPosition = entryStartNode.localPosition;
		int num = Mathf.Min(maxEntries, list.Count);
		for (int j = 0; j < num; j++)
		{
			CreateEntry(list[j], localPosition);
			localPosition.y -= entrySeparation;
		}
		content.transform.localPosition = new Vector3(0f, (float)(maxEntries - num) * (0f - entrySeparation), 0f);
		transitionAnimator.Show();
		return true;
	}

	public void Hide()
	{
		transitionAnimator.Hide();
	}

	private GUIStatEntry CreateEntry(StatDescriptor statDescriptor, Vector3 pos)
	{
		GUIStatEntry gUIStatEntry = Object.Instantiate(entryPrefab, content.position, content.rotation) as GUIStatEntry;
		gUIStatEntry.transform.parent = content;
		gUIStatEntry.transform.localPosition += pos;
		gUIStatEntry.transform.localScale = Vector3.one;
		gUIStatEntry.StatDescriptor = statDescriptor;
		m_entries.Add(gUIStatEntry);
		return gUIStatEntry;
	}

	private void Awake()
	{
		Instance = this;
	}
}
