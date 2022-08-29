using System;
using UnityEngine;

[AddComponentMenu("Sprites/CircleSprite")]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class CircleSprite : MonoBehaviour
{
	public enum CoordinateMode
	{
		Rectangular = 0,
		Polar = 1
	}

	public float radius = 1f;

	public Rect rect = new Rect(0f, 0f, 0f, 0f);

	public CoordinateMode coordinateMode;

	public float tiling = 1f;

	public int segmentCount = 64;

	private MeshFilter m_meshFilter;

	private MeshRenderer m_meshRenderer;

	private Mesh m_mesh;

	private Vector3[] m_vertices;

	private Vector2[] m_uv;

	private int[] m_triangles;

	private void SetupMesh()
	{
		m_meshFilter = GetComponent<MeshFilter>();
		m_meshRenderer = GetComponent<MeshRenderer>();
		m_mesh = new Mesh();
		m_meshFilter.mesh = m_mesh;
		switch (coordinateMode)
		{
		case CoordinateMode.Rectangular:
			SetupRectangularCoordinateMesh();
			break;
		case CoordinateMode.Polar:
			SetupPolarCoordinateMesh();
			break;
		}
	}

	private void SetupPolarCoordinateMesh()
	{
		int num = segmentCount * 2 + 2;
		int num2 = segmentCount * 6;
		m_vertices = new Vector3[num];
		m_uv = new Vector2[num];
		m_triangles = new int[num2];
		int num3 = 0;
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = 0f;
		num3++;
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = radius;
		num3++;
		float num4 = (float)Math.PI * 2f / (float)segmentCount;
		float num5 = num4;
		for (int i = 1; i < segmentCount; i++)
		{
			float num6 = Mathf.Cos(num5);
			float num7 = Mathf.Sin(num5);
			m_vertices[num3].x = 0f;
			m_vertices[num3].z = 0f;
			num3++;
			m_vertices[num3].x = num7 * radius;
			m_vertices[num3].z = num6 * radius;
			num3++;
			num5 += num4;
		}
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = 0f;
		num3++;
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = radius;
		num3++;
		num3 = 0;
		int num8 = 0;
		for (int j = 0; j < segmentCount; j++)
		{
			m_triangles[num8++] = num3;
			m_triangles[num8++] = num3 + 1;
			m_triangles[num8++] = num3 + 2;
			m_triangles[num8++] = num3 + 2;
			m_triangles[num8++] = num3 + 1;
			m_triangles[num8++] = num3 + 3;
			num3 += 2;
		}
		m_mesh.vertices = m_vertices;
		m_mesh.triangles = m_triangles;
		Texture mainTexture = m_meshRenderer.sharedMaterial.mainTexture;
		float width = mainTexture.width;
		float num9 = mainTexture.height;
		if (rect.width == 0f)
		{
			rect.width = width;
		}
		if (rect.height == 0f)
		{
			rect.height = num9;
		}
		int num10 = 0;
		float num11 = (float)Math.PI * 2f / tiling / (float)segmentCount;
		float num12 = 0f;
		for (int k = 0; k <= segmentCount; k++)
		{
			m_uv[num10++] = new Vector2(num12, rect.y / num9);
			m_uv[num10++] = new Vector2(num12, (rect.y + rect.height) / num9);
			num12 += num11;
		}
		m_mesh.uv = m_uv;
	}

	private void SetupRectangularCoordinateMesh()
	{
		int num = segmentCount + 1;
		int num2 = segmentCount * 3;
		m_vertices = new Vector3[num];
		m_uv = new Vector2[num];
		m_triangles = new int[num2];
		Texture mainTexture = m_meshRenderer.sharedMaterial.mainTexture;
		float num3 = mainTexture.width;
		float num4 = mainTexture.height;
		if (rect.width == 0f)
		{
			rect.width = num3;
		}
		if (rect.height == 0f)
		{
			rect.height = num4;
		}
		float num5 = rect.x / num3;
		float num6 = num5 + rect.width / num3;
		float num7 = (num6 + num5) * 0.5f;
		float num8 = (num6 - num5) * 0.5f;
		float num9 = 1f - rect.y / num4;
		float num10 = num9 - rect.height / num4;
		float num11 = (num9 + num10) * 0.5f;
		float num12 = (num9 - num10) * 0.5f;
		int num13 = 0;
		m_vertices[num13].x = 0f;
		m_vertices[num13].z = 0f;
		m_uv[num13] = new Vector2(num7, num11);
		num13++;
		m_vertices[num13].x = 0f;
		m_vertices[num13].z = radius;
		m_uv[num13] = new Vector2(num7, num11 + num12);
		num13++;
		float num14 = (float)Math.PI * 2f / (float)segmentCount;
		float num15 = num14;
		for (int i = 1; i < segmentCount; i++)
		{
			float num16 = Mathf.Cos(num15);
			float num17 = Mathf.Sin(num15);
			m_vertices[num13].x = num17 * radius;
			m_vertices[num13].z = num16 * radius;
			m_uv[num13] = new Vector2(num7 + num17 * num8, num11 + num16 * num12);
			num13++;
			num15 += num14;
		}
		num13 = 1;
		int num18 = 0;
		for (int j = 0; j < segmentCount - 1; j++)
		{
			m_triangles[num18++] = 0;
			m_triangles[num18++] = num13++;
			m_triangles[num18++] = num13;
		}
		m_triangles[num18++] = 0;
		m_triangles[num18++] = num13;
		m_triangles[num18++] = 1;
		m_mesh.vertices = m_vertices;
		m_mesh.triangles = m_triangles;
		m_mesh.uv = m_uv;
	}

	private void Awake()
	{
		SetupMesh();
	}

	private void OnDrawGizmosSelected()
	{
		if (m_vertices != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
			for (int i = 0; i < m_vertices.Length - 1; i++)
			{
				Gizmos.DrawLine(m_vertices[i], m_vertices[i + 1]);
			}
		}
	}
}
