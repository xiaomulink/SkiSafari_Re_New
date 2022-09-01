using UnityEngine;

public class GUIScore : MonoBehaviour
{
	public TextMesh scoreText;

	public Color scoreTextNormalColor = Color.white;

	public Color scoreTextHighlightColor = Color.yellow;

	public float scoreTextHighlightDuration = 0.5f;

	public float styleTextDisplayDuration = 3f;

	public Vector3 styleScoreStartPos;

	public Vector3 styleScoreMoveAmount;

	public Color comboColorStart;

	public Color comboColorEnd;

	public int maxComboLevels = 12;

	public TextMesh styleText;

	public TextMesh styleScoreText;
	public TextMesh styleScoreMultiplierText;
	public TextMesh styleMeterText;

	private float m_scoreTextHighlightTimer;

	private Material m_scoreTextMaterial;

	private Material m_styleTextMaterial;

	private Color m_styleTextStartColor;

	private Color m_styleTextEndColor;

	private float m_styleTextTimer;

	private float m_currentPoints;

	private int m_lastCombo;

	private float m_lastAddedPoints;

	private GoTweenConfig m_stylePunchTween = new GoTweenConfig().scale(0.1f, true).setEaseType(GoEaseType.ElasticPunch);

	private static bool s_suppressStyleText;

	public static bool SupressStyleText
	{
		get
		{
			return s_suppressStyleText;
		}
		set
		{
			s_suppressStyleText = value;
		}
	}

	public void SetScore(float score, bool highlight)
	{
		int num = Mathf.FloorToInt(score);
		scoreText.text = (num - num % 10).ToString("N0");
		if (highlight)
		{
			m_scoreTextHighlightTimer = scoreTextHighlightDuration;
		}
	}

	public void AddStyle(float points, int combo, string description)
	{
		if (!SupressStyleText && (!(points < m_lastAddedPoints) || !(description != styleText.text)))
		{
			styleText.text = description;
			styleScoreText.text = string.Format("+{0:N0}", points);
			m_styleTextTimer = styleTextDisplayDuration;
			styleText.transform.localScale = Vector3.one;
			styleScoreText.transform.localPosition = styleScoreStartPos;
			m_lastAddedPoints = points;
			m_lastCombo = combo;
			Go.killAllTweensWithTarget(styleScoreText.transform);
			Go.to(styleText.transform, 0.5f, m_stylePunchTween);
			Color color = styleScoreText.GetComponent<Renderer>().material.color;
			color.a = 1f;
			styleScoreText.GetComponent<Renderer>().material.color = color;
			m_currentPoints = points;
		}
	}

	public void ClearStyle()
	{
		m_lastAddedPoints = 0f;
		m_lastCombo = 0;
		m_styleTextTimer = 0f;
		m_lastCombo = 0;
		styleText.text = string.Empty;
		styleScoreText.text = string.Empty;
	}

	public void BreakCombo()
	{
		m_lastAddedPoints = 0f;
		m_lastCombo = 0;
	}

	public void Hide()
	{
		m_lastAddedPoints = 0f;
		m_lastCombo = 0;
		scoreText.text = string.Empty;
		styleText.text = string.Empty;
		styleScoreText.text = string.Empty;
	}

	private void OnEnable()
	{
		ClearStyle();
	}

	private void Start()
	{
		Hide();
		m_scoreTextMaterial = scoreText.GetComponent<Renderer>().material;
		m_styleTextMaterial = styleText.GetComponent<Renderer>().material;
		m_styleTextStartColor = m_styleTextMaterial.color;
		m_styleTextEndColor = m_styleTextStartColor;
		m_styleTextEndColor.a = 0f;
	}

	private void Update()
	{
        try
        {
            styleScoreMultiplierText.text = "X" + SkiGameManager.Instance.customIncreaseMultiple.ToString();
        }catch
        {

        }
        try
        {
            int num = Mathf.FloorToInt(SkiGameManager.Instance.CurrentDistance);

            styleMeterText.text = num+" M";
        }catch
        {

        }
        if (m_scoreTextHighlightTimer > 0f)
		{
			m_scoreTextHighlightTimer = Mathf.Max(0f, m_scoreTextHighlightTimer - Time.deltaTime);
			float t = m_scoreTextHighlightTimer / scoreTextHighlightDuration;
			m_scoreTextMaterial.color = Color.Lerp(scoreTextNormalColor, scoreTextHighlightColor, t);
		}
		if (!(m_styleTextTimer > 0f))
		{
			return;
		}
		if (s_suppressStyleText)
		{
			ClearStyle();
			return;
		}
		m_styleTextTimer -= Time.deltaTime;
		if (m_styleTextTimer <= 0f)
		{
			styleText.text = string.Empty;
			Go.to(styleScoreText.transform, 0.5f, new GoTweenConfig().position(styleScoreMoveAmount, true));
			Color color = styleScoreText.GetComponent<Renderer>().material.color;
			color.a = 0f;
			Go.to(styleScoreText.GetComponent<Renderer>().material, 0.25f, new GoTweenConfig().materialColor(color));
			if (m_lastCombo > 0)
			{
				styleScoreText.text = string.Format("+{0:N0}", m_currentPoints * (float)m_lastCombo);
			}
			m_lastAddedPoints = 0f;
			m_lastCombo = 0;
		}
		else
		{
			float t2 = Mathf.Min(1f, m_styleTextTimer / styleTextDisplayDuration * 2f);
			m_styleTextMaterial.color = Color.Lerp(m_styleTextEndColor, m_styleTextStartColor, t2);
		}
	}
}
