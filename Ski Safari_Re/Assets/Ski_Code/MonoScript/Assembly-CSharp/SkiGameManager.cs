using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CustomPanel;

public class SkiGameManager : MonoBehaviour
{
	public enum Mode
	{
		FreeSki = 0,
		Avalanche = 1
	}

	public enum RestartMode
	{
		Title = 0,
		QuickTitle = 1,
		QuickSki = 2,
		Shop = 3
	}

	public enum State
	{
		Initialising = 0,
		FadingIn = 1,
		Title = 2,
		Shop = 3,
		Spawning = 4,
		Playing = 5,
		Paused = 6,
		Finished = 7,
		LevelUp = 8,
		NameEntry = 9,
		ShowingLeaderboard = 10,
		Restarting = 11,
        Custom=12
    }

	public class Finger
	{
		private GUIButton m_button;

		private Vector3 m_lastWorldPos;

		private Camera m_camera;

		public GUIButton Button
		{
			get
			{
				return m_button;
			}
		}

		public void Press(GUIButton button, Vector3 screenPos, Camera camera)
		{
			if ((bool)m_button)
			{
				Release();
			}
			m_button = button;
			screenPos.z = button.transform.position.z;
			m_camera = camera;
			m_lastWorldPos = m_camera.ScreenToWorldPoint(screenPos);
			button.Click(m_lastWorldPos);
		}

		public void Drag(Vector3 screenPos)
		{
			Vector3 vector = m_camera.ScreenToWorldPoint(screenPos);
			Vector3 positionDelta = vector - m_lastWorldPos;
			m_button.Drag(positionDelta);
			m_lastWorldPos = vector;
		}

		public void Release()
		{
			m_button.Release();
			m_button = null;
		}
	}

	public delegate void OnStateChangedDelegate(State newState);

	public static SkiGameManager Instance;

	public Mode mode;

	public PlatformManager platformManagerPrefab;

	public SpawnParams[] startingAmbientSpawns;

	public GameObject sleepingEffect;

	public float startingAmbientSpawnDelayMin = 2f;

	public float startingAmbientSpawnDelayMax = 10f;

	public float restartDelay = 0.5f;

	public SpawnParams bestDistanceSign;

	public string leaderboardId = "high_scores";

	public float scorePerDistance = 1f;

	public float scorePerCollectable = 10f;

	public int coinMultiplier = 1;

	public int lives = 3;

	public int maxLives = 99;

	public int numCollectablesForLife = 20;

	public Attachment playerDefaultAttachmentPrefab;

	public Player secondChancePlayerPrefab;

	public Avalanche avalanchePrefab;

	public float avalancheStartDistance = -200f;

	public Avalanche[] backgroundAvalanchePrefabs;

	public Camera guiCamera;

	public LayerMask guiLayerMask;

	public LayerMask worldGuiLayerMask;

	public LayerMask popupGuiLayerMask;

	public Renderer popupBackground;

	public float popupFadeDuration = 0.5f;

	public GUIScore guiScore;

	public GUIComboThermometer guiComboThermometer;

	public GUIContinueButton continueButton;

	public float finishDelay = 2f;

	public GUIAchievements guiAchievementsPrefab;

	public GUIHighScore guiHighScorePrefab;

	public GUILeaderboard guiLeaderboardPrefab;

	public GUITransitionAnimator guiSettingsPrefab;

	public GUITransitionAnimator guiSocialSettingsPrefab;

	public GUITutorials guiTutorialsPrefab;

	public float initialShowTutorialsDelay = 4f;

	public float initialAchievementListShowTime = 4f;

	public GUITitle guiTitle;

	public GUIOtherGames guiOtherGamesPrefab;

	public GUIFacebookPopup guiFacebookPopupPrefab;

	public GUIShop guiShopPrefab;
	public GUICustom guiCustomPrefab;

	public GUIRolloutButton rolloutButton;

	public GUICoinCount guiCoinCount;

	public GUITransitionAnimator guiDistance;

	public Transform startButtonScaleNode;

	public Transform resumeButtonScaleNode;

	public GUIShopButton shopButton;

	public GUIShopSpecialButton shopSpecialButton;

	public GUITransitionAnimator[] onInitialiseShowTransitions;

	public GUITransitionAnimator[] onTutorialsShowingHideTransitions;

	public GUITransitionAnimator[] onTutorialsViewedShowTransitions;

	public GUITransitionAnimator[] onShopVisibleTransitions;

	public GUITransitionAnimator[] onShopHiddenTransitions;

	public GUITransitionAnimator[] onStartHideTransitions;

	public GUITransitionAnimator[] onPlayShowTransitions;

	public GUITransitionAnimator[] onCreditsShowTransitions;

	public GUITransitionAnimator[] onPlayHideTransitions;

	public GUITransitionAnimator[] onPauseShowTransitions;

	public GUITransitionAnimator[] onPauseHideTransitions;

	public GUITransitionAnimator[] onFinishShowTransitions;

	public GUITransitionAnimator[] onFinishHideTransitions;

	public GUITransitionAnimator[] onShowLeaderboardShowTransitions;

	public GUITransitionAnimator[] onShowEndShopHideTransitions;

	public GUITransitionAnimator[] onRestartHideTransitions;

	public float pauseAudioFadeDelay = 0.5f;

	public float pauseAudioFadeDuration = 1f;

	public Sound startSound;

	public Sound titleMusic;

	public Sound gameMusic;

	public Sound shopMusic;

	public Sound[] finishStings;

	public Transform focusIndicator;

	public GUIButton pocketRocket;

	public GUIButton superBlue;

	public bool debug_HideUI;

	public GameObject[] debug_UIToHide;

	public OnStateChangedDelegate OnStateChanged;

	public Player.SimplePlayerDelegate OnPlayerSpawn;

	public Player.SimplePlayerDelegate OnSecondChance;

	public Player.SimplePlayerDelegate OnDieByAvalanche;

	public Player.SimplePlayerDelegate OnPlayerInputPressed;

	private static int MaxFingers = 10;
    [SerializeField]
	private State m_state;

	private float m_stateTimer;

	private float m_lastUpdateTime;

	private bool m_titleGUIStateUpdated;

	private bool m_restartOnShopClose;

	private bool m_restartToShop;

	private bool m_autoStartSkiing;

	private List<string> m_inputLocks = new List<string>();

	private bool m_popupEnabled;

	private bool m_popupFading;

	private float m_popupFadeTime;

	private LayerMask m_inputMask = -1;

	private List<GUIButton> m_activeButtons = new List<GUIButton>();

	private Finger[] m_fingers;

	private Vector3 m_startPos;

	private float m_startingAmbientSpawnTimer;

	private GameObject m_sleepingEffectInstance;

	private GameObject m_startingAnimal;

	private float m_currentDistance;

	private float m_currentTime;

	private float m_score;

