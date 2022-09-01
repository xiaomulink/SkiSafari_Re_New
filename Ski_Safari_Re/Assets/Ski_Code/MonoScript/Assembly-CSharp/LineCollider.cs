using System.Collections.Generic;
using UnityEngine;

public class LineCollider : MonoBehaviour
{
	public class Line
	{
		public Vector3 pointA;

		public Vector3 pointB;
	}

	public TerrainLayer terrainLayer;

	public string category = string.Empty;

	public SurfaceType surfaceType;

	public LayerMask collisionMask = -1;

	public Vector3[] points;

	public bool allowPointCollisions;

	public float bounce;

	public float reflect;

	public float minSpeed;

	public float maxSpeed = 1000f;

	public GameObject breakEffect;

	public float breakEffectRadius = 1f;

	public bool allowSubdivisions;

	public bool isTerrain;

	private List<Line> m_lines;

	public List<Line> Lines
	{
		get
		{
			return m_lines;
		}
	}

	public void BreakLine(GeometryUtils.ContactInfo contactInfo, CircleCollider sourceCollider)
	{
		if (!breakEffect)
		{
			return;
		}
		Vector3 pointA = contactInfo.colliderLine.pointA;
		Vector3 pointB = contactInfo.colliderLine.pointB;
		Vector3 vector = pointB - pointA;
		float magnitude = vector.magnitude;
		vector *= 1f / magnitude;
		Vector3 a = contactInfo.position - contactInfo.normal * sourceCollider.radius;
		float value = Vector3.Distance(a, pointA);
		float num = Mathf.Clamp(value, breakEffectRadius, magnitude - breakEffectRadius);
		Vector3 vector2 = pointA + vector * num;
		Quaternion rotation = Quaternion.LookRotation(upwards: new Vector3(0f - vector.y, vector.x, 0f), forward: Vector3.forward);
		Pool.Spawn(breakEffect, vector2, rotation);
		m_lines.Remove(contactInfo.colliderLine);
		if (allowSubdivisions)
		{
			if (num > breakEffectRadius + float.Epsilon)
			{
				m_lines.Add(new Line
				{
					pointA = pointA,
					pointB = vector2 - vector * breakEffectRadius
				});
			}
			if (num < magnitude - breakEffectRadius - float.Epsilon)
			{
				m_lines.Add(new Line
				{
					pointA = vector2 + vector * breakEffectRadius,
					pointB = pointB
				});
			}
		}
	}
    private void FixedUpdate()
    {
        Debug.DrawLine(points[0], points[points.Length-1], Color.red);

    }
    private void OnEnable()
	{
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);

        if ((bool)terrainForLayer)
		{
			m_lines = new List<Line>(points.Length - 1);
			Vector3 pointA = base.transform.TransformPoint(points[0]);
			for (int i = 0; i < points.Length - 1; i++)
			{
				Vector3 vector = base.transform.TransformPoint(points[i + 1]);
				m_lines.Add(new Line
				{
					pointA = pointA,
					pointB = vector
                
                });
				pointA = vector;
                Debug.DrawLine(pointA, vector, Color.red);
            }
            terrainForLayer.AddStaticLineCollider(this);
		}
	}

	private void OnDisable()
	{
		Terrain terrainForLayer = Terrain.GetTerrainForLayer(terrainLayer);
		if ((bool)terrainForLayer)
		{
			terrainForLayer.RemoveStaticLineCollider(this);
		}
		m_lines = null;
	}
}
