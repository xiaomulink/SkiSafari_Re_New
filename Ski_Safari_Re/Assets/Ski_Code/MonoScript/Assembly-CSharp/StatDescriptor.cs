using System;

[Serializable]
public class StatDescriptor
{
	public string key;

	public string variant;

	public StatDescriptorMode mode;

	public string description;

	public string valueSuffix;

	private int m_sessionStartValue;

	public int Value
	{
		get
		{
			if (mode == StatDescriptorMode.Instance)
			{
				return GameState.GetStatInstanceMax(key, variant);
			}
			return GameState.GetStatSessionMax(key, variant);
		}
	}

	public int SessionStartValue
	{
		get
		{
			return m_sessionStartValue;
		}
	}

	public void OnSessionStarted()
	{
		m_sessionStartValue = Value;
	}
}