	private int m_currentRank;

	private Profile.LeaderboardEntry m_highScoreEntry;

	private bool m_shownNameEntryContinueButton;

	private int m_numRestarts;

	private bool m_secondChanceUsed;

	private int m_sessionCoinCount;

	private string m_currentSlopeName;

	private GUIAchievements m_guiAchievements;

	private GUILeaderboard m_guiLeaderboard;

	private GUIHighScore m_guiHighScore;

	private GUITransitionAnimator m_guiSettings;

	private GUITransitionAnimator m_guiSocialSettings;

	private SpawnManager[] m_spawnManagers;

	private GUIOtherGames m_guiOtherGames;

	private GUIFacebookPopup m_guiFacebookPopup;

	private GUIShop m_guiShop;
	private GUICustom m_guiCustom;

	private Terrain[] m_terrains;

    public Custom custom = new Custom();

    public int customIncreaseMultiple=1;

    public Bundle Slope_01;
    public Slope Slope_01Online;

    public Slope _Slope;

    public bool isOnline = false;

    public CustomSound gui_item_startingskier_yeti;

    public Sound PlayerHit1;
    public Sound PlayerHit2;

    public AudioSource FXFlameTrailGlow_1;
    public AudioSource FxFlameTrail_1;
    public AudioSource FxFlameTrail_2;
    public AudioSource FxFlameTrail_3;
    public AudioSource FxFlameTrail_4;
    public GameResCustomManager ResCustomManager;

    [Serializable]
    public class CustomSound
    {
        public Sound Resource;
        public AudioClip[] Audios;
    }

    public bool Initialising
	{
		get
		{
			return m_state <= State.FadingIn;
		}
	}

	public bool TitleScreenActive
	{
		get
		{
			return m_state <= State.Title;
		}
	}

	public bool ShowShop
	{
		get
		{
			return m_state == State.Shop;
		}
		set
		{
			if (value)
			{
				if (m_state == State.Title)
				{
					SetState(State.Shop);
					rolloutButton.Hide(false);
					HideTransitions(onShopHiddenTransitions);
					guiTitle.transitionAnimator.disallowDeactivate = true;
					guiTitle.transitionAnimator.Hide();
					m_titleGUIStateUpdated = false;
					m_restartOnShopClose = false;
					GameState.Synchronise();
				}
				else if (m_state == State.ShowingLeaderboard)
				{
					SetState(State.Shop);
					guiCoinCount.transitionAnimator.Hide();
					HideTransitions(onShopHiddenTransitions);
					HideTransitions(onShowEndShopHideTransitions);
					ShowLeaderboardGUI = false;
					m_titleGUIStateUpdated = false;
					m_restartOnShopClose = true;
					GameState.Synchronise();
				}
				SoundManager.Instance.StopMusic();
			}
			else if (m_state == State.Shop)
			{
				if (m_restartOnShopClose)
				{
					m_guiShop.Hide();
					Restart(RestartMode.QuickTitle);
					return;
				}
				SetState(State.Title);
				HideTransitions(onShopVisibleTransitions);
				m_guiShop.Hide();
				m_titleGUIStateUpdated = false;
				GameState.Synchronise();
				SoundManager.Instance.PlayMusic(titleMusic, false);
				GUITutorials.Instance.UpdateAutoShow();
			}
		}
	}

    public bool ShowCustom
    {
        get
        {
            return m_state == State.Custom;
        }
        set
        {
            if (value)
            {
                if (m_state == State.Title)
                {
                    SetState(State.Custom);
                    rolloutButton.Hide(false);
                    HideTransitions(onShopHiddenTransitions);
                    guiTitle.transitionAnimator.disallowDeactivate = true;
                    guiTitle.transitionAnimator.Hide();
                    m_titleGUIStateUpdated = false;
                    m_restartOnShopClose = false;
                    GameState.Synchronise();
                }
                else if (m_state == State.ShowingLeaderboard)
                {
                    SetState(State.Custom);
                    guiCoinCount.transitionAnimator.Hide();
                    HideTransitions(onShopHiddenTransitions);
                    HideTransitions(onShowEndShopHideTransitions);
                    ShowLeaderboardGUI = false;
                    m_titleGUIStateUpdated = false;
                    m_restartOnShopClose = true;
                    GameState.Synchronise();
                }
                SoundManager.Instance.StopMusic();
            }
            else if (m_state == State.Custom)
            {
                if (m_restartOnShopClose)
                {
                    m_guiCustom.Hide();
                    Restart(RestartMode.QuickTitle);
                    return;
                }
                SetState(State.Title);
                HideTransitions(onShopVisibleTransitions);
                //m_guiCustom.Hide();
                m_titleGUIStateUpdated = false;
                GameState.Synchronise();
                SoundManager.Instance.PlayMusic(titleMusic, false);
                GUITutorials.Instance.UpdateAutoShow();
            }
        }
    }

    public bool AutoStartSkiing
	{
		get
		{
			return m_autoStartSkiing;
		}
	}

	public bool PopupEnabled
	{
		get
		{
			return m_popupEnabled;
		}
		set
		{
			if (value != m_popupEnabled)
			{
				float num = Mathf.Max(0f, m_popupFadeTime - Time.realtimeSinceStartup);
				m_popupEnabled = value;
				m_inputMask = ((!value) ? (-1) : popupGuiLayerMask.value);
				m_popupFadeTime = Time.realtimeSinceStartup + (popupFadeDuration - num);
				m_popupFading = true;
				if (value)
				{
					popupBackground.enabled = true;
				}
			}
		}
	}

	public bool Started
	{
		get
		{
			return m_state >= State.Playing;
		}
	}

	public bool Playing
	{
		get
		{
			return m_state == State.Playing;
		}
	}

	public bool Finished
	{
		get
		{
			return m_state >= State.Finished;
		}
	}

	public int NumRestarts
	{
		get
		{
			return m_numRestarts;
		}
	}

	public bool Paused
	{
		get
		{
			return m_state == State.Paused;
		}
		set
		{
			if (value && m_state == State.Playing)
			{
				Time.timeScale = 0f;
				SetState(State.Paused);
			}
			else if (!value && m_state == State.Paused)
			{
				Time.timeScale = 1f;
				SetState(State.Playing);
			}
		}
	}

	public bool ShowAchievementsGUI
	{
		get
		{
			return m_guiAchievements.IsShowing;
		}
		set
		{
			if (value)
			{
				m_guiAchievements.Show();
				HideTransitions(onTutorialsShowingHideTransitions);
			}
			else
			{
				m_guiAchievements.Hide();
				ShowTransitions(onTutorialsShowingHideTransitions);
			}
		}
	}

	public bool ShowLeaderboardGUI
	{
		get
		{
			return m_guiLeaderboard.transitionAnimator.IsShowing;
		}
		set
		{
			ShowFullscreenWindow(m_guiLeaderboard.transitionAnimator, value);
		}
	}

