using System;
using System.Collections.Generic;
using UnityEngine;

public class StuntManager : MonoBehaviour
{
	public delegate void OnComboChangedDelegate(int combo);

	public static StuntManager Instance;

	public Stunt[] stunts;

	public Sound breakComboSound;

	public OnComboChangedDelegate OnComboChanged;

	private List<Stunt> m_stuntInstances;

	private int m_combo;

	private int m_maxCombo = 1;

	public int Combo
	{
		get
		{
			return m_combo;
		}
	}

	public int MaxCombo
	{
		get
		{
			return m_maxCombo;
		}
	}

	public void AddScore(float points, string description, int comboIncrease)
	{
		if (SkiGameManager.Instance.Finished)
		{
			return;
		}
		if (m_combo < m_maxCombo)
		{
			m_combo = Mathf.Min(m_maxCombo, m_combo + comboIncrease);
			if (OnComboChanged != null)
			{
				OnComboChanged(m_combo);
			}
		}
		SkiGameManager.Instance.AddScore(points, m_combo, description);
	}

	public void AddScore(float points, string description)
	{
		AddScore(points, description, 1);
	}

	public void BreakCombo()
	{
		if (m_combo > 0)
		{
			SoundManager.Instance.PlaySound(breakComboSound);
			SkiGameManager.Instance.BreakCombo();
			m_combo = 0;
			if (OnComboChanged != null)
			{
				OnComboChanged(m_combo);
			}
		}
	}

	public void Reset()
	{
		m_combo = 0;
		m_maxCombo = LevelManager.Instance.CurrentScoreMultiplier.maxCombo;
		foreach (Stunt stuntInstance in m_stuntInstances)
		{
			stuntInstance.OnReset();
		}
	}

	private void OnTakeDamage(Player previousPlayer, Player player)
	{
		BreakCombo();
	}

	private void Awake()
	{
		Instance = this;
		m_stuntInstances = new List<Stunt>(stunts.Length);
		Stunt[] array = stunts;
		foreach (Stunt original in array)
		{
			Stunt stunt = UnityEngine.Object.Instantiate(original);
			stunt.transform.parent = base.transform;
			stunt.Manager = this;
			m_stuntInstances.Add(stunt);
		}
	}

	private void Start()
	{
		Reset();
	}

	private void OnEnable()
	{
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Combine(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnTakeDamage));
		m_combo = 0;
	}

	private void OnDisable()
	{
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Remove(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnTakeDamage));
	}
}
