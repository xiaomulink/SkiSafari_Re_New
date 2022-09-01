using UnityEngine;

public class Achievement_Time : Achievement
{
	public bool persistent;

	public float requiredTime = 10f;

	private float m_time;

	private float m_bestTime;

	public override bool IsComplete
	{
		get
		{
			return GameState.GetFloat(base.name) >= requiredTime;
		}
	}

	protected override void OnComplete()
	{
		m_time = requiredTime;
	}

	protected override void OnLoad()
	{
		m_bestTime = GameState.GetFloat(base.name);
		if (persistent)
		{
			m_time = m_bestTime;
		}
		else
		{
			m_time = 0f;
		}
	}

	protected override void OnSave()
	{
		GameState.SetFloat(base.name, m_time);
	}

	protected void Increment(float amount)
	{
		m_time += amount;
		if (m_time >= requiredTime)
		{
			m_bestTime = requiredTime;
			Complete();
		}
		else
		{
			Save();
		}
	}

	protected void ResetTime()
	{
		m_time = 0f;
	}

	public override void MigrateToProfile(Profile profile)
	{
		profile.MigrateFloat(base.name);
	}

	public override string ToString()
	{
		if (!IsComplete)
		{
			if (persistent || (SkiGameManager.Instance.Started && !SkiGameManager.Instance.Finished))
			{
				int num = Mathf.CeilToInt(requiredTime - m_time);
				if ((float)num < requiredTime)
				{
					return string.Format("{0}, {1} second{2} to go", description, num, (num == 1) ? string.Empty : "s");
				}
			}
			else if (m_bestTime > 0f)
			{
				int num2 = Mathf.FloorToInt(m_bestTime);
				return string.Format("{0}, best is {1} second{2}", description, num2, (num2 == 1) ? string.Empty : "s");
			}
		}
		return description;
	}
}
