using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("Sprites/RingSprite")]
[RequireComponent(typeof(MeshRenderer))]
public class RingSprite : MonoBehaviour
{
	public float innerRadius = 0.5f;

	public float outerRadius = 1f;

	public float tileDistance = 1f;

	public bool pingPongTile;

	public Rect rect = new Rect(0f, 0f, 0f, 0f);

	public int segmentCount = 64;

	public bool createOnAwake = true;

	private MeshFilter m_meshFilter;

	private MeshRenderer m_meshRenderer;

	private Mesh m_mesh;

	private Vector3[] m_vertices;

	private Vector2[] m_uv;

	private int[] m_triangles;

	public void SetupMesh()
	{
		m_meshFilter = GetComponent<MeshFilter>();
		m_meshRenderer = GetComponent<MeshRenderer>();
		m_mesh = new Mesh();
		m_meshFilter.mesh = m_mesh;
		int num = segmentCount * 2 + 2;
		int num2 = (segmentCount + 1) * 2;
		m_vertices = new Vector3[num];
		m_uv = new Vector2[num];
		m_triangles = new int[num2];
		int num3 = 0;
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = innerRadius;
		num3++;
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = outerRadius;
		num3++;
		float num4 = (float)Math.PI * 2f / (float)segmentCount;
		float num5 = num4;
		for (int i = 1; i < segmentCount; i++)
		{
			float num6 = Mathf.Cos(num5);
			float num7 = Mathf.Sin(num5);
			m_vertices[num3].x = num7 * innerRadius;
			m_vertices[num3].z = num6 * innerRadius;
			num3++;
			m_vertices[num3].x = num7 * outerRadius;
			m_vertices[num3].z = num6 * outerRadius;
			num3++;
			num5 += num4;
		}
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = innerRadius;
		num3++;
		m_vertices[num3].x = 0f;
		m_vertices[num3].z = outerRadius;
		num3++;
		for (int j = 0; j < num2; j++)
		{
			m_triangles[j] = j;
		}
		m_mesh.vertices = m_vertices;
		m_mesh.SetTriangles(m_triangles, 0);
		Texture mainTexture = m_meshRenderer.sharedMaterial.mainTexture;
		float num8 = mainTexture.width;
		float num9 = mainTexture.height;
		Rect rect = this.rect;
		if (rect.width == 0f)
		{
			rect.width = num8;
		}
		else if (rect.width > 0f && rect.width <= 1f)
		{
			rect.width *= num8;
		}
		if (rect.height == 0f)
		{
			rect.height = num9;
		}
		else if (rect.height > 0f && rect.height <= 1f)
		{
			rect.height *= num9;
		}
		float num10 = rect.x / num8;
		float num11 = num10 + rect.width / num8;
		float num12 = 1f - rect.y / num9;
		float y = num12 - rect.height / num9;
		int num13 = 0;
		float num14 = (float)Math.PI * 2f * outerRadius * (num11 - num10) / (float)segmentCount;
		if (tileDistance > 1f)
		{
			num14 /= tileDistance;
		}
		else if (tileDistance > 0f)
		{
			num14 = (num11 - num10) / ((float)segmentCount * tileDistance);
		}
		float num15 = num10;
		for (int k = 0; k <= segmentCount; k++)
		{
			m_uv[num13++] = new Vector2(num15, y);
			m_uv[num13++] = new Vector2(num15, num12);
			num15 += num14;
			if (pingPongTile)
			{
				if (num15 > num11)
				{
					num15 = num11 * 2f - num15;
					num14 = 0f - num14;
				}
				else if (num15 < num10)
				{
					num15 = num10 * 2f - num15;
					num14 = 0f - num14;
				}
			}
		}
		m_mesh.uv = m_uv;
		m_mesh.RecalculateBounds();
	}

	private void Awake()
	{
		if (createOnAwake || !Application.isPlaying)
		{
			SetupMesh();
		}
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
