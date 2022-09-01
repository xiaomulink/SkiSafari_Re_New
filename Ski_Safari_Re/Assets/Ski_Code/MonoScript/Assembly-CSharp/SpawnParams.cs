using System.Collections.Generic;
using UnityEngine;

public class SpawnParams : MonoBehaviour
{
	public AchievementFlags achievementFlags;

	public GameObject[] prefabs;

	public int maxActiveSpawns = 10;

	public int requiredDistance;

	public List<string> blackListCurves = new List<string>();

	public int blackListCurveDistance;

	public int radius;

	public int leftClearance = 1;

	public int rightClearance = 1;

	public float minSeparation = 20f;

	public float maxSeparation = 40f;

	public bool groupable;

	public float groupChance = 0.5f;

	public float minSlope = 30f;

	public float maxSlope = 90f;

	public bool matchSlopeRotation = true;

	public float terrainHeightOffset;

	public float randomTerrainHeightOffset;

	public bool avoidCaveBoundaries;

	public int cullBehindDistance = 50;

	public int cullForwardDistance = 400;
}
