using UnityEngine;

public static class CameraUtils
{
	public static bool IsPointVisible(Vector3 position)
	{
		Vector3 vector = Camera.main.WorldToViewportPoint(position);
		if (vector.x < -1f || vector.x > 1f || vector.y < -1f || vector.y > 1f)
		{
			return false;
		}
		return true;
	}
}
