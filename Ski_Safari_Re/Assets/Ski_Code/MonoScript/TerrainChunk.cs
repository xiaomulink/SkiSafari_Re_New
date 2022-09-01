using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Terrain;

public class TerrainChunk : MonoBehaviour
{

    public Mesh mesh;

    public Mesh edgeMesh;

    public Vector3[] points;

    public Vector3[] normals;

    public int startSegmentIndex;

    public int endSegmentIndex;

    public int segmentsPerChunk = 20;

    public int textureScale = 10;

    public float surfaceTextureRatio = 0.5f;

    public Material edgeMaterial;

    public Material material;

    public List<CurveMarker> curveMarkers = new List<CurveMarker>();

    private List<ChunkInfo> m_pooledChunks = new List<ChunkInfo>();

    private void OnGUI()
    {
      
    }

    private Mesh CreateSegmentMesh()
    {
        int num = (segmentsPerChunk + 1) * 3;
        int num2 = segmentsPerChunk * 12;
        Vector3[] vertices = new Vector3[num];
        int[] array = new int[num2];
        Vector2[] array2 = new Vector2[num];
        float num3 = 0f;
        float num4 = 1f / (float)textureScale;
        int num5 = 0;
        for (int i = 0; i <= segmentsPerChunk; i++)
        {
            array2[num5++] = new Vector2(num3, 1f);
            array2[num5++] = new Vector2(num3, surfaceTextureRatio);
            array2[num5++] = new Vector2(num3, 0f);
            num3 += num4;
            if (num3 > 1f)
            {
                num4 = 0f - num4;
            }
            else if (num3 < 0f)
            {
                num4 = 0f - num4;
            }
        }
        int num6 = 0;
        for (int j = 0; j <= segmentsPerChunk - 1; j++)
        {
            int num7 = j * 3;
            array[num6++] = num7;
            array[num6++] = num7 + 3;
            array[num6++] = num7 + 1;
            array[num6++] = num7 + 1;
            array[num6++] = num7 + 3;
            array[num6++] = num7 + 4;
            array[num6++] = num7 + 1;
            array[num6++] = num7 + 4;
            array[num6++] = num7 + 2;
            array[num6++] = num7 + 2;
            array[num6++] = num7 + 4;
            array[num6++] = num7 + 5;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = array;
        mesh.uv = array2;
        return mesh;
    }

    private Mesh CreateEdgeMesh()
    {
        int num = (segmentsPerChunk + 1) * 2;
        int num2 = segmentsPerChunk * 6;
        Vector3[] vertices = new Vector3[num];
        int[] array = new int[num2];
        Vector2[] array2 = new Vector2[num];
        float num3 = 0f;
        float num4 = 1f / (float)textureScale;
        int num5 = 0;
        for (int i = 0; i <= segmentsPerChunk; i++)
        {
            array2[num5++] = new Vector2(num3, 1f);
            array2[num5++] = new Vector2(num3, 0f);
            num3 += num4;
            if (num3 > 1f)
            {
                num4 = 0f - num4;
            }
            else if (num3 < 0f)
            {
                num4 = 0f - num4;
            }
        }
        int num6 = 0;
        for (int j = 0; j <= segmentsPerChunk - 1; j++)
        {
            int num7 = j * 2;
            array[num6++] = num7;
            array[num6++] = num7 + 2;
            array[num6++] = num7 + 1;
            array[num6++] = num7 + 1;
            array[num6++] = num7 + 2;
            array[num6++] = num7 + 3;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = array;
        mesh.uv = array2;
        return mesh;
    }

    private void CreateChunk()
    {
        Vector3[] points = new Vector3[segmentsPerChunk + 1];
        Vector3[] normals = new Vector3[segmentsPerChunk + 1];
        GameObject gameObject = new GameObject("Terrain Chunk");
        gameObject.transform.parent = transform;
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateSegmentMesh();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        GameObject gameObject2 = new GameObject("Edge");
        gameObject2.transform.parent = gameObject.transform;
        MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
        Mesh edgeMesh = (meshFilter2.mesh = CreateEdgeMesh());
        MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
        meshRenderer2.material = edgeMaterial;
        gameObject.SetActive(false);
        m_pooledChunks.Add(new ChunkInfo(gameObject, meshFilter.mesh, edgeMesh, points, normals));
    }
}
