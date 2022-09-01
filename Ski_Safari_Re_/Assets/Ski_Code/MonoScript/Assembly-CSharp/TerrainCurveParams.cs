using UnityEngine;

public class TerrainCurveParams : MonoBehaviour
{
	public string curveName = "undefined";

	public AchievementFlags achievementFlags;

	public int occurances = 1;

	public float requiredDistance;

	public float operationalDistance = 1000f;

	public int segmentCountMin = 10;

	public int segmentCountMax = 40;

	public float heightMin = 1f;

	public float heightMax = 4f;

	public float angleLength = 360f;

	public float angleOffset = 90f;

	public SpawnParams fixedSpawnParams;

	public SpawnManager.SpawnFlags fixedSpawnFlags = SpawnManager.SpawnFlags.IgnoreAll;

	public bool preventSpawning;

	public bool CheckCriteria(int distance)
	{
		return (float)distance >= requiredDistance;
	}
}
