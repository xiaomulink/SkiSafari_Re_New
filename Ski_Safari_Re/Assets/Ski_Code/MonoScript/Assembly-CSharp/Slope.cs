using System;
using UnityEngine;

public class Slope : Item
{
	[Serializable]
	public class StartingSkierOverride
	{
		public StartingSkier oldStartingSkier;

		public StartingSkier newStartingSkier;
	}

    public static Slope Instance;

	public string leaderboardId;

	public float leaderboardTextScale = 1f;

	public float scorePerDistance = 1f;

	public Theme theme;

	public GUITitleSign signPrefab;

	public ParticleSystem titleParticles;

	public Sound titleParticlesSound;

	public float titleErodeDelay = 2f;

	public float titleErodeDuration = 1.5f;

	public Vector3 titleCameraOffset = new Vector3(0f, 6f, -20f);

	public GUITutorials.PageSet[] tutorialPageSetOverrides;

	public StartingSkierOverride[] startingSkierOverrides;

	public Vector3 startPositionOffset = new Vector3(0f, 1f, 0f);

	public Vector3 sleepingEffectOffset = new Vector3(0f, 0f, 0f);

	public Vector3 slopeSignPosition = new Vector3(-10f, -0.75f, 0.5f);

	public Vector3 startingSkierSignOffset;

	public SpawnParams[] startingAmbientSpawns;

	public SpawnManager.QueuedSpawn[] initialQueuedSpawns;

	public Attachment[] randomAttachments;

	public SpawnParams[] spawnParams;

	public SpawnParams[] cloudSpawnParams;

	public SpawnParams[] spawnParamsBG1;

	public SpawnParams[] spawnParamsBG2;

	public TerrainCurveParams initialCurveParams;

	public TerrainCurveParams[] curveParams;

	public TerrainCurveParams initialCurveParamsBG1;

	public TerrainCurveParams[] curveParamsBG1;

	public TerrainCurveParams initialCurveParamsBG2;

	public TerrainCurveParams[] curveParamsBG2;

	public Avalanche avalanchePrefab;

	public float avalancheStartDistance = -200f;

	public GameObject skyBoxPrefab;

	public GameObject skyParticlesPrefab;

	private GUITitleSign m_guiSign;

	private bool m_isInitialised;

	private static bool s_changingSlopes;

	public bool IsInitialised
	{
		get
		{
			return m_isInitialised;
		}
	}

	public override bool IsCached
	{
		get
		{
			return BundleManager.Instance.IsCached(bundles);
		}
	}

	public override void DownloadAssets(Action<bool, string> callback)
	{
		BundleManager.Instance.Download(bundles, callback);
	}

	public override void PreloadAssets(Action<bool, string> callback)
	{
		BundleManager.Instance.Preload(bundles, callback);
	}

	public override void LoadAssets()
	{
		BundleManager.Instance.Load(bundles);
	}

	public override void UnloadAssets()
	{
		BundleManager.Instance.Unload(bundles);
	}

	public StartingSkier GetStartingSkier()
	{
		StartingSkier instance = StartingSkier.Instance;
		StartingSkierOverride[] array = startingSkierOverrides;
		foreach (StartingSkierOverride startingSkierOverride in array)
		{
			if (instance.name == startingSkierOverride.oldStartingSkier.name)
			{
				return startingSkierOverride.newStartingSkier;
			}
		}
		return instance;
	}

	public void UpdateSign()
	{
		if (!GUITitleSign.ShouldShow)
		{
			m_guiSign.SnapShowing(false);
		}
		else
		{
			m_guiSign.Showing = true;
		}
		UpdateSignNewState();
	}

	private void UpdateSignNewState()
	{
		if ((bool)m_guiSign)
		{
			m_guiSign.UpdateNewState(!base.HasBeenUsed, ItemManager.Instance.GetItemSet("slope").UnusedItemCount > 0);
		}
	}

	private void SelectNextSlope()
	{
		s_changingSlopes = true;
		if (ItemManager.Instance.GetItemSet("slope").SelectNextItem(true))
		{
			AchievementManager.Instance.ClearActiveAchievements();
			AchievementManager.Instance.PopulateActiveAchievements();
		}
		else
		{
			GUITutorials.Instance.ShowCategory(GUITutorials.Category.TrailUnlock);
		}
		s_changingSlopes = false;
	}

