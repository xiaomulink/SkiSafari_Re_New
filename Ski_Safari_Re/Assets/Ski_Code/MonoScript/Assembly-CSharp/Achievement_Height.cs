using UnityEngine;

public class Achievement_Height : Achievement
{
	public float requiredHeight = 1f;

	public string requiredMountCategory;

	private float m_bestHeight;

	private float m_currentHeight;

	public override bool IsComplete
	{
		get
		{
			return GameState.GetFloat(base.name) >= requiredHeight;
		}
	}

	protected override void OnComplete()
	{
		m_bestHeight = requiredHeight;
	}

	protected override void OnLoad()
	{
		m_bestHeight = GameState.GetFloat(base.name);
	}

	protected override void OnSave()
	{
		GameState.SetFloat(base.name, m_bestHeight);
	}

	protected void FixedUpdate()
	{
		Player instance = Player.Instance;
		m_currentHeight = 0f;
		if (!instance || !Achievement.CheckMountCategory(instance, requiredMountCategory) || instance.Collider.OnGround)
		{
			return;
		}
		Vector3 position = instance.transform.position;
		float height = 0f;
		if (!Terrain.GetTerrainForLayer(TerrainLayer.Game).GetHeight(position.x, ref height))
		{
			return;
		}
		m_currentHeight = position.y - height;
		if (m_currentHeight > m_bestHeight)
		{
			m_bestHeight = m_currentHeight;
			if (m_bestHeight >= requiredHeight)
			{
				Complete();
			}
			else
			{
				Save();
			}
		}
	}

	public override void MigrateToProfile(Profile profile)
	{
		profile.MigrateFloat(base.name);
	}

	public override string ToString()
	{
		if (!IsComplete)
		{
			if (m_currentHeight >= 1f)
			{
				int num = Mathf.CeilToInt(requiredHeight - m_currentHeight);
				return string.Format("{0}, {1} metres to go", description, num);
			}
			if (m_bestHeight >= 1f)
			{
				int num2 = Mathf.FloorToInt(m_bestHeight);
				return string.Format("{0}, best is {1} metres", description, num2);
			}
		}
		return description;
	}
}
