using UnityEngine;

public class UVStepper : MonoBehaviour
{
	public Vector2 step = new Vector2(0.5f, 0f);

	public float interval = 0.5f;

	private Material m_material;

	private float m_timer;

	private void OnEnable()
	{
		m_timer = 0f;
	}

	private void Start()
	{
		m_material = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		m_timer += Time.deltaTime;
		m_material.mainTextureOffset = step * Mathf.Floor(m_timer / interval);
	}
}
