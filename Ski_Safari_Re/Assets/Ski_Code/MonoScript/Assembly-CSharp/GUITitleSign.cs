using System.Collections;
using UnityEngine;

public class GUITitleSign : GUIButton
{
    public static GUITitleSign titleSign;
	public GameObject pivot;

	public GameObject newSprite;

	public GameObject showEffectPrefab;

	public GameObject hideEffectPrefab;

	public Vector3 hiddenOffset = new Vector3(0f, -10f, 0f);

	public Vector3 flipRotation = new Vector3(0f, 90f, 0f);

	public Vector3 wobbleRotation = new Vector3(0f, 0f, 10f);

	private PoolRef m_currentEffect;

    public bool isshow=true;

	public bool m_showing;

	private bool m_isNew;

	private bool m_hasOtherNew;

	private bool m_hasSnappedToTerrain;

	protected GameObject CurrentEffect
	{
		get
		{
			return m_currentEffect;
		}
		set
		{
			if ((bool)m_currentEffect)
			{
				Pool.Despawn(m_currentEffect.Ref);
			}
			m_currentEffect = value;
		}
	}

	public static bool ShouldShow
	{
		get
		{
			if (SkiGameManager.Instance.ShowAchievementsGUI || SkiGameManager.Instance.ShowLeaderboardGUI
                 || GUITutorials.Instance.IsShowing || GUITutorials.Instance.AutoShow 
                || SkiGameManager.Instance.AutoStartSkiing || SkiGameManager.Instance.ShowOtherGamesGUI || SkiGameManager.Instance.ShowSettingsGUI || SkiGameManager.Instance.ShowSocialSettingsGUI || SkiGameManager.Instance.ShowFacebookGUI || GUIAchievements.Instance.AutoShow)
			{
				return false;
			}
			return true;
		}
	}

	public bool Showing
	{
		get
		{
			return m_showing;
		}
		set
		{
			if (value)
			{
				if (!m_showing)
				{
					Go.killAllTweensWithTarget(pivot.transform);
					Go.to(pivot.transform, 0.5f, new GoTweenConfig().localPosition(Vector3.zero));
					pivot.transform.localRotation = Quaternion.identity;
					m_showing = true;
					CurrentEffect = Pool.Spawn(showEffectPrefab, base.transform.position, base.transform.rotation);
					UpdateNewState(m_isNew, m_hasOtherNew);
				}
			}
			else if (m_showing)
			{
				Go.killAllTweensWithTarget(pivot.transform);
				Go.to(pivot.transform, 0.5f, new GoTweenConfig().localPosition(hiddenOffset));
				pivot.transform.localRotation = Quaternion.identity;
				if ((bool)newSprite)
				{
					newSprite.SetActive(false);
				}
				m_showing = false;
				CurrentEffect = Pool.Spawn(hideEffectPrefab, base.transform.position, base.transform.rotation);
				StopCoroutine("UpdateWobble");
			}
		}
	}

	public void SnapToTerrain()
	{
		m_hasSnappedToTerrain = false;
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(TerrainLayer.Game);
		if ((bool)terrainForLayer)
		{
			Vector3 position = base.transform.position;
			float y = 0f;
			Vector3 normal = Vector3.up;
			if (terrainForLayer.GetHeightAndNormal(position, ref y, ref normal))
			{
				position.y = y;
				Quaternion rotation = Quaternion.LookRotation(Vector3.forward, normal);
				base.transform.position = position;
				base.transform.rotation = rotation;
				m_hasSnappedToTerrain = true;
			}
		}
	}

	public void UpdateNewState(bool isNew, bool hasOtherNew)
	{
		m_isNew = isNew;
		m_hasOtherNew = hasOtherNew;
		if (m_showing)
		{
			StopCoroutine("UpdateWobble");
			if ((bool)newSprite)
			{
				newSprite.SetActive(isNew);
			}
			if (!isNew && hasOtherNew)
			{
				StartCoroutine("UpdateWobble");
			}
		}
	}

	public void InitialiseShowing()
	{
		if (!m_isNew && ShouldShow)
		{
			SnapShowing(true);
		}
		else
		{
			SnapShowing(false);
		}
	}

	private IEnumerator UpdateWobble()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(1f, 2f));
			Go.killAllTweensWithTarget(pivot.transform);
			pivot.transform.localRotation = Quaternion.identity;
			Go.to(pivot.transform, 2f, new GoTweenConfig().localRotation(wobbleRotation, true).setEaseType(GoEaseType.ElasticPunch));
		}
	}

	public override void Click(Vector3 position)
	{
        if (!SkiGameManager.Instance.isOnline)
        {
            if (isshow)
            {
                if (m_showing && SkiGameManager.Instance.TitleScreenActive)
                {
                    base.Click(position);
                }
            }
        }
	}

	public void SnapShowing(bool showing)
	{
		m_showing = showing;
		if (showing)
		{
			pivot.transform.localPosition = Vector3.zero;
			Go.from(pivot.transform, 0.5f, new GoTweenConfig().localRotation(flipRotation));
			UpdateNewState(m_isNew, m_hasOtherNew);
			return;
		}
		pivot.transform.localPosition = hiddenOffset;
		if ((bool)newSprite)
		{
			newSprite.SetActive(false);
		}
	}

	private void Update()
	{
		if (!m_hasSnappedToTerrain)
		{
			SnapToTerrain();
		}
		if (!SkiGameManager.Instance.Initialising)
		{
			Showing = ShouldShow;
		}
	}

	private void Awake()
	{
        titleSign = this;

        if ((bool)newSprite)
		{
			newSprite.SetActive(false);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((bool)pivot)
		{
			Go.killAllTweensWithTarget(pivot.transform);
		}
		if ((bool)m_currentEffect)
		{
			Pool.Despawn(m_currentEffect.Ref);
		}
	}
}
