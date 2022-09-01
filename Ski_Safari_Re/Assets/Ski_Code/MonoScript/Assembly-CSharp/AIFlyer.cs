using UnityEngine;

public class AIFlyer : AI
{
	public TerrainLayer terrainLayer;

	public float heightAboveTerrain = 10f;

	public float heightFilter = 2f;

	public float diveRecoveryAngle = 90f;

	private Flyer m_flyer;

	private Vector3 m_debugTargetPos;

	private void AvoidTerrain()
	{
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if (!terrainForLayer)
		{
			return;
		}
		float height = 0f;
		Vector3 normal = Vector3.zero;
		if (!terrainForLayer.GetHeightAndNormal(base.transform.position, ref height, ref normal))
		{
			return;
		}
		if (base.transform.position.y - height < heightAboveTerrain)
		{
			float num = Mathf.Acos(Vector3.Dot(base.transform.right, normal)) * 57.29578f;
			if (num > diveRecoveryAngle)
			{
				m_flyer.LiftInput = 1f;
			}
			else
			{
				m_flyer.LiftInput = 0f;
			}
		}
		else
		{
			m_flyer.LiftInput = 0f;
		}
	}

	private void FlyToTarget(Vector3 targetPos)
	{
		Vector3 rhs = targetPos - base.transform.position;
		float magnitude = rhs.magnitude;
		if (magnitude > Mathf.Epsilon)
		{
			rhs *= 1f / magnitude;
			float num = 57.29578f * Mathf.Acos(Vector3.Dot(base.transform.forward, rhs)) * Mathf.Sign(Vector3.Dot(base.transform.up, rhs));
			if (num > 0f)
			{
				m_flyer.LiftInput = 1f;
			}
			else
			{
				m_flyer.LiftInput = 0f;
			}
			m_flyer.ThrottleInput = 1f;
		}
		else
		{
			m_flyer.ThrottleInput = 0f;
		}
		m_debugTargetPos = targetPos;
	}

	private void Awake()
	{
		m_flyer = GetComponent<Flyer>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		AvoidTerrain();
	}

	private void OnDrawGizmos()
	{
		if (m_debugTargetPos != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, m_debugTargetPos);
		}
	}
}
