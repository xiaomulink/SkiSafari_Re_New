using UnityEngine;

public class DebugManager : MonoBehaviour
{
	public static DebugManager Instance;

	public static bool DebugEnabled
	{
		get
		{
			return false;
		}
	}
}
