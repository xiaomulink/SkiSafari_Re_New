using System;
using UnityEngine;

public class StartingSkier : Item
{
	public static StartingSkier Instance;

	public GameObject animal;

	public Vector3 animalOffset;

	public float animalSpawnDelay;

	public Player skier;

	public float spawnDelay = 2.8f;

	public SpawnParams npcSpawnParams;

	public GUIStartingSkier guiStartingSkier;

	public GUITitleSign signPrefab;

	public Vector3 signPosition = new Vector3(6f, -1f, 1f);

	public Quaternion signRotation = Quaternion.identity;

	private GUITitleSign m_guiSign;

    public GUITitleSign GuiSign;

	public void UpdateSign()
	{
		if (!m_guiSign)
		{
			CreateSign();
		}
		else
		{
			UpdateSignTransform();
		}
		if ((bool)m_guiSign)
		{
			m_guiSign.SnapToTerrain();
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
	}

	private void CreateSign()
	{
		if (ItemManager.Instance.GetItemSet("starting_skier").UnlockedItemCount > 1)
		{

			m_guiSign = UnityEngine.Object.Instantiate(signPrefab);
			m_guiSign.transform.parent = base.transform;
			UpdateSignTransform();
			m_guiSign.SnapShowing(GUITitleSign.ShouldShow);
            GuiSign = m_guiSign;
            GUITitleSign guiSign = m_guiSign;
			guiSign.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(guiSign.OnClick, new GUIButton.OnClickDelegate(SelectNextStartingSkier));
		}
	}

	private void UpdateSignTransform()
	{
		Vector3 vector = ((!Slope.Instance) ? Vector3.zero : Slope.Instance.startingSkierSignOffset);
		m_guiSign.transform.position = signPosition + vector;
		m_guiSign.transform.rotation = signRotation;
		m_guiSign.SnapToTerrain();
	}

	private void UpdateSignNewState()
	{
		if ((bool)m_guiSign)
		{
			m_guiSign.UpdateNewState(!base.HasBeenUsed, ItemManager.Instance.GetItemSet("starting_skier").UnusedItemCount > 0);
		}
	}

	private void SelectNextStartingSkier()
	{
		if (ItemManager.Instance.GetItemSet("starting_skier").SelectNextItem(false))
		{
			SoundManager.Instance.PlaySound(Slope.Instance.GetStartingSkier().selectSound);
			AchievementManager.Instance.ClearActiveAchievements();
			AchievementManager.Instance.PopulateActiveAchievements();
		}
	}

	private void Start()
	{
		UpdateSignNewState();
	}

	protected override void OnEnable()
	{
		Instance = this;
		CreateSign();
	}

	protected override void OnDisable()
	{
		if ((bool)m_guiSign)
		{
			UnityEngine.Object.Destroy(m_guiSign.gameObject);
		}
		Instance = null;
	}
}