	public bool ShowSettingsGUI
	{
		get
		{
			return m_guiSettings.IsShowing;
		}
		set
		{
			ShowFullscreenWindow(m_guiSettings, value);
		}
	}

	public bool ShowOtherGamesGUI
	{
		get
		{
			return (bool)m_guiOtherGames && m_guiOtherGames.transitionAnimator.IsShowing;
		}
		set
		{
			if ((bool)m_guiOtherGames)
			{
				ShowFullscreenWindow(m_guiOtherGames.transitionAnimator, value);
			}
		}
	}

	public bool ShowFacebookGUI
	{
		get
		{
			return (bool)m_guiFacebookPopup && m_guiFacebookPopup.transitionAnimator.IsShowing;
		}
		set
		{
			if ((bool)m_guiFacebookPopup)
			{
				ShowFullscreenWindow(m_guiFacebookPopup.transitionAnimator, value);
			}
		}
	}

	public bool ShowSocialSettingsGUI
	{
		get
		{
			return (bool)m_guiSocialSettings && m_guiSocialSettings.IsShowing;
		}
		set
		{
			if ((bool)m_guiSocialSettings)
			{
				ShowFullscreenWindow(m_guiSocialSettings, value);
			}
		}
	}

	public GUIOtherGames GUIOtherGames
	{
		get
		{
			return m_guiOtherGames;
		}
	}

	public float CurrentDistance
	{
		get
		{
			return m_currentDistance;
		}
	}

	public float CurrentScore
	{
		get
		{
			return m_score;
		}
	}

	public Vector3 StartPos
	{
		get
		{
			return m_startPos;
		}
	}

	public void AddInputLock(string id)
	{
		if (!m_inputLocks.Contains(id))
		{
			m_inputLocks.Add(id);
		}
	}

	public void RemoveInputLock(string id)
	{
		m_inputLocks.Remove(id);
	}

	public void IncrementLives()
	{
		if (mode == Mode.FreeSki)
		{
			lives = Mathf.Min(maxLives, lives + 1);
		}
	}

	public void DecrementLives()
	{
		if (mode == Mode.FreeSki)
		{
			lives = Mathf.Max(0, lives - 1);
		}
	}

	public void IncrementCollectableCount(int value)
	{
		AddScore((float)value * scorePerCollectable, Mathf.Max(1, StuntManager.Instance.Combo), null);
		GameState.IncrementCoinCount(coinMultiplier * value);
		if (!GUIAchievementComplete.Instance.IsShowing && !CreditsSpawner.Instance.EnableSpawning)
		{
			guiCoinCount.transitionAnimator.Show();
			if (value > 1)
			{
				guiCoinCount.ShowAddedCoinCount(value);
			}
		}
		m_sessionCoinCount += coinMultiplier;
	}

	public void AddScore(float points, int combo, string styleDescription)
	{
		m_score += points * (float)combo;
		if (!string.IsNullOrEmpty(styleDescription))
		{
			guiScore.AddStyle(points, combo, styleDescription);
		}
		guiScore.SetScore(m_score, true);
	}

	public void BreakCombo()
	{
		guiScore.BreakCombo();
	}

	public void OnTutorialsShowing()
	{
		HideTransitions(onTutorialsShowingHideTransitions);
	}

	public void OnTutorialsHiding()
	{
		ShowTransitions(onTutorialsShowingHideTransitions);
		guiTitle.transitionAnimator.disallowDeactivate = false;
		guiTitle.transitionAnimator.Show();
		rolloutButton.Show();
	}

	public void OnPlayerHitByAvalanche(Player player)
	{
		if ((bool)secondChancePlayerPrefab && !m_secondChanceUsed)
		{
			Vector3 position = player.transform.position;
			Quaternion rotation = player.transform.rotation;
			Pool.Despawn(player.gameObject);
			Player player2 = PlayerManager.SpawnReplacement(player, secondChancePlayerPrefab, position, rotation);
			if (OnSecondChance != null)
			{
				OnSecondChance(player2);
			}
			m_secondChanceUsed = true;
		}
		else
		{
			if (OnDieByAvalanche != null)
			{
				OnDieByAvalanche(player);
			}
			Pool.Despawn(player.gameObject);
			Finish();
		}
	}

	private void ShowFullscreenWindow(GUITransitionAnimator transitionAnimator, bool show)
	{
		if (show)
		{
			transitionAnimator.Show();
			HideTransitions(onTutorialsShowingHideTransitions);
			return;
		}
		transitionAnimator.Hide();
		if (m_state <= State.Title)
		{
			ShowTransitions(onTutorialsShowingHideTransitions);
		}
	}

	private void SpawnAnimal()
	{
		StartingSkier startingSkier = Slope.Instance.GetStartingSkier();
		m_startingAnimal = Pool.Spawn(startingSkier.animal, m_startPos + startingSkier.animalOffset, Quaternion.identity);
	}

	private void SpawnPlayer()
	{
		if ((bool)m_startingAnimal)
		{
			Pool.Despawn(m_startingAnimal);
		}

		StartingSkier startingSkier = Slope.Instance.GetStartingSkier();
        Debug.LogError("Player1");

        Player player = PlayerManager.Spawn(startingSkier.skier, m_startPos, Quaternion.identity, PlayerManager.PlayerType.Human_1);
        if (SkiGameManager.Instance.isOnline)
        {
            Player player1 = PlayerManager.Spawn(startingSkier.skier, m_startPos, Quaternion.identity, PlayerManager.PlayerType.Human_2);
            Player player2 = PlayerManager.Spawn(startingSkier.skier, m_startPos, Quaternion.identity, PlayerManager.PlayerType.Human_3);
            Player player3 = PlayerManager.Spawn(startingSkier.skier, m_startPos, Quaternion.identity, PlayerManager.PlayerType.Human_4);
            if ((bool)playerDefaultAttachmentPrefab)
            {
                player1.TryAttach(playerDefaultAttachmentPrefab, false);
                player2.TryAttach(playerDefaultAttachmentPrefab, false);
                player3.TryAttach(playerDefaultAttachmentPrefab, false);
            }
            if (OnPlayerSpawn != null)
            {
                OnPlayerSpawn(player1);
                OnPlayerSpawn(player2);
                OnPlayerSpawn(player3);
            }
        }
        if ((bool)playerDefaultAttachmentPrefab)
		{
			player.TryAttach(playerDefaultAttachmentPrefab, false);
           
		}
		if (OnPlayerSpawn != null)
		{
			OnPlayerSpawn(player);
			
		}
		UnityEngine.Object.Destroy(m_sleepingEffectInstance);
		if ((bool)startingSkier.npcSpawnParams)
		{
			SpawnManager.ForegroundInstance.RegisterSpawnParams(startingSkier.npcSpawnParams);
		}
		SoundManager.Instance.PlayMusic(gameMusic, false);
	}