	public void Initialise()
	{
		SkiGameManager.Instance.leaderboardId = leaderboardId;
		SkiGameManager.Instance.scorePerDistance = scorePerDistance;
		FollowCamera.Instance.titleOffset = titleCameraOffset;
		GUITutorials.PageSet[] array = tutorialPageSetOverrides;
		foreach (GUITutorials.PageSet pageSet in array)
		{
			GUITutorials.Instance.AddPageSetOverride(pageSet);
		}
		SkiGameManager.Instance.startingAmbientSpawns = startingAmbientSpawns;
		SpawnManager.ForegroundInstance.spawnParamsList = spawnParams;
		SpawnManager.ForegroundInstance.initialQueuedSpawns = initialQueuedSpawns;
		SpawnManager.GetForLayer(TerrainLayer.Background1).spawnParamsList = spawnParamsBG1;
		SpawnManager.GetForLayer(TerrainLayer.Background2).spawnParamsList = spawnParamsBG2;
		SpawnManager[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(SpawnManager)) as SpawnManager[];
		SpawnManager[] array3 = array2;
		foreach (SpawnManager spawnManager in array3)
		{
			if (spawnManager.name == "SpawnManager_Clouds")
			{
				spawnManager.spawnParamsList = cloudSpawnParams;
				break;
			}
		}
		Terrain.GetTerrainForLayer(TerrainLayer.Game).initialCurveParams = initialCurveParams;
		Terrain.GetTerrainForLayer(TerrainLayer.Game).curveParamsList = curveParams;
		Terrain.GetTerrainForLayer(TerrainLayer.Background1).initialCurveParams = initialCurveParamsBG1;
		Terrain.GetTerrainForLayer(TerrainLayer.Background1).curveParamsList = curveParamsBG1;
		Terrain.GetTerrainForLayer(TerrainLayer.Background2).initialCurveParams = initialCurveParamsBG2;
		Terrain.GetTerrainForLayer(TerrainLayer.Background2).curveParamsList = curveParamsBG2;
		SkiGameManager.Instance.avalanchePrefab = avalanchePrefab;
		SkiGameManager.Instance.avalancheStartDistance = avalancheStartDistance;
		if ((bool)SkyPlane.Instance)
		{
			UnityEngine.Object.Destroy(SkyPlane.Instance.gameObject);
		}
		UnityEngine.Object.Instantiate(skyBoxPrefab);
		TransformUtils.Instantiate(skyParticlesPrefab, base.transform);
		Quaternion identity = Quaternion.identity;
		m_guiSign = UnityEngine.Object.Instantiate(signPrefab, slopeSignPosition, identity) as GUITitleSign;
		m_guiSign.transform.parent = base.transform;
		m_guiSign.SnapShowing(GUITitleSign.ShouldShow && !s_changingSlopes);
		GUITitleSign guiSign = m_guiSign;
		guiSign.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(guiSign.OnClick, new GUIButton.OnClickDelegate(SelectNextSlope));
		SkiGameManager.Instance.guiTitle.SetTitleParticles(titleParticles, titleParticlesSound, titleErodeDelay, titleErodeDuration);
		if (!SkiGameManager.Instance.Initialising)
		{
			if (SkiGameManager.Instance.CurrentDistance > 0f)
			{
				if (SkiGameManager.Instance.ShowShop)
				{
					SkiGameManager.Instance.ShowShop = false;
				}
				else
				{
					SkiGameManager.Instance.Restart(SkiGameManager.RestartMode.QuickTitle);
				}
			}
			else if (SkiGameManager.Instance.TitleScreenActive)
			{
				SkiGameManager.Instance.Reset();
			}
			else
			{
				SkiGameManager.Instance.ReloadSlope();
			}
		}
		s_changingSlopes = false;
		m_isInitialised = true;
	}

	private void Start()
	{
		UpdateSignNewState();
	}

	protected override void OnEnable()
	{
        Instance = this;
   
		if ((bool)SkiGameManager.Instance)
		{  
			ThemeManager.Instance.LoadTheme(theme);
			Initialise();
		}
	}

	protected override void OnDisable()
	{
		if ((bool)m_guiSign)
		{
			UnityEngine.Object.Destroy(m_guiSign.gameObject);
		}
		GUITutorials.PageSet[] array = tutorialPageSetOverrides;
		foreach (GUITutorials.PageSet pageSet in array)
		{
			GUITutorials.Instance.RemovePageSetOverride(pageSet);
		}
	}
}
