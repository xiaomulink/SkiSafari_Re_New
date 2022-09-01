using UnityEngine;

public class FXBoostTrail : MonoBehaviour
{
	public float lifeTime = 1f;

	public int maxSegments = 50;

	public float minSegmentDistance = 0.25f;

	public float startWidth = 1f;

	public float endWidth;

	public Color startColor = Color.white;

	public Color endColor = Color.black;

	public Material material;

	private LineRenderer m_lineRenderer;

	private Vector3[] m_positions;

	private float m_minSegmentDistanceSqr;

	private float m_timer;

	private int m_currentSegment;

	private Vector3 m_lastPosition;

	public void Clear()
	{
		m_currentSegment = -1;
		GenerateSegment(base.transform.position);
	}

	private void Awake()
	{
		m_lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		m_lineRenderer.SetVertexCount(maxSegments);
		m_lineRenderer.SetWidth(startWidth, endWidth);
		m_lineRenderer.SetColors(startColor, endColor);
		m_lineRenderer.material = material;
		m_positions = new Vector3[maxSegments];
		m_minSegmentDistanceSqr = minSegmentDistance * minSegmentDistance;
	}

	private void OnEnable()
	{
		m_timer = 0f;
		m_currentSegment = -1;
		GenerateSegment(base.transform.position);
	}

	private void Update()
	{
		if (lifeTime > 0f)
		{
			m_timer += Time.deltaTime;
			if (!(m_timer < lifeTime))
			{
				base.gameObject.SetActive(false);
				return;
			}
			startColor.a = 1f - m_timer / lifeTime;
			endColor.a = startColor.a;
			m_lineRenderer.SetColors(startColor, endColor);
		}
		Vector3 position = base.transform.position;
		if ((position - m_lastPosition).sqrMagnitude >= m_minSegmentDistanceSqr)
		{
			GenerateSegment(position);
		}
		else
		{
			m_positions[m_currentSegment] = position;
		}
	}

	private void GenerateSegment(Vector3 position)
	{
		m_currentSegment++;
		if (m_currentSegment >= maxSegments)
		{
			int num = maxSegments - 1;
			for (int i = 0; i < num; i++)
			{
				m_positions[i] = m_positions[i + 1];
			}
			m_currentSegment = maxSegments - 1;
		}
		else
		{
			m_lineRenderer.SetVertexCount(m_currentSegment + 1);
		}
		m_positions[m_currentSegment] = position;
		int num2 = m_currentSegment;
		for (int j = 0; j <= m_currentSegment; j++)
		{
			m_lineRenderer.SetPosition(j, m_positions[num2]);
			num2--;
		}
		m_lastPosition = position;
	}
}
