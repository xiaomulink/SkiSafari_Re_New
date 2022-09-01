using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("Sprites/WindowSprite")]
public class WindowSprite : MonoBehaviour
{
	public Rect rect;

	public Vector2 size = new Vector2(1f, 1f);

	public Vector2 borderSize = new Vector2(0.2f, 0.2f);

	public Vector2 textureSize = Vector2.zero;

	private MeshFilter m_meshFilter;

	private MeshRenderer m_meshRenderer;

	private Mesh m_mesh;

	private Vector3[] m_vertices = new Vector3[16];

	private Vector2[] m_uv = new Vector2[16];

	private int[] m_triangles = new int[54];

	public Mesh Mesh
	{
		get
		{
			return m_mesh;
		}
	}

	private void UpdateUV(Rect rect)
	{
		if ((bool)m_mesh && (bool)m_meshRenderer.sharedMaterial)
		{
			Texture mainTexture = m_meshRenderer.sharedMaterial.mainTexture;
			if (textureSize == Vector2.zero)
			{
				textureSize.x = mainTexture.width;
				textureSize.y = mainTexture.height;
			}
			float num = rect.x / textureSize.x;
			float num2 = num + rect.width / textureSize.x;
			float num3 = num2 - num;
			float x = num + num3 * 0.5f;
			float num4 = 1f - rect.y / textureSize.y;
			float num5 = num4 - rect.height / textureSize.y;
			float num6 = num4 - num5;
			float y = num5 + num6 * 0.5f;
			m_uv[0] = new Vector2(num, num4);
			m_uv[1] = new Vector2(x, num4);
			m_uv[2] = new Vector2(x, num4);
			m_uv[3] = new Vector2(num2, num4);
			m_uv[4] = new Vector2(num, y);
			m_uv[5] = new Vector2(x, y);
			m_uv[6] = new Vector2(x, y);
			m_uv[7] = new Vector2(num2, y);
			m_uv[8] = new Vector2(num, y);
			m_uv[9] = new Vector2(x, y);
			m_uv[10] = new Vector2(x, y);
			m_uv[11] = new Vector2(num2, y);
			m_uv[12] = new Vector2(num, num5);
			m_uv[13] = new Vector2(x, num5);
			m_uv[14] = new Vector2(x, num5);
			m_uv[15] = new Vector2(num2, num5);
			m_mesh.uv = m_uv;
		}
	}

	public void SetupMesh()
	{
		m_meshFilter = GetComponent<MeshFilter>();
		m_meshRenderer = GetComponent<MeshRenderer>();
		m_mesh = new Mesh();
		m_meshFilter.mesh = m_mesh;
		Texture mainTexture = m_meshRenderer.sharedMaterial.mainTexture;
		if (rect.width == 0f)
		{
			rect.width = mainTexture.width;
		}
		if (rect.height == 0f)
		{
			rect.height = mainTexture.height;
		}
		Vector2 vector = size * 0.5f;
		int num = 0;
		float y = vector.y;
		m_vertices[num++] = new Vector3(0f - vector.x, y, 0f);
		m_vertices[num++] = new Vector3(0f - vector.x + borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x - borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x, y, 0f);
		y = vector.y - borderSize.y;
		m_vertices[num++] = new Vector3(0f - vector.x, y, 0f);
		m_vertices[num++] = new Vector3(0f - vector.x + borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x - borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x, y, 0f);
		y = 0f - vector.y + borderSize.y;
		m_vertices[num++] = new Vector3(0f - vector.x, y, 0f);
		m_vertices[num++] = new Vector3(0f - vector.x + borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x - borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x, y, 0f);
		y = 0f - vector.y;
		m_vertices[num++] = new Vector3(0f - vector.x, y, 0f);
		m_vertices[num++] = new Vector3(0f - vector.x + borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x - borderSize.x, y, 0f);
		m_vertices[num++] = new Vector3(vector.x, y, 0f);
		num = 0;
		int num2 = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				m_triangles[num2++] = num;
				m_triangles[num2++] = num + 1;
				m_triangles[num2++] = num + 4;
				m_triangles[num2++] = num + 4;
				m_triangles[num2++] = num + 1;
				m_triangles[num2++] = num + 5;
				num++;
			}
			num++;
		}
		m_mesh.vertices = m_vertices;
		m_mesh.triangles = m_triangles;
		UpdateUV(rect);
	}

	private void Awake()
	{
		SetupMesh();
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			return;
		}
		Gizmos.color = new Color(1f, 1f, 1f, 0f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, new Vector3(1f, 0f, 1f));
		Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Gizmos.DrawLine(m_vertices[num], m_vertices[num + 1]);
				Gizmos.DrawLine(m_vertices[num + 1], m_vertices[num + 4]);
				Gizmos.DrawLine(m_vertices[num + 4], m_vertices[num]);
				Gizmos.DrawLine(m_vertices[num + 4], m_vertices[num + 1]);
				Gizmos.DrawLine(m_vertices[num + 1], m_vertices[num + 5]);
				Gizmos.DrawLine(m_vertices[num + 5], m_vertices[num + 4]);
				num++;
			}
			num++;
		}
	}
}
