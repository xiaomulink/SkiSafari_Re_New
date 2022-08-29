using UnityEngine;

public class GUIDropShadowText : MonoBehaviour
{
	public Vector3 offset = new Vector3(0.075f, -0.05f, 0.25f);

	public Material material;

	private string m_text;

	private TextMesh m_textMesh;

	private TextMesh m_shadowTextMesh;

	private float m_initialCharacterSize;

	private float m_textScale = 1f;

	public string Text
	{
		get
		{
			return m_text;
		}
		set
		{
			m_text = value;
			if ((bool)m_textMesh)
			{
				m_textMesh.text = value;
				m_shadowTextMesh.text = value;
			}
		}
	}

	public float TextScale
	{
		get
		{
			return m_textScale;
		}
		set
		{
			m_textScale = value;
			if ((bool)m_textMesh)
			{
				m_textMesh.characterSize = m_textScale * m_initialCharacterSize;
				m_shadowTextMesh.characterSize = m_textScale * m_initialCharacterSize;
			}
		}
	}

	public TextMesh TextMesh
	{
		get
		{
			return m_textMesh;
		}
	}

	protected void Awake()
	{
		m_textMesh = GetComponent<TextMesh>();
		if (!string.IsNullOrEmpty(m_text))
		{
			m_textMesh.text = m_text;
		}
		else
		{
			m_text = m_textMesh.text;
		}
		GameObject gameObject = new GameObject("Shadow");
		gameObject.layer = base.gameObject.layer;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = offset;
		gameObject.transform.localRotation = Quaternion.identity;
		m_shadowTextMesh = gameObject.AddComponent<TextMesh>();
		MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
		m_shadowTextMesh.text = m_textMesh.text;
		m_shadowTextMesh.characterSize = m_textMesh.characterSize;
		m_shadowTextMesh.lineSpacing = m_textMesh.lineSpacing;
		m_shadowTextMesh.anchor = m_textMesh.anchor;
		m_shadowTextMesh.alignment = m_textMesh.alignment;
		m_shadowTextMesh.tabSize = m_textMesh.tabSize;
		m_shadowTextMesh.font = m_textMesh.font;
		component.material = material;
		m_initialCharacterSize = m_textMesh.characterSize;
	}
}
