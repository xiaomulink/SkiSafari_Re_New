using System;
using UnityEngine;

public class Hurdle : MonoBehaviour
{
	public delegate void OnJumpDelegate(Player player, Hurdle hurdle, bool insideCave);

	public string category = string.Empty;

	public bool canBeInsideCaves = true;

	public Vector3 line1Start;

	public Vector3 line1End;

	public Vector3 line2Start;

	public Vector3 line2End;

	public static OnJumpDelegate OnJump;

	private Vector3 line1Start_world;

	private Vector3 line1End_world;

	private Vector3 line2Start_world;

	private Vector3 line2End_world;

	private Vector3 m_lastPos;

	private bool m_inProgress;

	private bool m_insideCave;

	private void StartProgress(bool playerInCave)
	{
		if (!m_inProgress)
		{
			m_inProgress = true;
			m_insideCave = playerInCave;
			Player.OnPlayerLand = (Player.SimplePlayerDelegate)Delegate.Combine(Player.OnPlayerLand, new Player.SimplePlayerDelegate(OnLand));
		}
	}

	private void StopProgress()
	{
		if (m_inProgress)
		{
			m_inProgress = false;
			Player.OnPlayerLand = (Player.SimplePlayerDelegate)Delegate.Remove(Player.OnPlayerLand, new Player.SimplePlayerDelegate(OnLand));
		}
	}

	protected void OnLand(Player player)
	{
		StopProgress();
	}

	private void OnEnable()
	{
		line1Start_world = base.transform.TransformPoint(line1Start);
		line1End_world = base.transform.TransformPoint(line1End);
		line2Start_world = base.transform.TransformPoint(line2Start);
		line2End_world = base.transform.TransformPoint(line2End);
	}

	private void FixedUpdate()
	{
		if ((bool)Player.Instance)
		{
			Vector3 position = Player.Instance.transform.position;
			if (OnJump != null && position.x > m_lastPos.x)
			{
				if (!m_inProgress)
				{
					if (!Player.Instance.Collider.OnGround)
					{
						if (canBeInsideCaves)
						{
							Terrain terrainForLayer = Terrain.GetTerrainForLayer(TerrainLayer.Game);
							bool flag = terrainForLayer.IsPointInCave(Player.Instance.transform.position);
							bool flag2 = terrainForLayer.IsPointInCave(base.transform.position);
							if (flag == flag2)
							{
								Vector3 intersection = Vector3.zero;
								if (GeometryUtils.LinePointIntersection2D(m_lastPos, position, line1Start_world, line1End_world, ref intersection))
								{
									StartProgress(flag);
								}
							}
						}
						else
						{
							Vector3 intersection2 = Vector3.zero;
							if (GeometryUtils.LinePointIntersection2D(m_lastPos, position, line1Start_world, line1End_world, ref intersection2))
							{
								StartProgress(false);
							}
						}
					}
				}
				else
				{
					Vector3 intersection3 = Vector3.zero;
					if (GeometryUtils.LinePointIntersection2D(m_lastPos, position, line2Start_world, line2End_world, ref intersection3))
					{
						StopProgress();
						OnJump(Player.Instance, this, m_insideCave);
					}
				}
			}
			m_lastPos = position;
		}
		else
		{
			StopProgress();
			m_lastPos = Vector3.zero;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.TransformPoint(line1Start), base.transform.TransformPoint(line1End));
		Gizmos.DrawLine(base.transform.TransformPoint(line2Start), base.transform.TransformPoint(line2End));
	}
}