	public void StartSkiing()
	{
        Debug.Log("StartAvalanche!");
		m_currentRank = LevelManager.Instance.CurrentLevel;
		m_currentSlopeName = Slope.Instance.name;
		if (AchievementManager.Instance.ActiveAchievements.Count == 0 && GameState.GetString("credits_version") != GUITutorials.Instance.UpdateVersion)
		{
			GameState.SetString("credits_version", GUITutorials.Instance.UpdateVersion);
			CreditsSpawner.Instance.EnableSpawning = true;
		}
		StartingSkier startingSkier = Slope.Instance.GetStartingSkier();
		if ((bool)startingSkier.animal)
		{
			Invoke("SpawnAnimal", startingSkier.animalSpawnDelay);
		}
		Invoke("SpawnPlayer", startingSkier.spawnDelay);
		ItemManager.Instance.MarkCurrentItemsUsed();
		Instance.ShowAchievementsGUI = false;
		GUITutorials.Instance.Hide();
		ShowLeaderboardGUI = false;
		ShowOtherGamesGUI = false;
		rolloutButton.Hide();
		GUITransitionAnimator[] array = onStartHideTransitions;
		foreach (GUITransitionAnimator gUITransitionAnimator in array)
		{
			gUITransitionAnimator.Hide();
		}
		if (mode == Mode.Avalanche)
		{
			Terrain terrainForLayer = Terrain.GetTerrainForLayer(TerrainLayer.Game);
			Vector3 position = new Vector3(m_startPos.x + avalancheStartDistance, 0f, terrainForLayer.transform.position.z);
			terrainForLayer.GetHeight(position.x, ref position.y);
			UnityEngine.Object.Instantiate(avalanchePrefab, position, Quaternion.identity);
			Avalanche[] array2 = backgroundAvalanchePrefabs;
			foreach (Avalanche avalanche in array2)
			{
				Terrain terrainForLayer2 = Terrain.GetTerrainForLayer(avalanche.terrainLayer);
				Vector3 position2 = new Vector3(m_startPos.x + avalancheStartDistance, 0f, terrainForLayer2.transform.position.z);
				terrainForLayer2.GetHeight(position2.x, ref position2.y);
				UnityEngine.Object.Instantiate(avalanche, position2, Quaternion.identity);
			}
		}
		SoundManager.Instance.PlaySound(startSound);
		SetState(State.Spawning);
	}

	public void Restart(RestartMode mode)
	{
		SetState(State.Restarting);
		m_autoStartSkiing = false;
		m_restartToShop = false;
		switch (mode)
		{
		case RestartMode.Title:
			break;
		case RestartMode.QuickTitle:
			Reset();
			break;
		case RestartMode.Shop:
			Reset();
			m_restartToShop = true;
			break;
		case RestartMode.QuickSki:
			Reset();
			m_autoStartSkiing = true;
			break;
		}
	}

    public bool OnlineFinish=false;

	public void Finish()
	{
		SetState(State.Finished);
	}

	public void AddActiveButton(GUIButton button)
	{
		m_activeButtons.Add(button);
	}

	public void RemoveActiveButton(GUIButton button)
	{
		m_activeButtons.Remove(button);
		Finger[] fingers = m_fingers;
		foreach (Finger finger in fingers)
		{
			if (finger.Button == button)
			{
				finger.Release();
				break;
			}
		}
	}

	public void ReloadSlope()
	{
		DisableWorld();
		EnableWorld();
		StartingSkier.Instance.UpdateSign();
		guiTitle.ShowTitleErrode();
	}

	public void Reset()
	{
		DisableWorld();
		GUITutorials.Instance.SelectPage(0);
		UnityEngine.Object.Destroy(m_sleepingEffectInstance);
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}

	private void DisableWorld()
	{
		if ((bool)Avalanche.Instance)
		{
			UnityEngine.Object.Destroy(Avalanche.Instance.gameObject);
		}
		SpawnManager[] spawnManagers = m_spawnManagers;
		foreach (SpawnManager spawnManager in spawnManagers)
		{
			spawnManager.enabled = false;
		}
		Terrain[] terrains = m_terrains;
		foreach (Terrain terrain in terrains)
		{
			terrain.enabled = false;
		}
		if ((bool)DistanceSpawner.Instance)
		{
			DistanceSpawner.Instance.enabled = false;
		}
		StuntManager.Instance.Reset();
		AchievementManager.Instance.ReloadActiveAchievements();
		if ((bool)CreditsSpawner.Instance)
		{
			CreditsSpawner.Instance.enabled = false;
		}
		PlayerManager.RemovePlayer(PlayerManager.PlayerType.Human_1);
		if ((bool)m_sleepingEffectInstance)
		{
			UnityEngine.Object.Destroy(m_sleepingEffectInstance);
		}
		guiTitle.ResetTitleOverlay();
	}

	private void EnableWorld()
	{
		Terrain[] terrains = m_terrains;
		foreach (Terrain terrain in terrains)
		{
			terrain.enabled = true;
			terrain.GenerateInitialChunks();
		}
		SpawnManager[] spawnManagers = m_spawnManagers;
		foreach (SpawnManager spawnManager in spawnManagers)
		{
			spawnManager.enabled = true;
		}
		if ((bool)DistanceSpawner.Instance)
		{
			DistanceSpawner.Instance.enabled = true;
		}
		if ((bool)CreditsSpawner.Instance)
		{
			CreditsSpawner.Instance.enabled = true;
		}
		Vector3 zero = Vector3.zero;
		Terrain.GetTerrainForLayer(TerrainLayer.Game).GetHeight(0f, ref zero.y);
		m_sleepingEffectInstance = UnityEngine.Object.Instantiate(sleepingEffect, zero + Slope.Instance.sleepingEffectOffset, Quaternion.identity) as GameObject;
		m_sleepingEffectInstance.transform.parent = base.transform;
		guiTitle.FadeIn();
	}

