using UnityEngine;

public abstract class AnalyticsManager : MonoBehaviour
{
	public static AnalyticsManager Instance;

	public abstract void SendEvent(string eventName);

	public abstract void SendEvent(string eventName, object key1, object value1);

	public abstract void SendEvent(string eventName, object key1, object value1, object key2, object value2);

	public abstract void SendEvent(string eventName, object key1, object value1, object key2, object value2, object key3, object value3);

	public abstract void SendEvent(string eventName, object key1, object value1, object key2, object value2, object key3, object value3, object key4, object value4);

	protected virtual void Awake()
	{
		Instance = this;
	}
}
