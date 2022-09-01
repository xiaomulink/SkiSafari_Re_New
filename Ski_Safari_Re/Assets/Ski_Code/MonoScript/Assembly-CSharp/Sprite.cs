using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Sprites/Sprite")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Sprite : MonoBehaviour
{
	public Rect rect;

	public Mesh bakedMesh;

	private MeshFilter m_meshFilter;

	private MeshRenderer m_meshRenderer;

	private Mesh m_mesh;

	private Vector3[] m_vertices;

	private Vector2[] m_uv;

	private int[] m_triangles;

	private float m_textureWidth;

	private float m_textureHeight;

	private static Vector3[] s_vertices;

	private static int[] s_triangles;

	protected MeshFilter MeshFilter
	{
		get
		{
			if (m_meshFilter != null)
			{
				return m_meshFilter;
			}
			m_meshFilter = GetComponent<MeshFilter>();
			return m_meshFilter;
		}
	}

	protected float TextureWidth
	{
		get
		{
			return m_textureWidth;
		}
	}

	protected float TextureHeight
	{
		get
		{
			return m_textureHeight;
		}
	}

	protected void UpdateUVs(Rect newRect)
	{
		if (m_uv != null)
		{
			if (newRect.width == 0f)
			{
				newRect.width = m_textureWidth;
			}
			if (newRect.height == 0f)
			{
				newRect.height = m_textureHeight;
			}
			float num = newRect.x / m_textureWidth;
			float uMax = num + newRect.width / m_textureWidth;
			float num2 = 1f - newRect.y / m_textureHeight;
			float vMin = num2 - newRect.height / m_textureHeight;
			UpdateUVs(ref m_uv, num, uMax, vMin, num2);
			m_mesh.uv = m_uv;
		}
	}

	protected virtual void UpdateUVs(ref Vector2[] uvs, float uMin, float uMax, float vMin, float vMax)
	{
		uvs[0].x = uMin;
		uvs[0].y = vMax;
		uvs[1].x = uMin;
		uvs[1].y = vMin;
		uvs[2].x = uMax;
		uvs[2].y = vMax;
		uvs[3].x = uMax;
		uvs[3].y = vMin;
	}

	private void SetupMesh()
	{
		m_meshFilter = GetComponent<MeshFilter>();
		m_meshRenderer = GetComponent<MeshRenderer>();
		if ((bool)bakedMesh)
		{
			return;
		}
		m_mesh = new Mesh();
		m_mesh.name = "Temporary Mesh";
		m_meshFilter.mesh = m_mesh;
		if ((bool)m_meshRenderer.sharedMaterial)
		{

           
            Texture mainTexture = m_meshRenderer.sharedMaterial.mainTexture;
            if ((bool)mainTexture)
			{
				m_textureWidth = mainTexture.width;
				m_textureHeight = mainTexture.height;
			}
		}else
        {
            Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
            m_meshRenderer.sharedMaterial = mat;
        }
		if (s_vertices == null)
		{
			s_vertices = new Vector3[4];
			s_vertices[0] = new Vector3(-0.5f, 0.5f, 0f);
			s_vertices[1] = new Vector3(-0.5f, -0.5f, 0f);
			s_vertices[2] = new Vector3(0.5f, 0.5f, 0f);
			s_vertices[3] = new Vector3(0.5f, -0.5f, 0f);
		}
		if (s_triangles == null)
		{
			s_triangles = new int[6];
			s_triangles[0] = 0;
			s_triangles[1] = 2;
			s_triangles[2] = 1;
			s_triangles[3] = 1;
			s_triangles[4] = 2;
			s_triangles[5] = 3;
		}
		m_vertices = s_vertices;
		m_triangles = s_triangles;
		m_uv = new Vector2[4];
		m_mesh.vertices = m_vertices;
		m_mesh.triangles = m_triangles;
		UpdateUVs(rect);
		m_mesh.uv = m_uv;
		m_mesh.RecalculateBounds();
	}

	public void EditorUpdate()
	{
		if ((bool)bakedMesh)
		{
			bakedMesh = null;
		}
		SetupMesh();
	}

	protected virtual void Awake()
	{
		if (!m_mesh)
		{
			SetupMesh();
		}
	}

	private void OnDestroy()
	{
	}
}
