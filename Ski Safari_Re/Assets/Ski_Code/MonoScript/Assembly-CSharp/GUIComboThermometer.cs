using System;
using UnityEngine;

public class GUIComboThermometer : MonoBehaviour
{
	public static GUIComboThermometer Instance;

	public GameObject pivot;

	public GUIDropShadowText comboText;

	public GameObject[] fills;

	public Renderer[] highlights;

	public GameObject[] frames;

	public GameObject[] breakEffects;

	public int comboOffset = 2;

	public int maxComboLevels = 12;

	public Color comboColorStart;

	public Color comboColorEnd;

	public Material highlightMaterial;

	public Material frozenHighlightMaterial;

	public GUITransitionAnimator transitionAnimator;

	private int m_lastCombo;

	private int m_activeFrameIndex;

	private GoTweenConfig m_comboUpScaleTween = new GoTweenConfig().scale(0.1f, true).setEaseType(GoEaseType.ElasticPunch);

	private GoTweenConfig m_comboUpShakeTween = new GoTweenConfig().shake(new Vector3(0f, 0f, 3f), GoShakeType.Eulers);

	public void OnComboChanged(int combo)
	{
        if(SkiGameManager.Instance.isOnline)
        {
            return;
        }
		if (CreditsSpawner.Instance.EnableSpawning)
		{
			return;
		}
		if (combo >= comboOffset)
		{
			CancelInvoke("Hide");
			transitionAnimator.Show();
			if (m_lastCombo >= comboOffset)
			{
				fills[m_lastCombo - comboOffset].SetActive(false);
			}
			else
			{
				Init();
			}
			fills[combo - comboOffset].SetActive(true);
			highlights[m_activeFrameIndex].material = highlightMaterial;
			comboText.Text = "x" + combo;
			Color color = Color.Lerp(comboColorStart, comboColorEnd, Mathf.Min(1f, (float)(combo - 2) / (float)(maxComboLevels - 2)));
			comboText.GetComponent<Renderer>().material.SetColor("_TintColor", color);
			if (combo > m_lastCombo)
			{
				Go.killAllTweensWithTarget(pivot.transform);
				pivot.transform.localRotation = Quaternion.identity;
				pivot.transform.localScale = Vector3.one;
				Go.to(pivot.transform, 0.5f, m_comboUpScaleTween);
			}
		}
		else if (m_lastCombo >= comboOffset)
		{
			comboText.Text = string.Empty;
			fills[m_lastCombo - comboOffset].SetActive(false);
			highlights[m_activeFrameIndex].material = frozenHighlightMaterial;
			breakEffects[m_lastCombo - comboOffset].SetActive(true);
			Go.killAllTweensWithTarget(pivot.transform);
			pivot.transform.localRotation = Quaternion.identity;
			pivot.transform.localScale = Vector3.one;
			Go.to(pivot.transform, 0.5f, m_comboUpShakeTween);
			Invoke("Hide", 1.5f);
		}
		m_lastCombo = combo;
	}

	private void Hide()
	{
		transitionAnimator.Hide();
	}

	private void Init()
	{
		CancelInvoke("Hide");
		GameObject[] array = fills;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		m_activeFrameIndex = StuntManager.Instance.MaxCombo - comboOffset;
		for (int j = 0; j < frames.Length; j++)
		{
			frames[j].SetActive(j == m_activeFrameIndex);
		}
		GameObject[] array2 = breakEffects;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.SetActive(false);
		}
		comboText.Text = string.Empty;
		m_lastCombo = 0;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		StuntManager instance = StuntManager.Instance;
		instance.OnComboChanged = (StuntManager.OnComboChangedDelegate)Delegate.Combine(instance.OnComboChanged, new StuntManager.OnComboChangedDelegate(OnComboChanged));
	}

	private void OnEnable()
	{
		m_lastCombo = 0;
	}
}
