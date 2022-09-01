using System.Collections.Generic;
using UnityEngine.Analytics;

public class AnalyticsManager_Unity : AnalyticsManager
{
	public override void SendEvent(string eventName)
	{
		DoSendEvent(eventName);
	}

	public override void SendEvent(string eventName, object key1, object value1)
	{
		DoSendEvent(eventName, key1, value1);
	}

	public override void SendEvent(string eventName, object key1, object value1, object key2, object value2)
	{
		DoSendEvent(eventName, key1, value1, key2, value2);
	}

	public override void SendEvent(string eventName, object key1, object value1, object key2, object value2, object key3, object value3)
	{
		DoSendEvent(eventName, key1, value1, key2, value2, key3, value3);
	}

	public override void SendEvent(string eventName, object key1, object value1, object key2, object value2, object key3, object value3, object key4, object value4)
	{
		DoSendEvent(eventName, key1, value1, key2, value2, key3, value3, key4, value4);
	}

	private void DoSendEvent(string eventName, params object[] keyValueParams)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		for (int i = 0; i < keyValueParams.Length - 1; i += 2)
		{
			dictionary[keyValueParams[i].ToString()] = keyValueParams[i + 1].ToString();
		}
		if (Analytics.CustomEvent(eventName, dictionary) == AnalyticsResult.Ok)
		{
		}
	}
}
