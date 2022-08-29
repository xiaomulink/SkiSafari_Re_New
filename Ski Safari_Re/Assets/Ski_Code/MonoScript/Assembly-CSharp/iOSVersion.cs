using System.Text.RegularExpressions;
using UnityEngine;

public class iOSVersion
{
	private static Regex versionMatcher = new Regex("\\OS\\ (\\d+)\\.(\\d+)\\.?(\\d?)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

	public static int[] Get()
	{
		int[] array = new int[3];
		Match match = versionMatcher.Match(SystemInfo.operatingSystem);
		for (int i = 0; i < 3; i++)
		{
			if (match == null || i >= match.Groups.Count || !match.Groups[i + 1].Success || !int.TryParse(match.Groups[i + 1].Value, out array[i]))
			{
				array[i] = 0;
			}
		}
		return array;
	}
}
