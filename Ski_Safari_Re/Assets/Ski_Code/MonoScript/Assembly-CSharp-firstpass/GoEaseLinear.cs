using System;
using UnityEngine;

public static class GoEaseLinear
{
	public static float EaseNone(float t, float b, float c, float d)
	{
		return c * t / d + b;
	}

	public static float Punch(float t, float b, float c, float d)
	{
		if (t == 0f)
		{
			return 0f;
		}
		if ((t /= d) == 1f)
		{
			return 0f;
		}
		return c * Mathf.Pow(2f, -10f * t) * Mathf.Sin(t * ((float)Math.PI * 2f) / 0.3f);
	}
}
