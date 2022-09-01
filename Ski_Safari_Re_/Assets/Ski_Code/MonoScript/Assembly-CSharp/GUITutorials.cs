using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GUITutorials : MonoBehaviour
{
	public enum Category
	{
		None = 0,
		StartingSkier = 1,
		Trail = 2,
		TrailUnlock = 3
	}

	[Serializable]
	public class PageSet
	{
		public string name;

		public Category category;

		public string[] pageNames;
	}

	[CompilerGenerated]
	private sealed class _003CFilterPages_003Ec__AnonStorey1F
	{
		internal PageSet pageSet;

		internal bool _003C_003Em__16(PageSet p)
		{
			return p.category == pageSet.category;
		}
	}

	public static GUITutorials Instance;

	public GUIDropShadowText headingText;

	public string[] categoryHeadings;

	public Renderer pageSprite;

	public PageSet[] pageSets;

	public GUITutorialPageButton leftButton;

	public GUITutorialPageButton rightButton;

	public GUIButton closeButton;

	public Sound closeButtonSound;

	public GUITransitionAnimator transitionAnimator;

	public GUIPageBlip pageBlipPrefab;

	public Vector3 pageBlipOffset;

	public float pageBlipSeparation = 1f;

	public float autoShowDelay = 5f;

	public GUIUpdateNews[] updateNewsList;

	public GUIRatingPopup ratingPopup;

	public GUIProfileBackupPopup profileBackupPopup;

	public GUIProfileSyncPopup profileSyncPopup;

	public GUIProfileSelectorPopup profileSelectorPopup;

	public GUILicenseCheckPopup licenseCheckPopup;

	private List<PageSet> m_pageSetOverrides = new List<PageSet>();

	private List<string> m_activePages = new List<string>();

	public int m_currentPageIndex;

	private GUIPageBlip[] m_pageBlips;

	private bool m_shownLastPage;

	private bool m_autoShow=false;

	private Category m_currentCategory;

	public string UpdateVersion
	{
		get
		{
			if (updateNewsList.Length > 0)
			{
				return updateNewsList[updateNewsList.Length - 1].version;
			}
			return "1.0";
		}
	}

	public bool AutoShow
	{
		get
		{
			return false;
		}
	}

	public bool IsShowing
	{
		get
		{
			return false;
		}
	}

	public void AddPageSetOverride(PageSet pageSet)
	{
		m_pageSetOverrides.Add(pageSet);
	}

	public void RemovePageSetOverride(PageSet pageSet)
	{
		m_pageSetOverrides.Remove(pageSet);
	}

	public static string GetCategoryKey(Category category)
	{
		return "tut_" + category;
	}

	public void MoveRight()
	{
		m_currentPageIndex++;
		UpdatePage();
	}

	public void MoveLeft()
	{
		m_currentPageIndex--;
		UpdatePage();
	}

	public void SelectPage(int pageIndex)
	{
		m_currentPageIndex = Mathf.Clamp(pageIndex, 0, m_activePages.Count);
		if (transitionAnimator.IsShowing)
		{
			UpdatePage();
		}
	}

	public void Show()
	{
		m_currentCategory = Category.None;
		m_autoShow = false;
		DoShow();
	}

	public void ShowCategory(Category category)
	{
		m_currentCategory = category;
		m_autoShow = false;
		DoShow();
	}

	private bool ShowCategoryIfNotViewed(Category category)
	{
		if (GameState.HasKey(GetCategoryKey(category)))
		{
			return false;
		}
		m_autoShow = true;
		m_currentCategory = category;
		CancelInvoke("DoShow");
		if (SkiGameManager.Instance.Initialising)
		{
			Invoke("DoShow", autoShowDelay);
		}
		else
		{
			Invoke("DoShow", transitionAnimator.transitionOutTime);
		}
		return true;
	}

	private void DoShow()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		transitionAnimator.Show();
		if (!m_autoShow)
		{
			SkiGameManager.Instance.OnTutorialsShowing();
		}
		m_shownLastPage = false;
		if (m_autoShow)
		{
			closeButton.gameObject.SetActive(false);
		}
		FilterPages(m_currentCategory);
		UpdatePage();
	}

	public void Hide()
	{
		if (transitionAnimator.IsShowing)
		{
			if (m_currentCategory != 0)
			{
				UpdateAutoShow();
			}
			transitionAnimator.Hide();
			AssetManager.UnloadTexture(pageSprite);
			if (!m_autoShow)
			{
				SkiGameManager.Instance.OnTutorialsHiding();
			}
		}
	}

	public void UpdateAutoShow()
	{
        return;
		m_autoShow = false;
		if (ShowCategoryIfNotViewed(Category.None))
		{
			return;
		}
		GUIUpdateNews[] array = updateNewsList;
		foreach (GUIUpdateNews gUIUpdateNews in array)
		{
			int num = GameState.GetString("last_update_version").CompareTo(gUIUpdateNews.version);
			if (num < 0)
			{
				m_autoShow = true;
				UnityEngine.Object.Instantiate(gUIUpdateNews);
				GameState.SetString("last_update_version", gUIUpdateNews.version);
				int num2 = 1000;
				int num3 = Mathf.FloorToInt((float)GameState.CoinCount / (float)num2) * num2;
				AnalyticsManager.Instance.SendEvent("updated", "version", gUIUpdateNews.version, "coins", num3.ToString(), "rank", LevelManager.Instance.CurrentLevel.ToString());
				return;
			}
		}
		if ((bool)GUISocialSettings.Instance && GUISocialSettings.Instance.ShouldAutoShow())
		{
			m_autoShow = true;
			GUISocialSettings.Instance.transitionAnimator.Show();
		}
		else if ((bool)profileSelectorPopup && profileSelectorPopup.ShouldShow())
		{
			m_autoShow = true;
			UnityEngine.Object.Instantiate(profileSelectorPopup);
			AnalyticsManager.Instance.SendEvent("profile_selector_popup_show");
		}
		else if (licenseCheckPopup.ShouldShow())
		{
			m_autoShow = true;
			UnityEngine.Object.Instantiate(licenseCheckPopup);
			AnalyticsManager.Instance.SendEvent("license_popup_show");
		}
		else if ((ItemManager.Instance.GetItemSet("starting_skier").UnlockedItemCount <= 1 || !ShowCategoryIfNotViewed(Category.StartingSkier)) && (ItemManager.Instance.GetItemSet("slope").UnlockedItemCount <= 1 || !ShowCategoryIfNotViewed(Category.Trail)))
		{
			if (ratingPopup.ShouldShow())
			{
				m_autoShow = true;
				UnityEngine.Object.Instantiate(ratingPopup);
			}
			else if ((bool)AdManager.Instance && AdManager.Instance.ShowInterstitial())
			{
				m_autoShow = true;
			}
		}
	}

	private void UpdatePage()
	{
		m_currentPageIndex = Mathf.Clamp(m_currentPageIndex, 0, m_activePages.Count - 1);
		AssetManager.UpdateTexture(pageSprite, m_activePages[m_currentPageIndex]);
		if (m_activePages.Count > 1)
		{
			for (int i = 0; i < m_activePages.Count; i++)
			{
				m_pageBlips[i].Active = i == m_currentPageIndex;
			}
		}
		bool flag = OnLastPage();
        Debug.Log(OnLastPage()+":"+ m_shownLastPage);
		if (!m_shownLastPage && flag)
		{
            Debug.Log("ShownLastPage");

            m_shownLastPage = true;
            OnLastPageShown();

            GameState.SetInt(GetCategoryKey(m_currentCategory), 1);
		}
		leftButton.gameObject.SetActive(m_currentPageIndex > 0 && (!m_autoShow || m_shownLastPage));
		rightButton.gameObject.SetActive(!flag);
	}

	public GUIButton InitialFocusButton()
	{
		if (OnLastPage())
		{
			return closeButton;
		}
		return rightButton;
	}

	private bool OnLastPage()
	{
		return m_currentPageIndex >= 9;
	}

	private void OnLastPageShown()
	{
		if (m_currentCategory == Category.None && m_autoShow)
		{
			SkiGameManager.Instance.OnTutorialsViewed();
		}
		else
		{
			Invoke("ShowCloseButton", 1f);
		}
	}

	private void ShowCloseButton()
	{
		if (!closeButton.gameObject.activeInHierarchy)
		{
			closeButton.gameObject.SetActive(true);
			closeButton.transform.localScale = Vector3.one;
			if (0 == 0)
			{
				Go.to(closeButton.transform, 0.5f, new GoTweenConfig().scale(0.5f, true).setEaseType(GoEaseType.ElasticPunch));
			}
			SoundManager.Instance.PlaySound(closeButtonSound);
		}
	}

	private void FilterPages(Category category)
	{
		headingText.Text = categoryHeadings[(int)category];
		ClearActivePages();
		_003CFilterPages_003Ec__AnonStorey1F _003CFilterPages_003Ec__AnonStorey1F = new _003CFilterPages_003Ec__AnonStorey1F();
		PageSet[] array = pageSets;
		for (int i = 0; i < array.Length; i++)
		{
			_003CFilterPages_003Ec__AnonStorey1F.pageSet = array[i];
			if (_003CFilterPages_003Ec__AnonStorey1F.pageSet.category == category || (category == Category.None && GameState.HasKey("tut_" + _003CFilterPages_003Ec__AnonStorey1F.pageSet.category)))
			{
				PageSet pageSet = m_pageSetOverrides.Find(_003CFilterPages_003Ec__AnonStorey1F._003C_003Em__16);
				if (pageSet != null)
				{
					m_activePages.AddRange(pageSet.pageNames);
				}
				else
				{
					m_activePages.AddRange(_003CFilterPages_003Ec__AnonStorey1F.pageSet.pageNames);
				}
			}
		}
		Vector3 localPosition = pageBlipOffset;
		localPosition.x -= (float)(m_activePages.Count - 1) * 0.5f * pageBlipSeparation;
		int num = ((m_activePages.Count > 1) ? m_activePages.Count : 0);
		for (int j = 0; j < num; j++)
		{
			m_pageBlips[j].transform.localPosition = localPosition;
			m_pageBlips[j].PageIndex = j;
			m_pageBlips[j].gameObject.SetActive(true);
			localPosition.x += pageBlipSeparation;
		}
		for (int k = num; k < m_pageBlips.Length; k++)
		{
			m_pageBlips[k].gameObject.SetActive(false);
		}
	}

	private void ClearActivePages()
	{
		m_activePages.Clear();
	}

	private void Awake()
	{
		Instance = this;
		int num = 0;
		PageSet[] array = pageSets;
		foreach (PageSet pageSet in array)
		{
			num += pageSet.pageNames.Length;
		}
		m_pageBlips = new GUIPageBlip[num];
		for (int j = 0; j < num; j++)
		{
			GUIPageBlip gUIPageBlip = TransformUtils.Instantiate(pageBlipPrefab, base.transform);
			m_pageBlips[j] = gUIPageBlip;
		}
		closeButton.OnClick = Hide;
	}

	private void OnDisable()
	{
		ClearActivePages();
	}
}