	public void SpawnStartingAmbientAnimal(int index)
	{
		index = Mathf.Min(startingAmbientSpawns.Length - 1, index);
		SpawnParams spawnParams = startingAmbientSpawns[index];
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(TerrainLayer.Game);
		float z = Mathf.Abs(terrainForLayer.transform.position.z - Camera.main.transform.position.z);
		Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, z));
		position.x -= spawnParams.rightClearance;
		Vector3 normal = Vector3.up;
		terrainForLayer.GetHeightAndNormal(position.x, ref position.y, ref normal);
		position.y += spawnParams.terrainHeightOffset;
		Quaternion rotation = Quaternion.identity;
		if (spawnParams.matchSlopeRotation)
		{
			rotation = Quaternion.LookRotation(Vector3.forward, normal);
		}
		SpawnManager.SpawnFlags spawnFlags = (SpawnManager.SpawnFlags)9;
		SpawnManager.ForegroundInstance.ManualSpawn(spawnParams, position, rotation, spawnFlags);
		m_startingAmbientSpawnTimer = UnityEngine.Random.Range(startingAmbientSpawnDelayMin, startingAmbientSpawnDelayMax);
	}

	public void StopAmbientSpawns()
	{
		m_startingAmbientSpawnTimer = float.MaxValue;
	}

	private void UpdateStartingAmbientSpawns()
	{
		if (startingAmbientSpawns.Length != 0)
		{
			m_startingAmbientSpawnTimer -= Time.deltaTime;
			if (m_startingAmbientSpawnTimer <= 0f)
			{
				SpawnStartingAmbientAnimal(UnityEngine.Random.Range(0, startingAmbientSpawns.Length));
			}
		}
	}

	private void UpdateButtonPulsing(Transform node, float period, float amount)
	{
		if (0 == 0)
		{
			float num = 1f + Mathf.Sin(m_stateTimer * period) * amount;
			node.localScale = new Vector3(num, num, num);
		}
	}

	private void UpdateDistanceScore()
	{
		if ((bool)Player.Instance)
		{
			float num = Mathf.FloorToInt(Player.Instance.transform.position.x - m_startPos.x);
			float num2 = num - m_currentDistance;
			m_currentDistance = num;
			if (num2 > 0f)
			{
				m_score += num2 * scorePerDistance * Mathf.Max(1f, StuntManager.Instance.Combo) * customIncreaseMultiple;
			}
			guiScore.SetScore(m_score , false);
		}
	}

	private bool TestButtonCollision(Ray ray, GUIButton button, Camera camera, ref float closestDistance)
	{
		float num = Mathf.Min(button.radius, closestDistance);
		float num2 = ((!(button.height > 0f)) ? num : Mathf.Min(closestDistance, button.height));
		Vector3 vector = button.transform.position + button.center - ray.origin;
		if (camera.orthographic)
		{
			if (vector.x > 0f - num && vector.x < num && vector.y > 0f - num2 && vector.y < num2)
			{
				closestDistance = Mathf.Min(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
				return true;
			}
		}
		else
		{
			float magnitude = vector.magnitude;
			Vector3 lhs = vector * (1f / magnitude);
			float f = Mathf.Acos(Vector3.Dot(lhs, ray.direction));
			float num3 = Mathf.Sin(f) * magnitude;
			if (num3 <= num)
			{
				closestDistance = num3;
				return true;
			}
		}
		return false;
	}

	private bool TryFingerPress(int fingerId, Vector3 screenPos, Camera camera, LayerMask layerMask)
	{
		GUIButton gUIButton = null;
		float closestDistance = float.MaxValue;
		Ray ray = camera.ScreenPointToRay(screenPos);
		for (int i = 0; i < m_activeButtons.Count; i++)
		{
			GUIButton gUIButton2 = m_activeButtons[i];
			if (((1 << gUIButton2.gameObject.layer) & (int)layerMask) != 0 && TestButtonCollision(ray, gUIButton2, camera, ref closestDistance))
			{
				gUIButton = gUIButton2;
			}
		}
		if ((bool)gUIButton)
		{
			HandleRelease(fingerId);
			m_fingers[fingerId].Press(gUIButton, screenPos, camera);
			m_activeButtons.Remove(gUIButton);
			return true;
		}
		return false;
	}

	private bool HandlePress(int fingerId, Vector3 screenPos)
	{
		if (TryFingerPress(fingerId, screenPos, guiCamera, (int)guiLayerMask & (int)m_inputMask))
		{
			return true;
		}
		if (!GUITutorials.Instance.AutoShow && TryFingerPress(fingerId, screenPos, Camera.main, (int)worldGuiLayerMask & (int)m_inputMask))
		{
			return true;
		}
		return false;
	}

	private void HandleDrag(int fingerId, Vector3 screenPos)
	{
		if ((bool)m_fingers[fingerId].Button)
		{
			m_fingers[fingerId].Drag(screenPos);
		}
	}

	private void HandleRelease(int fingerId)
	{
		if (fingerId < m_fingers.Length && (bool)m_fingers[fingerId].Button)
		{
			m_activeButtons.Add(m_fingers[fingerId].Button);
			m_fingers[fingerId].Release();
		}
	}

	private void HandleBackPress()
	{
		if (m_popupEnabled)
		{
			return;
		}
		for (int num = m_activeButtons.Count - 1; num >= 0; num--)
		{
			if (m_activeButtons[num].clickOnBackPress)
			{
				m_activeButtons[num].Click(Vector3.zero);
				return;
			}
		}
		if (rolloutButton.IsShowing)
		{
			rolloutButton.Minimise();
		}
		else if (TitleScreenActive)
		{
			Application.Quit();
		}
	}

	private void UpdateInput(float deltaTime)
	{
		if (m_inputLocks.Count > 0)
		{
			return;
		}
		bool flag = false;
		int num = Mathf.Min(Input.touchCount, MaxFingers);
		for (int i = 0; i < num; i++)
		{
			Touch touch = Input.GetTouch(i);
			switch (touch.phase)
			{
			case TouchPhase.Began:
				if (!flag && HandlePress(touch.fingerId, touch.position))
				{
					flag = true;
				}
				break;
			case TouchPhase.Moved:
				HandleDrag(touch.fingerId, touch.position);
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				HandleRelease(touch.fingerId);
				break;
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			HandleBackPress();
		}
		if (!Player.Instance)
		{
			return;
		}
		bool flag2 = Input.GetMouseButton(0);
		if (Paused)
		{
			flag2 = false;
		}
		else
		{
			Finger[] fingers = m_fingers;
			foreach (Finger finger in fingers)
			{
				if ((bool)finger.Button)
				{
					flag2 = false;
					break;
				}
			}
		}
		if (flag2)
		{
			Player.Instance.LiftInput = 1f;
			if (OnPlayerInputPressed != null)
			{
				OnPlayerInputPressed(Player.Instance);
			}
		}
		else
		{
			Player.Instance.LiftInput = 0f;
		}
	}

	private void PressEnterButtons()
	{
		for (int num = m_activeButtons.Count - 1; num >= 0; num--)
		{
			if (m_activeButtons[num].clickOnEnterPress)
			{
				m_activeButtons[num].Click(Vector3.zero);
				break;
			}
		}
	}

	public void OnTutorialsViewed()
	{
		ShowTransitions(onTutorialsViewedShowTransitions);
	}

	public void OnShowAchievementComplete()
	{
		guiCoinCount.transitionAnimator.Hide();
		guiDistance.Hide();
	}

	private void FinishNameEntry()
	{
		if ((bool)m_guiHighScore)
		{
			m_guiHighScore.Submit();
		}
	}

	private void ShowTransitions(GUITransitionAnimator[] transitionAnimators)
	{
		foreach (GUITransitionAnimator gUITransitionAnimator in transitionAnimators)
		{
			gUITransitionAnimator.Show();
		}
	}

	private void SnapShowTransitions(GUITransitionAnimator[] transitionAnimators)
	{
		foreach (GUITransitionAnimator gUITransitionAnimator in transitionAnimators)
		{
			gUITransitionAnimator.SnapShow();
		}
	}

	public void HideTransitions(GUITransitionAnimator[] transitionAnimators)
	{
		foreach (GUITransitionAnimator gUITransitionAnimator in transitionAnimators)
		{
			gUITransitionAnimator.Hide();
		}
	}

	private void SnapHideTransitions(GUITransitionAnimator[] transitionAnimators)
	{
		foreach (GUITransitionAnimator gUITransitionAnimator in transitionAnimators)
		{
			gUITransitionAnimator.SnapHide();
		}
	}

	private void SetState(State state)
	{
		State state2 = m_state;
		if (state2 == State.Shop)
		{
			m_guiShop.Hide();
		}
		m_state = state;
		m_stateTimer = 0f;
		if (OnStateChanged != null)
		{
			OnStateChanged(state);
		}
        switch (m_state)
        {
            case State.Initialising:
                {
                    GUITransitionAnimator[] array2 = onInitialiseShowTransitions;
                    foreach (GUITransitionAnimator gUITransitionAnimator2 in array2)
                    {
                        if (isOnline)
                        {
                            if(gUITransitionAnimator2.name != "StartButton")
                                gUITransitionAnimator2.SnapShow();
                            if (gUITransitionAnimator2.name == "MatchButton")
                                gUITransitionAnimator2.SnapShow();
                        }else
                        {
                            gUITransitionAnimator2.SnapShow();
                        }
                    }
                    break;
                }
            case State.Title:
                if (GUIAchievements.Instance.AutoShow)
                {
                    Instance.ShowAchievementsGUI = true;

                }
                break;
            case State.Shop:
                break;
            case State.Custom:
                break;
            case State.Spawning:
                m_sessionCoinCount = 0;
                break;
            case State.Playing:
                AudioListener.volume = 1f;
                AchievementManager.Instance.ShowAchievementList = false;
                resumeButtonScaleNode.localScale = Vector3.one;
                HideTransitions(onPlayHideTransitions);
                if (!CreditsSpawner.Instance.EnableSpawning)
                {
                    ShowTransitions(onPlayShowTransitions);
                }
                else
                {
                    ShowTransitions(onCreditsShowTransitions);
                }
                break;
            case State.Paused:
                AchievementManager.Instance.ShowAchievementList = true;
                ShowTransitions(onPauseShowTransitions);
                HideTransitions(onPauseHideTransitions);
                GameState.Save();
                break;
            case State.Finished:
                {
                    SocialManager.Instance.Authenticate(false);
                    SoundManager.Instance.PlaySting(finishStings[UnityEngine.Random.Range(0, finishStings.Length)], false);
                    CommentManager.Instance.HideGUI();
                    AchievementManager.Instance.ShowAchievementList = false;
                    ShowTransitions(onFinishShowTransitions);
                    HideTransitions(onFinishHideTransitions);

                    PlayerManager.RemovePlayer(PlayerManager.PlayerType.Human_1);
                    if (m_currentDistance > GameState.GetFloat("best_distance"))
                    {
                        GameState.SetFloat("best_distance", m_currentDistance);
                        SocialManager.Instance.SubmitDistance(Mathf.FloorToInt(m_currentDistance));
                    }
                    if (GameState.IsHighScore(leaderboardId, m_score * customIncreaseMultiple))
                    {
                        string @string = PlayerPrefs.GetString("LastPlayerName");
                        m_highScoreEntry = GameState.AddHighScore(leaderboardId, @string, m_score, m_currentRank);
                    }
                    int coinCount = GameState.CoinCount;
                    break;
                }
            case State.LevelUp:
                if (AchievementManager.Instance.CompletedActiveAchievementCount > 0)
                {
                    Instance.ShowAchievementsGUI = true;
                }
                else
                {
                    SetState(State.NameEntry);
                }
                break;
            case State.NameEntry:
                if (m_highScoreEntry != null)
                {
                    m_guiHighScore = UnityEngine.Object.Instantiate(guiHighScorePrefab);
                    m_guiHighScore.transform.parent = base.transform;
                    m_guiHighScore.HighScoreEntry = m_highScoreEntry;
                }
                if (!m_guiHighScore)
                {
                    SetState(State.ShowingLeaderboard);
                }
                else
                {
                    m_shownNameEntryContinueButton = false;
                }
                break;
            case State.ShowingLeaderboard:
                {
                    m_guiLeaderboard.RecentEntry = m_highScoreEntry;

                    ShowLeaderboardGUI = true;
                    //if (!isOnline)
                    {
                        GUITransitionAnimator[] array3 = onShowLeaderboardShowTransitions;
                        foreach (GUITransitionAnimator gUITransitionAnimator3 in array3)
                        {
                            if (isOnline)
                            {
                                if(gUITransitionAnimator3.name== "ShopButton"|| gUITransitionAnimator3.name == "RestartButton")
                                {
                                    return;
                                }
                            }
                            gUITransitionAnimator3.Show();
                        }
                    }
                    break;

                }
            case State.Restarting:
                {
                    CommentManager.Instance.HideGUI();
                    AchievementManager.Instance.ShowAchievementList = false;
                    ShowLeaderboardGUI = false;
                    CreditsSpawner.Instance.EnableSpawning = false;
                    startButtonScaleNode.localScale = Vector3.one;
                    GUITransitionAnimator[] array = onRestartHideTransitions;
                    foreach (GUITransitionAnimator gUITransitionAnimator in array)
                    {
                        gUITransitionAnimator.Hide();
                    }
                    SoundManager.Instance.StopMusic();
                    CommentManager.Instance.IncrementRuns();
                    m_guiLeaderboard.RecentEntry = null;
                    m_highScoreEntry = null;
                    m_secondChanceUsed = false;
                    GameState.Synchronise();
                    int num = 10;
                    float num2 = m_currentTime / 60f;
                    int num3 = 0;
                    if (num2 > Mathf.Epsilon)
                    {
                        num3 = Mathf.FloorToInt((float)m_sessionCoinCount / num2 / (float)num) * num;
                    }
                    m_sessionCoinCount = Mathf.FloorToInt((float)m_sessionCoinCount / (float)num) * num;
                    AnalyticsManager.Instance.SendEvent("session_finish", "coins", m_sessionCoinCount.ToString(), "minutes", Mathf.FloorToInt(num2).ToString(), "coins_per_minute", num3.ToString(), "slope", m_currentSlopeName);
                    m_numRestarts++;
                    break;
                }
            case State.FadingIn:
                break;
        }
	}

	private void OnAuthenticationFinished(bool success)
	{
		if (success)
		{
			GameState.SubmitScores();
			float @float = GameState.GetFloat("best_distance");
			if (@float > 0f)
			{
				SocialManager.Instance.SubmitDistance(Mathf.FloorToInt(@float));
			}
		}
	}

	public static void SendStartupApsalarEvents()
	{
		string deviceModel = SystemInfo.deviceModel;
		bool genuine = Application.genuine;
		AnalyticsManager.Instance.SendEvent("Ski Safari Started", "model", deviceModel, "genuine", genuine.ToString());
	}

	private void Awake()
	{
		Instance = this;
		Application.targetFrameRate = 60;
		if (!PlatformManager.Instance)
		{
			PlatformManager platformManager = UnityEngine.Object.Instantiate(platformManagerPrefab);
			platformManager.name = platformManagerPrefab.name;
		}
		GameState.Synchronise();
		m_fingers = new Finger[MaxFingers];
		for (int i = 0; i < m_fingers.Length; i++)
		{
			m_fingers[i] = new Finger();
		}
		popupBackground.enabled = false;
		m_guiAchievements = TransformUtils.Instantiate(guiAchievementsPrefab, base.transform);
		m_guiLeaderboard = TransformUtils.Instantiate(guiLeaderboardPrefab, base.transform);
		m_guiSettings = TransformUtils.Instantiate(guiSettingsPrefab, base.transform);
		m_guiSocialSettings = TransformUtils.Instantiate(guiSocialSettingsPrefab, base.transform);
		if ((bool)guiOtherGamesPrefab)
		{
			m_guiOtherGames = TransformUtils.Instantiate(guiOtherGamesPrefab, base.transform);
		}
		if ((bool)guiFacebookPopupPrefab)
		{
			m_guiFacebookPopup = TransformUtils.Instantiate(guiFacebookPopupPrefab, base.transform);
		}
		m_guiShop = TransformUtils.Instantiate(guiShopPrefab, base.transform);
		m_guiCustom = TransformUtils.Instantiate(guiCustomPrefab, base.transform);
        //废弃
		TransformUtils.Instantiate(guiTutorialsPrefab, base.transform);
        //
        if (!SkiGameManager.Instance.isOnline)
        {
            guiComboThermometer.gameObject.SetActive(true);
        }
		Go.defaultUpdateType = GoUpdateType.TimeScaleIndependentUpdate;
		Go.defaultEaseType = GoEaseType.ExpoOut;
		if (focusIndicator != null)
		{
			focusIndicator.gameObject.SetActive(false);
		}
		m_numRestarts = 0;
	}

	private void OnEnable()
	{
		Time.timeScale = 1f;
		AudioListener.volume = 1f;
		m_startingAmbientSpawnTimer = UnityEngine.Random.Range(startingAmbientSpawnDelayMin, startingAmbientSpawnDelayMax);
		m_lastUpdateTime = Time.time;
		m_currentDistance = 0f;
		m_score = 0f;
		m_currentTime = 0f;
		m_currentRank = 0;
		SetState(State.Initialising);
	}

	private void OnDisable()
	{
	}

	private async void Start()
	{
        customIncreaseMultiple = 1;
        ResCustomManager.enabled = false;
        m_spawnManagers = UnityEngine.Object.FindObjectsOfType(typeof(SpawnManager)) as SpawnManager[];
		m_terrains = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
		SocialManager instance = SocialManager.Instance;
		instance.OnAuthenticationFinished = (Action<bool>)Delegate.Combine(instance.OnAuthenticationFinished, new Action<bool>(OnAuthenticationFinished));
     
		AchievementManager.Instance.CountCompletedAchievements();
      
        LevelManager.Instance.Load();
      
        ItemManager.Instance.Load();

        AchievementManager.Instance.PopulateActiveAchievements();
    
        SendStartupApsalarEvents();
		Reset();
		SocialManager.Instance.LoadAchievements();
   
        await Task.Delay(3000);
        PanelManager.Init();
        PanelManager.Open<Tip>(PanelManager.UIstyle.Default, "公告 \n 没有什么公告");
    }

    private void Update()
	{
        _Slope = Slope.Instance;
       if(isOnline)
        {
            if(SkiGameManager.Instance.CurrentDistance>=5000)
            {
                if (!OnlineFinish)
                {
                    Finish();
                    OnlineFinish = true;
                }
            }
        }
        if (_Slope.name=="Slope_08")
        {
            gui_item_startingskier_yeti.Resource.clip = gui_item_startingskier_yeti.Audios[1];
        }else
        {
            gui_item_startingskier_yeti.Resource.clip = gui_item_startingskier_yeti.Audios[0];
        }
        try
        {
            if (PlayerPrefs.GetInt("OriginalMap") == 0)
            {
                Slope_01.library.textures[7].name = "env_terrain_01_game_fill_1";
            }else
            {
                Slope_01.library.textures[7].name = "env_terrain_01_game_fill";

            }
        }
        catch
        {
            PlayerPrefs.SetInt("OriginalMap", 0);
        }
        float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = realtimeSinceStartup - m_lastUpdateTime;
		m_lastUpdateTime = realtimeSinceStartup;
		m_stateTimer += num;

		switch (m_state)
		{
		case State.Initialising:
			if ((bool)Slope.Instance)
			{
				if (AchievementManager.Instance.HasPendingDemoStateChange)
				{
					AchievementManager.Instance.RefreshAfterSynchronise();
				}
				ItemManager.Instance.RefreshCurrentCosts();
				m_startPos = Slope.Instance.startPositionOffset;
				FollowCamera.Instance.Snap();
				EnableWorld();
				GUIAchievements.Instance.UpdateAutoShow();
				if (!GUIAchievements.Instance.AutoShow)
				{
                        try
                        {
                            GUITutorials.Instance.UpdateAutoShow();
                        }
                        catch { }
				}
                    try
                    {
                        if (GUITutorials.Instance.AutoShow || GUIAchievements.Instance.AutoShow)
                        {
                            SnapHideTransitions(onTutorialsShowingHideTransitions);
                            guiTitle.transitionAnimator.disallowDeactivate = true;
                            guiTitle.transitionAnimator.SnapHide();
                            rolloutButton.SnapHide();
                            m_autoStartSkiing = false;
                        }
                        else
                        {
                            rolloutButton.SnapShow();
                        }
                    }
                    catch
                    {

                    }
				StartingSkier.Instance.UpdateSign();
				Slope.Instance.UpdateSign();
                    Debug.LogError("Initing");
				float @float = GameState.GetFloat("best_distance");
				if (@float > 0f)
				{
					SpawnManager.ForegroundInstance.QueueSpawn(bestDistanceSign, SpawnManager.SpawnFlags.None, Mathf.CeilToInt(@float));
				}
				m_titleGUIStateUpdated = false;
				if (m_autoStartSkiing)
				{
					SnapHideTransitions(onStartHideTransitions);
					StartSkiing();
				}
				else if (m_restartToShop)
				{
					m_restartToShop = false;
					SetState(State.Shop);
					SnapHideTransitions(onShopHiddenTransitions);
					guiTitle.transitionAnimator.disallowDeactivate = true;
					guiTitle.transitionAnimator.SnapHide();
					rolloutButton.SnapHide();
					m_titleGUIStateUpdated = false;
					m_restartOnShopClose = false;
					FollowCamera.Instance.Snap();
				}
				else
				{
					SetState(State.FadingIn);
					SoundManager.Instance.PlayMusic(titleMusic, false);
				}
			}
			break;
		case State.FadingIn:
			if (guiTitle.FinishedTransition)
			{
				SetState(State.Title);
			}
			break;
		case State.Title:
		{
			UpdateStartingAmbientSpawns();
			UpdateInput(num);
			UpdateButtonPulsing(startButtonScaleNode, 3f, 0.1f);
			if (shopButton.CanBuyAnyItem)
			{
				UpdateButtonPulsing(shopButton.scaleNode, 3f, 0.1f);
			}
			if (shopSpecialButton.CanBuyItem)
			{
				UpdateButtonPulsing(shopSpecialButton.scaleNode, 3f, 0.1f);
			}
			if ( !GUITutorials.Instance.IsShowing && !GUITutorials.Instance.AutoShow && 
                        !GUIAchievements.Instance.AutoShow && m_stateTimer >= 0.5f)
			{
				if (!m_titleGUIStateUpdated)
				{
					ShowTransitions(onShopHiddenTransitions);
					guiTitle.transitionAnimator.disallowDeactivate = false;
					guiTitle.transitionAnimator.Show();
					rolloutButton.Show();
					shopSpecialButton.transitionAnimator.Show();
					m_titleGUIStateUpdated = true;
				}
				if (GameState.IsLoadRequested && !ShowAchievementsGUI && !ShowLeaderboardGUI)
				{
					GameState.Synchronise();
				}
			}
			if (!debug_HideUI)
			{
				break;
			}
			GameObject[] array = debug_UIToHide;
			foreach (GameObject gameObject in array)
			{
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
				Renderer[] array2 = componentsInChildren;
				foreach (Renderer renderer in array2)
				{
					renderer.enabled = false;
				}
			}
			break;
		}
		case State.Shop:
			UpdateInput(num);
			if (!m_titleGUIStateUpdated && m_stateTimer >= 0.5f && (FollowCamera.Instance.IsStationary || m_stateTimer >= 1.5f))
			{
				ShowTransitions(onShopVisibleTransitions);
				m_guiShop.Show();
				m_titleGUIStateUpdated = true;
			}
			break;
		case State.Spawning:
			if ((bool)Player.Instance)
			{
				SetState(State.Playing);
				if (AchievementManager.Instance.CompletedAchievementCount > 0 && !CreditsSpawner.Instance.EnableSpawning && PlayerPrefs.GetInt("hide_start_challenges") == 0)
				{
					AchievementManager.Instance.ShowAchievementList = true;
				}
			}
			break;
            case State.Custom:
                UpdateInput(num);
                if (!m_titleGUIStateUpdated && m_stateTimer >= 0.5f && (FollowCamera.Instance.IsStationary || m_stateTimer >= 1.5f))
                {
                    ShowTransitions(onShopVisibleTransitions);
                    PanelManager.Open<CustomPanel>();
                    m_titleGUIStateUpdated = true;
                }
                break;
		case State.Playing:
			UpdateDistanceScore();
			UpdateInput(num);
			m_currentTime += Time.deltaTime;
			if (m_stateTimer > initialAchievementListShowTime && AchievementManager.Instance.ShowAchievementList)
			{
				AchievementManager.Instance.ShowAchievementList = false;
			}
			break;
		case State.Paused:
			UpdateInput(num);
			UpdateButtonPulsing(resumeButtonScaleNode, 3f, 0.1f);
			if (m_stateTimer > 0.5f)
			{
				AudioListener.volume = 1f - Mathf.Clamp01((m_stateTimer - pauseAudioFadeDelay) / pauseAudioFadeDuration);
			}
			break;
		case State.Finished:
			if (m_stateTimer > finishDelay)
			{
				if (SoundManager.Instance.SFXEnabled)
				{
					SoundManager.Instance.SFXEnabled = false;
					SoundManager.Instance.SFXEnabled = true;
				}
				SetState(State.LevelUp);
			}
			break;
		case State.LevelUp:
			UpdateInput(num);
			if (!Instance.ShowAchievementsGUI)
			{
				SetState(State.NameEntry);
			}
			break;
		case State.NameEntry:
			UpdateInput(num);
			if (!m_shownNameEntryContinueButton && m_stateTimer >= 1f)
			{
				continueButton.Show(FinishNameEntry);
				m_shownNameEntryContinueButton = true;
			}
			else if (!m_guiHighScore)
			{
				SetState(State.ShowingLeaderboard);
			}
			break;
		case State.ShowingLeaderboard:
			UpdateInput(num);
			if (shopButton.CanBuyAnyItem)
			{
				UpdateButtonPulsing(shopButton.scaleNode, 3f, 0.1f);
			}
			if (shopSpecialButton.CanBuyItem)
			{
				UpdateButtonPulsing(shopSpecialButton.scaleNode, 3f, 0.1f);
			}
			break;
		case State.Restarting:
			if (m_stateTimer >= restartDelay)
			{
				Reset();
			}
			break;
		}

    }

	private void LateUpdate()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup < m_popupFadeTime)
		{
			float num = (m_popupFadeTime - realtimeSinceStartup) / popupFadeDuration;
			if (m_popupEnabled)
			{
				num = 1f - num;
			}
			popupBackground.material.SetColor("_TintColor", new Color(1f, 1f, 1f, num));
		}
		else if (m_popupFading)
		{
			if (m_popupEnabled)
			{
				popupBackground.material.SetColor("_TintColor", Color.white);
			}
			else
			{
				popupBackground.material.SetColor("_TintColor", new Color(1f, 1f, 1f, 0.5f));
				popupBackground.enabled = false;
			}
			m_popupFading = false;
		}
	}

    private void OnApplicationPause(bool pause)
	{
		if (m_state == State.Playing)
		{
            if (!SkiGameManager.Instance.isOnline)
            {
                Paused = true;
            }
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(guiCamera.transform.position + new Vector3(0f, 0f, (guiCamera.nearClipPlane + guiCamera.farClipPlane) * 0.5f), new Vector3(guiCamera.orthographicSize * 8f / 3f, guiCamera.orthographicSize * 2f, guiCamera.farClipPlane - guiCamera.nearClipPlane));
	}
}
