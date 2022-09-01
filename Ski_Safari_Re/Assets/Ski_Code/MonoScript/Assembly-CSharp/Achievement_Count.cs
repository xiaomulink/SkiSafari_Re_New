using UnityEngine;

public class Achievement_Count : Achievement
{
	public bool persistent;

	public int requiredCount = 1;

	private int m_count;

	private int m_bestCount;

	protected int Count
	{
		get
		{
			return m_count;
		}
	}

	public override bool IsComplete
	{
		get
		{
			return GameState.GetInt(base.name) >= requiredCount;
		}
	}

	protected override void OnComplete()
	{
		m_count = (m_bestCount = requiredCount);
	}

	protected override void OnLoad()
	{
		m_bestCount = GameState.GetInt(base.name);
		if (persistent)
		{
			m_count = m_bestCount;
		}
		else
		{
			m_count = 0;
		}
	}

	protected override void OnSave()
	{
		GameState.SetInt(base.name, m_bestCount);
	}

	protected void IncrementCount(int amount)
	{
		if (m_count < requiredCount)
		{
			m_count += amount;
			m_bestCount = Mathf.Max(m_count, m_bestCount);
			AchievementManager.Instance.OnIncrement(this);
			if (m_count >= requiredCount)
			{
				Complete();
			}
			else
			{
				Save();
			}
		}
	}

	protected void ResetCount()
	{
		m_count = 0;
	}

	public override void MigrateToProfile(Profile profile)
	{
		profile.MigrateInt(base.name);
	}

	public override string ToString()
	{
		if (!IsComplete && requiredCount > 1)
		{
			if (persistent || (m_count > 0 && SkiGameManager.Instance.Started && !SkiGameManager.Instance.Finished))
			{
				return string.Format("{0}, {1} to go", description, requiredCount - m_count);
			}
			if (m_bestCount > 1)
			{
				return string.Format("{0}, best is {1}", description, m_bestCount);
			}
		}
		return description;
	}
}
