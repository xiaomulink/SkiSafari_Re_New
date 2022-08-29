using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Leaderboard_old
{
	public class Entry : IComparable
	{
		public string name;

		public float score;

		public int rank;

		public bool isSubmitted;

		public bool isRecent;

		public int CompareTo(object obj)
		{
			Entry entry = obj as Entry;
			return (entry == null) ? 1 : ((entry.score > score) ? 1 : ((entry.score < score) ? (-1) : 0));
		}
	}

	public int maxEntries = 5;

	private string m_id;

	private string m_filename;

	private List<Entry> m_entries = new List<Entry>();

	public string Id
	{
		get
		{
			return m_id;
		}
	}

	public List<Entry> Entries
	{
		get
		{
			return m_entries;
		}
	}

	public float BestScore
	{
		get
		{
			return (m_entries.Count <= 0) ? 0f : m_entries[0].score;
		}
	}

	public void Load(string leaderboardId)
	{
		m_id = leaderboardId;
		m_entries.Clear();
		if (string.IsNullOrEmpty(leaderboardId))
		{
			return;
		}
		m_filename = Path.Combine(Application.persistentDataPath, leaderboardId + ".txt");
		using (CSVReader cSVReader = new CSVReader(m_filename, true))
		{
			while (cSVReader.NextRow())
			{
				string name = cSVReader.ReadString();
				float score = cSVReader.ReadFloat();
				int rank = cSVReader.ReadInt();
				bool isSubmitted = cSVReader.ReadInt() != 0;
				m_entries.Add(new Entry
				{
					name = name,
					score = score,
					rank = rank,
					isSubmitted = isSubmitted
				});
			}
		}
		m_entries.Sort();
	}
}
