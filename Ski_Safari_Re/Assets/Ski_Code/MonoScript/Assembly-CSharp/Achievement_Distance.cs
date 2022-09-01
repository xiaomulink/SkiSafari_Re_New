using System;
using UnityEngine;

public class Achievement_Distance : Achievement
{
	public enum PersistentMode
	{
		None = 0,
		WithinLevel = 1,
		AcrossLevels = 2
	}

	public enum TravelMode
	{
		Any = 0,
		Ground = 1,
		Air = 2,
		InsideCave = 3,
		AboveCave = 4
	}

	public PersistentMode persistentMode;

	public float requiredDistance = 1f;

	public TravelMode requiredTravelMode;

	public string requiredMountCategory;

	public string requiredAttachmentCategory;

	public bool requiresFrozen;

	public bool resetOnMount;

	public FlamePowerup.Level requiredIgnitionLevel;

	private float m_distance;

	private float m_bestDistance;

	private Vector3 m_lastPosition;

	private bool m_lastPositionValid;

	public override bool IsComplete
	{
		get
		{
			return GameState.GetFloat(base.name) >= requiredDistance;
		}
	}

	protected override void OnComplete()
	{
		m_distance = requiredDistance;
	}

	protected override void OnLoad()
	{
		m_bestDistance = GameState.GetFloat(base.name);
		if (persistentMode == PersistentMode.AcrossLevels)
		{
			m_distance = m_bestDistance;
		}
		else
		{
			m_distance = 0f;
		}
		m_lastPositionValid = false;
	}

	protected override void OnSave()
	{
		GameState.SetFloat(base.name, Mathf.Max(m_distance, m_bestDistance));
	}

	private void OnMount(Player previousPlayer, Player mountedPlayer)
	{
		ClearDistance();
	}

	protected void IncrementDistance()
	{
		Vector3 position = Player.Instance.transform.position;
		if (!m_lastPositionValid)
		{
			m_lastPosition = position;
			m_lastPositionValid = true;
			return;
		}
		float num = Vector3.Distance(m_lastPosition, position);
		m_lastPosition = position;
		m_distance += num;
		if (m_distance >= requiredDistance)
		{
			m_bestDistance = requiredDistance;
			Complete();
		}
		else
		{
			Save();
		}
	}

	protected void ClearDistance()
	{
		m_lastPositionValid = false;
		if (persistentMode == PersistentMode.None)
		{
			if (m_distance > m_bestDistance)
			{
				m_bestDistance = m_distance;
			}
			m_distance = 0f;
		}
	}

	private bool CheckTravelMode(Player player)
	{
		switch (requiredTravelMode)
		{
		case TravelMode.Ground:
			return (bool)player.Collider && player.Collider.OnGround;
		case TravelMode.Air:
			return (bool)player.Collider && !player.Collider.OnGround;
		case TravelMode.AboveCave:
			return Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPointAboveCave(player.transform.position);
		case TravelMode.InsideCave:
			return Terrain.GetTerrainForLayer(TerrainLayer.Game).IsPointInCave(player.transform.position);
		default:
			return true;
		}
	}

	protected virtual void FixedUpdate()
	{
		Player instance = Player.Instance;
		if ((bool)instance && Achievement.CheckMountCategory(instance, requiredMountCategory) && Achievement.CheckAttachmentCategory(instance, requiredAttachmentCategory) && (!requiresFrozen || instance is PlayerSkierFrozen) && CheckTravelMode(instance) && instance.IgnitionLevel >= requiredIgnitionLevel)
		{
			IncrementDistance();
		}
		else
		{
			ClearDistance();
		}
	}

	public override void MigrateToProfile(Profile profile)
	{
		profile.MigrateFloat(base.name);
	}

	public override string ToString()
	{
		if (!IsComplete && requiredDistance > 1f)
		{
			if (persistentMode != 0 || m_distance > 0f)
			{
				int num = Mathf.CeilToInt(requiredDistance - m_distance);
				return string.Format("{0}, {1} metres to go", description, num);
			}
			if (m_bestDistance > 0f)
			{
				int num2 = Mathf.FloorToInt(m_bestDistance);
				return string.Format("{0}, best is {1:0} metres", description, num2);
			}
		}
		return description;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (resetOnMount)
		{
			Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (resetOnMount)
		{
			Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Remove(Player.OnPlayerMount, new Player.OnMountDelegate(OnMount));
		}
	}
}
