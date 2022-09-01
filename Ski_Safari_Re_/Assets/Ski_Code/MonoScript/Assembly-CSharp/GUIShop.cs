using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class GUIShop : MonoBehaviour
{
	[Serializable]
	public class TabButtonInfo
	{
		public GUIShopTabButton button;

		public GUIShopTab tabPrefab;

		public string[] itemSetNames;

		public bool isProductTab;

		public TargetStore store;
	}

	private enum State
	{
		Hidden = 0,
		Opening = 1,
		Browsing = 2,
		Buying = 3,
		Downloading = 4,
		AddingBoosterCount = 5,
		RefreshingBoosterItem = 6,
		ChallengeStarHidingShop = 7,
		ChallengeStarShowingChallenges = 8,
		ChallengeStarAnimatingStar = 9,
		ChallengeStarCompleting = 10,
		WaitingOnTransaction = 11,
		WaitingOnRestore = 12,
		GivingCoins = 13,
		ShowingMessage = 14,
		ShowingPopup = 15,
        RefreshingQiuqiubiItem = 16,
        AddingQiuqiubiCount = 17

    }

    [CompilerGenerated]
	private sealed class _003CStart_003Ec__AnonStorey1E
	{
		internal TargetStore currentStore;

		internal bool _003C_003Em__15(TabButtonInfo t)
		{
			return (t.store & currentStore) == currentStore;
		}
	}

	public static GUIShop Instance;

	public GUITransitionAnimator[] transitionAnimators;

	public GUIShopTab tabPrefab;

	public Transform tabContainer;

	public TabButtonInfo[] tabButtons;

	public GUICoinCount coinCount;

	public GUITransitionAnimator message;

	public TextMesh messageText;

	public GameObject itemDetails;

	public GUIDropShadowText nameText;

	public GUIDropShadowText descriptionText;

	public GUIDropShadowText lockedText;

	public Transform starCenterNode;

	public GUIStar starPrefab;

	public float starSeparation = 1.25f;

	public int maxItemAchievements = 5;

	public GUIShopPopup boostersPopupPrefab;

	public GUITransitionAnimator boosterInfo;
	public GUITransitionAnimator qiuqiubiInfo;

	public Renderer boosterSprite;

	public GUIDropShadowText boosterCountText;

	public Sound boosterAddSound;

	public float boosterIncrementDuration = 0.75f;

	public GUITransitionAnimator commentator;

	public GameObject commentatorMessage;

	public GUIDropShadowText commentatorSavingsText;

	public Sound commentatorMesssageSound;

	public GameObject commentatorTalkingFace;

	public float showCommentDelay = 2f;

	public GameObject buttons;

	public GUIButton buyButton;

	public GameObject buyButtonActiveObject;

	public GameObject buyButtonInactiveObject;

	public Sound buySuccessSound;

	public Sound buyFailSound;

	public Sound unequipSound;

	public GUIButton equipButton;

	public GameObject equippedObject;

	public GUIButton unequipButton;

	public GUIButton buyProductButton;

	public Sound purchaseSuccessSound;

	public Transform buyEffectNode;

	public  ParticleSystem buyCoinParticles;

	public GameObject unlockEffectPrefab;

	public GUIButton downloadButton;

	public GameObject downloadButtonActiveObject;

	public GameObject downloadButtonInactiveObject;

	public GUIShopDownloadPopup downloadPopup;

	public GUIShopProductPopup productPopupPrefab;

	public Transform giveCoinsEffectNode;

	public  ParticleSystem giveCoinParticles;

	public GameObject giveCoinGlintEffect;

	public GUIButton restoreTransactionsButton;

	public GUIShopCoinButton coinButton;

	public float transactionTimeout = 10f;

	public GUITransitionAnimator banner;

	public GUIButton bannerButton;

	public Renderer bannerImage;

	public float bannerUpdateDelay = 0.5f;

	public Action OnShow;

	public Action OnHide;

	public Action OnItemChanged;

	private State m_state;

	private float m_stateTimer;

	private GUIShopTab[] m_tabs;

	private int m_currentTabIndex;

	private GUIStar[] m_stars;

	private float m_showCommentTimer;

	private float m_buyPulseTimer;

	private int m_costRemainingSource;

	private int m_costRemainingTarget;

	private string m_currentBannerURL;

	private string m_currentBannerName;

	private bool m_musicPlaying;

	private bool m_hasShownOnce;

	private bool m_equipBoughtItemWhenFinishedPopup;

	private Vector3 m_initialItemSpriteLocalPos;

	private Vector3 m_initialItemSpriteLocalScale;

	public int CurrentTabIndex
	{
		get
		{
			return m_currentTabIndex;
		}
		set
		{
			if (value != m_currentTabIndex && m_state == State.Browsing)
			{
				m_tabs[m_currentTabIndex].SelectedGUIItem.IsNew = false;
			}
			m_currentTabIndex = value;
			for (int i = 0; i < m_tabs.Length; i++)
			{
				GUIShopTab gUIShopTab = m_tabs[i];
				gUIShopTab.Show = false;
				gUIShopTab.tabButton.SetToggled(i == m_currentTabIndex);
			}
			if (m_state != 0 && m_state != State.Opening)
			{
				LoadCurrentTab();
				SetState(State.Browsing);
			}
		}
	}

	public GUIShopTab CurrentTab
	{
		get
		{
			return m_tabs[m_currentTabIndex];
		}
	}

	private int CoinCount
	{
		get
		{
			return GameState.CoinCount;
		}
	}
    private int QiuqiubiCount
    {
        get
        {
            return GameState.QiuqiubiCount;
        }
    }
    public static TargetStore GetTargetStore()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			return TargetStore.Editor;
		case RuntimePlatform.Android:
			return TargetStore.Editor;
		default:
			return TargetStore.Editor;
		}
	}

	public void Show()
	{
		if (m_state == State.Hidden)
		{
			m_state = State.Opening;
			m_stateTimer = 0f;
			AnalyticsManager.Instance.SendEvent("shop_show");
			giveCoinGlintEffect.SetActive(false);
			SelectInitialTab();
			itemDetails.SetActive(true);
			buttons.SetActive(true);
			if (OnShow != null)
			{
				OnShow();
			}
		}
	}

	public void Hide()
	{
		if (m_state != 0)
		{
			m_state = State.Hidden;
			AnalyticsManager.Instance.SendEvent("shop_hide");
			GUITransitionAnimator[] array = transitionAnimators;
			foreach (GUITransitionAnimator gUITransitionAnimator in array)
			{
				gUITransitionAnimator.Hide();
			}
			boosterInfo.SnapHide();
			AssetManager.UnloadTexture(boosterSprite);
			itemDetails.SetActive(false);
			buttons.SetActive(false);
			lockedText.gameObject.SetActive(false);
			commentator.Hide();
			m_tabs[m_currentTabIndex].SelectedGUIItem.IsNew = false;
			m_tabs[m_currentTabIndex].Show = false;
			CancelInvoke("UpdateBanner");
			banner.SnapHide();
			if (OnHide != null)
			{
				OnHide();
			}
		}
	}

	public void SelectItem(Item item)
	{
		for (int i = 0; i < m_tabs.Length; i++)
		{
			if (m_tabs[i].TrySnapToItem(item))
			{
				m_hasShownOnce = true;
				CurrentTabIndex = i;
				break;
			}
		}
	}

	private void SelectInitialTab()
	{
		if (!m_hasShownOnce)
		{
			m_hasShownOnce = true;
			for (int i = 0; i < m_tabs.Length; i++)
			{
				int firstNewItemIndex = m_tabs[i].GetFirstNewItemIndex();
				if (firstNewItemIndex != -1)
				{
					CurrentTabIndex = i;
					m_tabs[i].SnapToItemIndex(firstNewItemIndex);
					UpdateItemDetails();
					return;
				}
			}
		}
		CurrentTabIndex = m_currentTabIndex;
	}

	private void ShowMessage(string text="",byte[] data_=null)
	{
		if (m_state != 0)
		{
            string back="";
            if(text!="")
            {
                back = text;
            }
            if (data_ != null)
            {
                back = Encoding.UTF8.GetString(data_);
            }
            messageText.text = back;
			message.Show();
			SetState(State.ShowingMessage);
		}
	}

	private void BuyItem()
	{
		Item selectedItem = m_tabs[m_currentTabIndex].SelectedItem;
        Debug.Log(selectedItem.Unlocked);
        Debug.Log(selectedItem.CurrentCost);
        Debug.Log(CoinCount);
		if (!selectedItem.Unlocked && selectedItem.CurrentCost <= CoinCount)
		{
			if (selectedItem.IsCached || HasInternetConnection())
			{
				SetState(State.Buying);
			}
			else
			{
				ShowMessage("Please connect to the internet\n   to download " + selectedItem.displayName);
			}
		}
		else
		{
			Vector3 shakeMagnitude = new Vector3(0f, 0f, 25f);
			float duration = 0.3f;
			Go.to(buyButton.transform, duration, new GoTweenConfig().shake(shakeMagnitude, GoShakeType.Eulers));
			SoundManager.Instance.PlaySound(buyFailSound);
			coinButton.OnBuyFail();
		}
	}
  
    private void BuyProduct()
	{
		Item_Product item_Product = m_tabs[m_currentTabIndex].SelectedItem as Item_Product;
        if (!item_Product)
		{
			return;
		}
        if(GameState.QiuqiubiCount -item_Product.cost<0)
        {
            byte[] data = Encoding.UTF8.GetBytes("你的球球币不够！");
            ShowMessage("",data);
            return;
        }
		Item_Gift item_Gift = item_Product as Item_Gift;
		if ((bool)item_Gift)
		{
			Application.OpenURL(item_Gift.giftURL);
			return;
		}
		if (!StoreManager.Instance.IsBillingSupported)
		{
			ShowMessage("In App Purchase is currently not available");
			return;
		}
		if (!StoreManager.Instance.CanPurchaseProduct(item_Product))
		{
			ShowMessage("This product is currently not available");
			return;
		}
		Item_CoinPack item_CoinPack = item_Product as Item_CoinPack;
        Debug.Log((bool)item_CoinPack);
		if ((bool)item_CoinPack)
		{
            int num = 0;
			ItemSet[] itemSets = ItemManager.Instance.itemSets;
            foreach (ItemSet itemSet in itemSets)
			{
				Item[] items = itemSet.items;
                foreach (Item item in items)
				{
                    if (item.CurrentCost > 0 && !item.Unlocked)
					{
						num = item_CoinPack.coinCount;

                    }else
                    {

                    }
                }
			}
			int num2 = GameState.CoinCount;
            GameState.IncrementQiuqiubiCount(-item_Product.cost);
            GameState.IncrementCoinCount(num);

            if (GameState.CoinCount >= num2)
			{
				ShowMessage("Thanks, but you already have plenty of coins!");
				return;
			}
		}
		SetState(State.WaitingOnTransaction);
		ItemManager.Instance.BuyProduct(item_Product);
	}

	private void RestoreTransactions()
	{
		if (m_tabs[m_currentTabIndex].ContainsNonConsumableProduct())
		{
			StoreManager.Instance.RestoreTransactions();
			SetState(State.WaitingOnRestore);
		}
	}

	private void EquipItem()
	{
		AnalyticsManager.Instance.SendEvent("shop_equip", "item", m_tabs[m_currentTabIndex].SelectedItem.name.ToLower());
		m_tabs[m_currentTabIndex].EquipItem();
	}

	private void UnequipItem()
	{
		AnalyticsManager.Instance.SendEvent("shop_unequip", "item", m_tabs[m_currentTabIndex].SelectedItem.name.ToLower());
		m_tabs[m_currentTabIndex].UnequipItem();
		SoundManager.Instance.PlaySound(unequipSound);
	}

	private void ShowPopup(GUIShopPopup popupPrefab)
	{
		GUIShopPopup gUIShopPopup = UnityEngine.Object.Instantiate(popupPrefab);
		gUIShopPopup.OnClosed = (Action)Delegate.Combine(gUIShopPopup.OnClosed, new Action(OnPopupClosed));
		SetState(State.ShowingPopup);
	}

	private bool HasInternetConnection()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	private void DownloadItem()
	{
		if (HasInternetConnection())
		{
			GUIShopDownloadPopup gUIShopDownloadPopup = UnityEngine.Object.Instantiate(downloadPopup);
			gUIShopDownloadPopup.SetItem(m_tabs[m_currentTabIndex].SelectedItem);
			gUIShopDownloadPopup.OnDownloadFinished = OnDownloadFinished;
			SetState(State.Downloading);
		}
		else
		{
			SoundManager.Instance.PlaySound(buyFailSound);
			ShowMessage("No internet connection");
			UpdateItemDetails();
		}
	}

	private void OnDownloadFinished(bool success, string error)
	{
		if (success)
		{
			EquipItem();
			SetState(State.Browsing);
		}
		else
		{
			ShowMessage(error);
			UpdateItemDetails();
		}
	}

	private void OnPopupClosed()
	{
		if (m_equipBoughtItemWhenFinishedPopup)
		{
			m_equipBoughtItemWhenFinishedPopup = false;
			EquipBoughtItem();
		}
		else
		{
			SetState(State.Browsing);
		}
	}

	private void OpenBannerURL()
	{
		AnalyticsManager.Instance.SendEvent("shop_banner_click", "item", m_currentBannerName);
		Application.OpenURL(m_currentBannerURL);
	}

	private void UpdateBanner()
	{
		Item selectedItem = m_tabs[m_currentTabIndex].SelectedItem;
		string bannerURL = selectedItem.BannerURL;
		if (!string.IsNullOrEmpty(selectedItem.bannerTextureName) && !string.IsNullOrEmpty(bannerURL))
		{
			bannerImage.material.mainTexture = AssetManager.LoadAsset<Texture>(selectedItem.bannerTextureName);
			m_currentBannerURL = bannerURL;
			m_currentBannerName = selectedItem.name;
			banner.Show();
		}
		else
		{
			banner.Hide();
		}
	}

	private void HideBuyAndEquipButtons()
	{
		buyButton.gameObject.SetActive(false);
		equipButton.gameObject.SetActive(false);
		buyProductButton.gameObject.SetActive(false);
		buyButton.transform.localScale = Vector3.one;
		equipButton.transform.localScale = Vector3.one;
		buyProductButton.transform.localScale = Vector3.one;
		downloadButton.gameObject.SetActive(false);
		m_buyPulseTimer = 0f;
	}

	public void UpdateItemDetails()
	{
		GUIShopTab gUIShopTab = m_tabs[m_currentTabIndex];
		Item selectedItem = gUIShopTab.SelectedItem;
		nameText.Text = selectedItem.displayName;
		descriptionText.Text = selectedItem.description;
		descriptionText.TextScale = selectedItem.descriptionTextScale;
		bool flag = false;
        if (selectedItem == gUIShopTab.SelectedItemSet.CurrentItem)
        {
            lockedText.gameObject.SetActive(false);
            equippedObject.SetActive(true);
            unequipButton.gameObject.SetActive(gUIShopTab.RelativeSelectedItemIndex != gUIShopTab.SelectedItemSet.defaultIndex);
            HideBuyAndEquipButtons();
        }
        else if (selectedItem is Item_Product)
        {
            lockedText.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            equippedObject.SetActive(false);
            unequipButton.gameObject.SetActive(false);
            downloadButton.gameObject.SetActive(false);
            Item_Product item_Product = selectedItem as Item_Product;
            buyProductButton.gameObject.SetActive(item_Product.Purchasable);
        }
        else if (selectedItem is Item_Booster)
        {
            lockedText.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(true);
            bool flag2 = selectedItem.CurrentCost <= CoinCount;
            buyButtonActiveObject.SetActive(flag2);
            buyButtonInactiveObject.SetActive(!flag2);
            equipButton.gameObject.SetActive(false);
            equippedObject.SetActive(false);
            unequipButton.gameObject.SetActive(false);
            buyProductButton.gameObject.SetActive(false);
            downloadButton.gameObject.SetActive(false);
        }
        else if (selectedItem is Item_QiuqiubiPack)
        {
            lockedText.gameObject.SetActive(false);
            bool flag2 = false;

            if (SkiGameManager.Instance.custom.InfiniteCoinToggle)
            {
                flag2 = selectedItem.CurrentCost <= CoinCount - 114514;
            }
            else
            {
                flag2 = selectedItem.CurrentCost <= CoinCount;
            }
            buyButton.gameObject.SetActive(flag2);
            buyButtonActiveObject.SetActive(flag2);
            buyButtonInactiveObject.SetActive(!flag2);
            equipButton.gameObject.SetActive(false);
            equippedObject.SetActive(false);
            unequipButton.gameObject.SetActive(false);
            buyProductButton.gameObject.SetActive(false);
            downloadButton.gameObject.SetActive(false);
        }
        else if (selectedItem.Unlocked)
		{
			lockedText.gameObject.SetActive(false);
			buyButton.gameObject.SetActive(false);
			equippedObject.SetActive(false);
			unequipButton.gameObject.SetActive(false);
			buyProductButton.gameObject.SetActive(false);
			if (selectedItem.IsCached)
			{
				equipButton.gameObject.SetActive(true);
				downloadButton.gameObject.SetActive(false);
			}
			else
			{
				equipButton.gameObject.SetActive(false);
				downloadButton.gameObject.SetActive(true);
				bool flag3 = HasInternetConnection();
				downloadButtonActiveObject.SetActive(flag3);
				downloadButtonInactiveObject.SetActive(!flag3);
			}
		}
		else if (selectedItem.lockedUntilUpdate)
		{
			lockedText.gameObject.SetActive(true);
			lockedText.Text = "Coming soon!";
			equippedObject.SetActive(false);
			unequipButton.gameObject.SetActive(false);
			HideBuyAndEquipButtons();
		}
		else if (selectedItem.requiredLevel > LevelManager.Instance.CurrentLevel)
		{
			lockedText.gameObject.SetActive(true);
			lockedText.Text = string.Format("Available at Rank {0} ({1})", selectedItem.requiredLevel, LevelManager.Instance.levelDescriptors[selectedItem.requiredLevel - 1].name);
			equippedObject.SetActive(false);
			unequipButton.gameObject.SetActive(false);
			HideBuyAndEquipButtons();
		}
		else if ((bool)selectedItem.requiredItem && !selectedItem.requiredItem.Unlocked)
		{
			lockedText.gameObject.SetActive(true);
			lockedText.Text = "Buy " + selectedItem.requiredItem.displayName + " to unlock!";
			equippedObject.SetActive(false);
			unequipButton.gameObject.SetActive(false);
			HideBuyAndEquipButtons();
		}
		else
		{
			lockedText.gameObject.SetActive(false);
			buyButton.gameObject.SetActive(true);
			bool flag4 = selectedItem.CurrentCost <= CoinCount && (selectedItem.IsCached || HasInternetConnection());
			buyButtonActiveObject.SetActive(flag4);
			buyButtonInactiveObject.SetActive(!flag4);
			equipButton.gameObject.SetActive(false);
			equippedObject.SetActive(false);
			unequipButton.gameObject.SetActive(false);
			buyProductButton.gameObject.SetActive(false);
			downloadButton.gameObject.SetActive(false);
			if (m_state != 0 && selectedItem.cost > selectedItem.CurrentCost)
			{
				commentator.Show();
				commentatorMessage.SetActive(false);
				commentatorTalkingFace.SetActive(false);
				commentatorSavingsText.Text = (selectedItem.cost - selectedItem.CurrentCost).ToString("N0");
				m_showCommentTimer = showCommentDelay;
				flag = true;
			}
		}
		if (!flag)
		{
			commentator.SnapHide();
		}
		Item_QiuqiubiPack item_Qiuqiubi = selectedItem as Item_QiuqiubiPack;
		Item_Booster item_Booster = selectedItem as Item_Booster;
       /*if ((bool)item_Qiuqiubi)
        {
            boosterInfo.Show();
            AssetManager.UpdateTexture(boosterSprite, item_Booster.commonIconTextureName);
            boosterCountText.Text = item_Booster.booster.CurrentCount.ToString();
        }
        else
        {
            AssetManager.UnloadTexture(boosterSprite);
            boosterInfo.SnapHide();
        }*/
        if ((bool)item_Booster)
		{
			boosterInfo.Show();
			AssetManager.UpdateTexture(boosterSprite, item_Booster.commonIconTextureName);
			boosterCountText.Text = item_Booster.booster.CurrentCount.ToString();
		}
		else
		{
			AssetManager.UnloadTexture(boosterSprite);
			boosterInfo.SnapHide();
		}
		int num = 0;
		for (int i = 0; i < selectedItem.achievements.Length; i++)
		{
			if (selectedItem.achievements[i].IsComplete)
			{
				num++;
			}
		}
		Vector3 position = starCenterNode.position;
		position.x -= (float)(selectedItem.achievements.Length - 1) * starSeparation * 0.5f;
		for (int j = 0; j < selectedItem.achievements.Length; j++)
		{
			m_stars[j].gameObject.SetActive(true);
			m_stars[j].Active = j < num;
			m_stars[j].transform.position = position;
			position.x += starSeparation;
		}
		for (int k = selectedItem.achievements.Length; k < maxItemAchievements; k++)
		{
			m_stars[k].gameObject.SetActive(false);
		}
		if (StoreManager.Instance.IsRestoreTransactionsSupported && selectedItem is Item_NonConsumableProduct && !selectedItem.Unlocked)
		{
			restoreTransactionsButton.gameObject.SetActive(true);
		}
		else
		{
			restoreTransactionsButton.gameObject.SetActive(false);
		}
		CancelInvoke("UpdateBanner");
		Invoke("UpdateBanner", bannerUpdateDelay);
		if (m_tabs[m_currentTabIndex].SelectedItemSet.name == "booster" && PlayerPrefs.GetInt("booster_popup_shown") == 0)
		{
			ShowPopup(boostersPopupPrefab);
			PlayerPrefs.SetInt("booster_popup_shown", 1);
			m_tabs[m_currentTabIndex].SnapToItemIndex(m_tabs[m_currentTabIndex].SelectedItemIndex);
		}
	}

	private void LoadCurrentTab()
	{
		m_tabs[m_currentTabIndex].Show = true;
		AnalyticsManager.Instance.SendEvent("shop_browse", "tab", ItemManager.Instance.itemSets[m_currentTabIndex].name.ToLower());
		UpdateItemDetails();
		if (tabButtons[m_currentTabIndex].isProductTab && !PlayerPrefs.HasKey("product_popup_shown"))
		{
			ShowPopup(productPopupPrefab);
			PlayerPrefs.SetInt("product_popup_shown", 1);
		}
	}

	private void SetState(State state)
	{
		switch (m_state)
		{
		case State.WaitingOnTransaction:
		case State.WaitingOnRestore:
			if (state != State.ShowingMessage)
			{
				SkiGameManager.Instance.PopupEnabled = false;
				m_tabs[m_currentTabIndex].Locked = false;
				message.Hide();
			}
			break;
		case State.ShowingMessage:
			SkiGameManager.Instance.PopupEnabled = false;
			m_tabs[m_currentTabIndex].Locked = false;
			break;
		case State.Buying:
		case State.RefreshingBoosterItem:
            case State.RefreshingQiuqiubiItem:
            case State.GivingCoins:
			SkiGameManager.Instance.RemoveInputLock("shop");
			m_tabs[m_currentTabIndex].Locked = false;
			break;
		}
		m_state = state;
		m_stateTimer = 0f;
		switch (m_state)
		{
		case State.Browsing:
                RayTarget.IsRay = true;
			m_musicPlaying = false;
			break;
		case State.Buying:
		{
			SkiGameManager.Instance.AddInputLock("shop");
			m_tabs[m_currentTabIndex].Locked = true;
			m_costRemainingSource = (m_costRemainingTarget = m_tabs[m_currentTabIndex].SelectedItem.CurrentCost);
			buyButton.gameObject.SetActive(false);
			buyCoinParticles.enableEmission = true;
                    if (SoundManager.Instance.SFXEnabled)
                    {
                        buyEffectNode.GetComponent<AudioSource>().Play();
                    }
			SoundManager.Instance.StopMusic();
			Item selectedItem = m_tabs[m_currentTabIndex].SelectedItem;
			Item_Booster item_Booster = selectedItem as Item_Booster;
                    if ((bool)item_Booster)
                    {
                        item_Booster.booster.CurrentCount += item_Booster.count;
                    }
                    else
                    {
                        selectedItem.Unlocked = true;
                    }
                    Item_QiuqiubiPack item_Qiuqiubi = selectedItem as Item_QiuqiubiPack;
                    if ((bool)item_Qiuqiubi)
                    {
                        GameState.IncrementQiuqiubiCount(item_Qiuqiubi.count);
                    }
                    else
                    {
                        selectedItem.Unlocked = true;
                    }
                    selectedItem.IsNew = false;
                    //
                    //
                    //
			GameState.IncrementCoinCount(-selectedItem.CurrentCost);
                    //
                    //
                    //
			GameState.Save();
			coinCount.enabled = false;
			try
			{
				int num = ItemManager.Instance.CountPurchasedItems();
				AnalyticsManager.Instance.SendEvent("shop_buy", "item", m_tabs[m_currentTabIndex].SelectedItem.name.ToLower(), "item_count", num.ToString());
				if (PlayerPrefs.HasKey("coin_pack_bought"))
				{
					AnalyticsManager.Instance.SendEvent("shop_buy_after_coins", "item", m_tabs[m_currentTabIndex].SelectedItem.name.ToLower(), "item_count", num.ToString());
					PlayerPrefs.DeleteKey("coin_pack_bought");
				}
				break;
			}
			catch (Exception)
			{
				break;
			}
		}
		case State.WaitingOnTransaction:
		case State.WaitingOnRestore:
			SkiGameManager.Instance.PopupEnabled = true;
			m_tabs[m_currentTabIndex].Locked = true;
			messageText.text = "Processing...";
			message.Show();
			break;
		case State.GivingCoins:
		{
			SkiGameManager.Instance.AddInputLock("shop");
			m_tabs[m_currentTabIndex].Locked = true;
			Item_CoinPack item_CoinPack = m_tabs[m_currentTabIndex].SelectedItem as Item_CoinPack;
			GameState.IncrementCoinCount(item_CoinPack.coinCount);
			GameState.Save();
			coinCount.enabled = false;
			m_costRemainingSource = (m_costRemainingTarget = item_CoinPack.coinCount);
			buyProductButton.gameObject.SetActive(false);
			giveCoinParticles.enableEmission = true;
			giveCoinGlintEffect.SetActive(true);
			if (SoundManager.Instance.SFXEnabled)
			{
				giveCoinsEffectNode.GetComponent<AudioSource>().Play();
			}
			SoundManager.Instance.StopMusic();
			m_tabs[m_currentTabIndex].SelectedGUIItem.IsNew = false;
			break;
		}
		case State.AddingBoosterCount:
            case State.AddingQiuqiubiCount:
            case State.RefreshingBoosterItem:
			SkiGameManager.Instance.AddInputLock("shop");
			m_tabs[m_currentTabIndex].Locked = true;
			break;
            case State.RefreshingQiuqiubiItem:
                SkiGameManager.Instance.AddInputLock("shop");
                m_tabs[m_currentTabIndex].Locked = true;
                break;
            case State.ShowingMessage:
                RayTarget.IsRay = false;
			SkiGameManager.Instance.PopupEnabled = true;
			m_tabs[m_currentTabIndex].Locked = true;
			break;
		case State.Downloading:
		case State.ChallengeStarHidingShop:
		case State.ChallengeStarShowingChallenges:
		case State.ChallengeStarAnimatingStar:
		case State.ChallengeStarCompleting:
			break;
		}
	}

	private void UpdateButtonScale(GUIButton button)
	{
		float num = 1f + Mathf.Sin(m_buyPulseTimer * 3f) * 0.1f;
		button.transform.localScale = new Vector3(num, num, num);
		m_buyPulseTimer += Time.deltaTime;
	}

	private float CalculateCoinsPerSecond(float minCoins, float maxCoins, float minBuyDuration, float maxBuyDuration, float totalCoins)
	{
		float t = Mathf.Clamp01((totalCoins - minCoins) / (maxCoins - minCoins));
		float num = Mathf.Lerp(minBuyDuration, maxBuyDuration, t);
		return totalCoins / num;
	}

	private void UpdateBuying()
	{
		Item selectedItem = m_tabs[m_currentTabIndex].SelectedItem;
		float num = 5f;
		float num2 = 1.1f;
		float num3 = CalculateCoinsPerSecond(1000f, 10000f, 1f, 3f, selectedItem.CurrentCost);
		int b = Mathf.CeilToInt(num3 * Time.deltaTime);
		GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
		float num4 = 0.01f;
		buyEffectNode.GetComponent<AudioSource>().pitch = 1f + num4 * m_stateTimer;
		if (m_costRemainingSource > 0)
		{
			int num5 = Mathf.Min(m_costRemainingSource, b);
			m_costRemainingSource -= num5;
			coinCount.coinCountText.Text = (GameState.CoinCount + m_costRemainingSource).ToString("N0");
			if (m_costRemainingSource == 0)
			{
				buyCoinParticles.enableEmission = false;
			}
		}
		if (m_stateTimer > buyCoinParticles.startLifetime)
		{
			int num6 = Mathf.Min(m_costRemainingTarget, b);
			m_costRemainingTarget -= num6;
			m_tabs[m_currentTabIndex].SelectedGUIItem.costText.Text = m_costRemainingTarget.ToString("N0");
			float num7 = (float)selectedItem.CurrentCost / num3;
			float num8 = (m_stateTimer - buyCoinParticles.startLifetime) / num7;
			Quaternion localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f - num, num));
			selectedGUIItem.lockedObject.transform.localRotation = localRotation;
			selectedGUIItem.iconObject.transform.localRotation = localRotation;
			selectedGUIItem.lockedObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * num2, num8);
			selectedGUIItem.cracksSprite.material.SetFloat("_Cutoff", 1f - num8);
		}
		if (m_costRemainingTarget != 0)
		{
			return;
		}
		selectedGUIItem.iconObject.transform.localRotation = Quaternion.identity;
		selectedGUIItem.UpdateState(m_tabs[m_currentTabIndex].SelectedItemSet.CurrentItem);
		Go.from(selectedGUIItem.unlockedObject.transform, 0.5f, new GoTweenConfig().scale(0f));
		buyEffectNode.GetComponent<AudioSource>().Stop();
		SoundManager.Instance.PlaySound(buySuccessSound);
		TransformUtils.Instantiate(unlockEffectPrefab, selectedGUIItem.transform, false);
		coinCount.enabled = true;
		Item_Booster item_Booster = selectedItem as Item_Booster;
        Item_QiuqiubiPack item_Qiuqiubi = selectedItem as Item_QiuqiubiPack;
		Item_ChallengeStar item_ChallengeStar = selectedItem as Item_ChallengeStar;
        if ((bool)item_Qiuqiubi)
        {
            selectedGUIItem.lockedObject.SetActive(false);
            selectedGUIItem.costObject.SetActive(false);
            m_initialItemSpriteLocalPos = selectedGUIItem.sprite.transform.localPosition;
            m_initialItemSpriteLocalScale = selectedGUIItem.sprite.transform.localScale;
            //Go.to(selectedGUIItem.sprite.transform, boosterIncrementDuration, new GoTweenConfig().position(boosterSprite.transform.position + new Vector3(0f, 0f, -1f)).setEaseType(GoEaseType.QuadIn));
            //Go.to(selectedGUIItem.sprite.transform, boosterIncrementDuration, new GoTweenConfig().scale(boosterSprite.transform.localScale));
            SoundManager.Instance.PlaySound(boosterAddSound);
            SetState(State.AddingQiuqiubiCount);
        }else
        if ((bool)item_Booster)
		{
			selectedGUIItem.lockedObject.SetActive(false);
			selectedGUIItem.costObject.SetActive(false);
			m_initialItemSpriteLocalPos = selectedGUIItem.sprite.transform.localPosition;
			m_initialItemSpriteLocalScale = selectedGUIItem.sprite.transform.localScale;
			Go.to(selectedGUIItem.sprite.transform, boosterIncrementDuration, new GoTweenConfig().position(boosterSprite.transform.position + new Vector3(0f, 0f, -1f)).setEaseType(GoEaseType.QuadIn));
			Go.to(selectedGUIItem.sprite.transform, boosterIncrementDuration, new GoTweenConfig().scale(boosterSprite.transform.localScale));
			SoundManager.Instance.PlaySound(boosterAddSound);
			SetState(State.AddingBoosterCount);
		}
		else if ((bool)item_ChallengeStar)
		{
			m_initialItemSpriteLocalPos = selectedGUIItem.sprite.transform.localPosition;
			m_initialItemSpriteLocalScale = selectedGUIItem.sprite.transform.localScale;
			selectedGUIItem.sprite.transform.parent = null;
			Go.to(selectedGUIItem.sprite.transform, 0.5f, new GoTweenConfig().localPosition(new Vector3(0f, 2f, 0f), true));
			GUITransitionAnimator[] array = transitionAnimators;
			foreach (GUITransitionAnimator gUITransitionAnimator in array)
			{
				gUITransitionAnimator.Hide();
			}
			SetState(State.ChallengeStarHidingShop);
		}
		else if ((bool)selectedItem.guiShopPopup)
		{
			m_equipBoughtItemWhenFinishedPopup = true;
			ShowPopup(selectedItem.guiShopPopup);
		}
		else
		{
			EquipBoughtItem();
		}
	}

	private void EquipBoughtItem()
	{
		GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
		ItemManager.Instance.SelectDependentItems(selectedGUIItem.Item);
		if (m_tabs[m_currentTabIndex].SelectedItem.IsCached)
		{
			EquipItem();
		}
		else
		{
			DownloadItem();
		}
	}

	private void ItemSelected(Item item)
	{
		if (m_state != 0)
		{
			m_tabs[m_currentTabIndex].UpdateState();
			UpdateItemDetails();
			if (OnItemChanged != null)
			{
				OnItemChanged();
			}
			AchievementManager.Instance.ClearActiveAchievements();
			AchievementManager.Instance.PopulateActiveAchievements();
			if (m_state != State.ShowingPopup && m_state != State.ShowingMessage)
			{
				SetState(State.Browsing);
			}
		}
	}

	private void ItemDownloadFailed(Item item, string reason)
	{
		if (m_state != 0)
		{
			ShowMessage(reason);
		}
	}


    private void UpdateAddingQiuqiubiCount()
    {
        if (m_stateTimer > boosterIncrementDuration)
        {
            Item_QiuqiubiPack item_Booster = m_tabs[m_currentTabIndex].SelectedItem as Item_QiuqiubiPack;
            GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
            selectedGUIItem.sprite.enabled = false;
            qiuqiubiInfo.transform.localScale = Vector3.one;
            Go.to(qiuqiubiInfo.gameObject.transform, 0.5f, new GoTweenConfig().scale(0.25f, true).setEaseType(GoEaseType.ElasticPunch));
            SoundManager.Instance.PlaySound(item_Booster.selectSound);
            SetState(State.RefreshingBoosterItem);
        }
    }

    private void UpdateAddingBoosterCount()
	{
		if (m_stateTimer > boosterIncrementDuration)
		{
			Item_Booster item_Booster = m_tabs[m_currentTabIndex].SelectedItem as Item_Booster;
			boosterCountText.Text = item_Booster.booster.CurrentCount.ToString();
			GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
			selectedGUIItem.sprite.enabled = false;
			boosterInfo.transform.localScale = Vector3.one;
			Go.to(boosterInfo.gameObject.transform, 0.5f, new GoTweenConfig().scale(0.25f, true).setEaseType(GoEaseType.ElasticPunch));
			SoundManager.Instance.PlaySound(item_Booster.selectSound);
			SetState(State.RefreshingBoosterItem);
		}
	}
    private void UpdateRefreshingQiuqiubiItem()
    {
        if (m_stateTimer > 0.5f)
        {
            GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
            selectedGUIItem.sprite.enabled = true;
            Go.killAllTweensWithTarget(selectedGUIItem.sprite.transform);
            selectedGUIItem.sprite.transform.localPosition = m_initialItemSpriteLocalPos;
            selectedGUIItem.sprite.transform.localScale = m_initialItemSpriteLocalScale;
            selectedGUIItem.costObject.SetActive(true);
            selectedGUIItem.lockedObject.SetActive(true);
            Go.from(selectedGUIItem.sprite.transform, 0.5f, new GoTweenConfig().scale(0f));
            Go.from(selectedGUIItem.lockedObject.transform, 0.5f, new GoTweenConfig().scale(0f));
            UpdateItemDetails();
            SetState(State.Browsing);
        }
    }
    private void UpdateRefreshingBoosterItem()
	{
		if (m_stateTimer > 0.5f)
		{
			GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
			selectedGUIItem.sprite.enabled = true;
			Go.killAllTweensWithTarget(selectedGUIItem.sprite.transform);
			selectedGUIItem.sprite.transform.localPosition = m_initialItemSpriteLocalPos;
			selectedGUIItem.sprite.transform.localScale = m_initialItemSpriteLocalScale;
			selectedGUIItem.costObject.SetActive(true);
			selectedGUIItem.lockedObject.SetActive(true);
			Go.from(selectedGUIItem.sprite.transform, 0.5f, new GoTweenConfig().scale(0f));
			Go.from(selectedGUIItem.lockedObject.transform, 0.5f, new GoTweenConfig().scale(0f));
			UpdateItemDetails();
			SetState(State.Browsing);
		}
	}

	private void UpdateChallengeStarHidingShop()
	{
		if (m_stateTimer > 1f)
		{
			GUIAchievementList.Instance.Show(GUIAchievementList.ShowPosition.BottomLeft, true);
			SetState(State.ChallengeStarShowingChallenges);
		}
	}

	private void UpdateChallengeStarShowingChallenges()
	{
		if (m_stateTimer > 0.5f)
		{
			Item_ChallengeStar item_ChallengeStar = CurrentTab.SelectedItem as Item_ChallengeStar;
			Transform transform = GUIAchievementList.Instance.Entries[item_ChallengeStar.index].starSprite.transform;
			GUIShopItem selectedGUIItem = CurrentTab.SelectedGUIItem;
			Go.to(selectedGUIItem.sprite.transform, 0.5f, new GoTweenConfig().position(transform.position).scale(transform.lossyScale));
			SetState(State.ChallengeStarAnimatingStar);
		}
	}

	private void UpdateChallengeStarAnimatingStar()
	{
		if (m_stateTimer > 0.5f)
		{
			Item_ChallengeStar item_ChallengeStar = CurrentTab.SelectedItem as Item_ChallengeStar;
			GUIAchievementList.Instance.Entries[item_ChallengeStar.index].starSprite.SetActive(true);
			GUIShopItem selectedGUIItem = CurrentTab.SelectedGUIItem;
			selectedGUIItem.sprite.enabled = false;
			AchievementManager.Instance.ActiveAchievements[item_ChallengeStar.index].Complete();
			GUIAchievements.Instance.UpdateAutoShow();
			GUIAchievements.Instance.Show();
			SetState(State.ChallengeStarCompleting);
		}
	}

	private void UpdateChallengeStarCompleting()
	{
		if (!GUIAchievements.Instance.IsShowing && !GUITutorials.Instance.AutoShow)
		{
			GUIShopItem selectedGUIItem = CurrentTab.SelectedGUIItem;
			selectedGUIItem.sprite.transform.parent = selectedGUIItem.transform;
			Go.killAllTweensWithTarget(selectedGUIItem.sprite.transform);
			selectedGUIItem.sprite.transform.localPosition = m_initialItemSpriteLocalPos;
			selectedGUIItem.sprite.transform.localScale = m_initialItemSpriteLocalScale;
			selectedGUIItem.sprite.enabled = true;
			CurrentTab.UpdateState();
			GUITransitionAnimator[] array = transitionAnimators;
			foreach (GUITransitionAnimator gUITransitionAnimator in array)
			{
				gUITransitionAnimator.Show();
			}
			SetState(State.Browsing);
		}
	}

	private void BuyProductRespose(Item_Product product, bool success, string error)
	{
		if (success)
		{
			AnalyticsManager.Instance.SendEvent("shop_purchase_" + product.productId, "item_count", ItemManager.Instance.CountPurchasedItems().ToString());
			if (product is Item_CoinPack)
			{
				PlayerPrefs.SetInt("coin_pack_bought", 1);
			}
			Item_Product item_Product = m_tabs[m_currentTabIndex].SelectedItem as Item_Product;
			if (m_state == State.WaitingOnTransaction && item_Product == product)
			{
				GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
				if (product is Item_CoinPack)
				{
					selectedGUIItem.purchaseCostObject.SetActive(false);
					SetState(State.GivingCoins);
					return;
				}
				selectedGUIItem.Item.IsNew = false;
				selectedGUIItem.Item.Unlocked = true;
				selectedGUIItem.UpdateState(m_tabs[m_currentTabIndex].SelectedItemSet.CurrentItem);
				Go.from(selectedGUIItem.unlockedObject.transform, 0.5f, new GoTweenConfig().scale(0f));
				SoundManager.Instance.PlaySound(purchaseSuccessSound);
				TransformUtils.Instantiate(unlockEffectPrefab, selectedGUIItem.transform, true);
				EquipItem();
				UpdateItemDetails();
				GameState.Save();
				if (m_state == State.WaitingOnTransaction)
				{
					SetState(State.Browsing);
				}
			}
			else
			{
				if (product is Item_CoinPack)
				{
					Item_CoinPack item_CoinPack = product as Item_CoinPack;
					GameState.IncrementCoinCount(item_CoinPack.coinCount);
				}
				else
				{
					product.Unlocked = true;
				}
				GameState.Save();
			}
		}
		else
		{
			AnalyticsManager.Instance.SendEvent("shop_buy_product_fail", "item", product.name.ToLower(), "item_count", ItemManager.Instance.CountPurchasedItems().ToString(), "error", error);
			ShowMessage(error);
		}
	}

	private void RestoreTransactionsSucceeded()
	{
		AnalyticsManager.Instance.SendEvent("shop_restore_transactions");
		ShowMessage("Transactions have been restored!");
	}

	private void RestoreTransactionsFailed(string error)
	{
		ShowMessage(error);
	}

	private void UpdateGivingCoins()
	{
		GUIShopItem selectedGUIItem = m_tabs[m_currentTabIndex].SelectedGUIItem;
		Item_CoinPack item_CoinPack = selectedGUIItem.Item as Item_CoinPack;
		float num = CalculateCoinsPerSecond(10000f, 75000f, 3f, 6f, item_CoinPack.coinCount);
		int b = Mathf.CeilToInt(num * Time.deltaTime);
		float num2 = 0.01f;
		giveCoinsEffectNode.GetComponent<AudioSource>().pitch = 1f + num2 * m_stateTimer;
		if (m_costRemainingSource > 0)
		{
			int num3 = Mathf.Min(m_costRemainingSource, b);
			m_costRemainingSource -= num3;
			if (m_costRemainingSource == 0)
			{
				giveCoinParticles.enableEmission = false;
				giveCoinGlintEffect.SetActive(false);
			}
			float num5 = (selectedGUIItem.CutoffRatio = 1f - (float)m_costRemainingSource / (float)item_CoinPack.coinCount);
		}
		if (m_stateTimer > giveCoinParticles.startLifetime)
		{
			int num6 = Mathf.Min(m_costRemainingTarget, b);
			m_costRemainingTarget -= num6;
			coinCount.coinCountText.Text = (GameState.CoinCount - m_costRemainingTarget).ToString("N0");
		}
		if (m_costRemainingTarget == 0)
		{
			selectedGUIItem.CutoffRatio = 0f;
			Go.from(selectedGUIItem.sprite.transform, 0.5f, new GoTweenConfig().scale(0f));
			selectedGUIItem.UpdateState(m_tabs[m_currentTabIndex].SelectedItemSet.CurrentItem);
			giveCoinsEffectNode.GetComponent<AudioSource>().Stop();
			SoundManager.Instance.PlaySound(purchaseSuccessSound);
			UpdateItemDetails();
			coinCount.enabled = true;
			SetState(State.Browsing);
		}
	}

	private void UpdateCommentary()
	{
		if (commentator.IsShowing && m_showCommentTimer > 0f)
		{
			m_showCommentTimer -= Time.deltaTime;
			if (m_showCommentTimer <= 0f)
			{
				commentatorMessage.SetActive(true);
				commentatorTalkingFace.SetActive(true);
				Go.killAllTweensWithTarget(commentatorMessage.transform);
				commentatorMessage.transform.localScale = Vector3.one;
				Go.to(commentatorMessage.transform, 0.5f, new GoTweenConfig().scale(0.25f, true).setEaseType(GoEaseType.ElasticPunch));
				SoundManager.Instance.PlaySound(commentatorMesssageSound);
			}
		}
	}

	private void UpdateState()
	{
		Item selectedItem = m_tabs[m_currentTabIndex].SelectedItem;
		switch (m_state)
		{
		case State.Opening:
			if (m_stateTimer > 0.1f)
			{
				LoadCurrentTab();
				GUITransitionAnimator[] array = transitionAnimators;
				foreach (GUITransitionAnimator gUITransitionAnimator in array)
				{
					gUITransitionAnimator.Show();
				}
				SetState(State.Browsing);
			}
			break;
		case State.Browsing:
			UpdateCommentary();
			if (selectedItem is Item_Product)
			{
				UpdateButtonScale(buyProductButton);
			}
			else if (selectedItem.Unlocked && selectedItem != m_tabs[m_currentTabIndex].SelectedItemSet.CurrentItem)
			{
				if (selectedItem.IsCached)
				{
					UpdateButtonScale(equipButton);
				}
				else
				{
					UpdateButtonScale(downloadButton);
				}
			}
			else if (!selectedItem.Unlocked && !selectedItem.lockedUntilUpdate && selectedItem.CurrentCost <= CoinCount && selectedItem.requiredLevel <= LevelManager.Instance.CurrentLevel && (!selectedItem.requiredItem || selectedItem.requiredItem.Unlocked))
			{
				UpdateButtonScale(buyButton);
			}
			if (!m_musicPlaying && m_stateTimer >= 1.5f)
			{
				SoundManager.Instance.PlayMusic(SkiGameManager.Instance.shopMusic, false);
			}
			break;
		case State.Buying:
			UpdateCommentary();
			UpdateBuying();
			break;
		case State.AddingBoosterCount:
			UpdateAddingBoosterCount();
			break;
            case State.AddingQiuqiubiCount:
                UpdateAddingQiuqiubiCount();
                break;
            case State.RefreshingBoosterItem:
			UpdateRefreshingBoosterItem();
			break;
            case State.RefreshingQiuqiubiItem:
                UpdateRefreshingQiuqiubiItem();
                break;
            case State.ChallengeStarHidingShop:
			UpdateChallengeStarHidingShop();
			break;
		case State.ChallengeStarShowingChallenges:
			UpdateChallengeStarShowingChallenges();
			break;
		case State.ChallengeStarAnimatingStar:
			UpdateChallengeStarAnimatingStar();
			break;
		case State.ChallengeStarCompleting:
			UpdateChallengeStarCompleting();
			break;
		case State.WaitingOnTransaction:
			if (m_stateTimer > transactionTimeout)
			{
				ShowMessage("Timed out waiting for transaction");
			}
			break;
		case State.GivingCoins:
			UpdateGivingCoins();
			break;
		case State.ShowingMessage:
			if (m_stateTimer >= 2f)
			{
				message.Hide();
				SetState(State.Browsing);
			}
			break;
		case State.ShowingPopup:
			break;
		case State.Downloading:
		case State.WaitingOnRestore:
			break;
		}
	}

	private void SetupEffects()
	{
		float num = 7.5f;
		Vector2 vector = new Vector2(-1f, -1.5f);
		Vector2 vector2 = new Vector3(-2f, -1.5f);
		Vector2 vector3 = new Vector3(0f, -1f, 0f);
		float num2 = 0.5f;
		float z = -2f;
		float num3 = (float)Screen.width / (float)Screen.height;
		Vector2 vector4 = new Vector2(num3 * num, num);
		vector4 += vector;
		float z2 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
		float magnitude = vector4.magnitude;
		float startLifetime = (magnitude + num2) / buyCoinParticles.startSpeed;
		buyEffectNode.localRotation = Quaternion.Euler(0f, 0f, z2);
		buyCoinParticles.transform.localPosition = new Vector3(magnitude, 0f, z);
		buyCoinParticles.startLifetime = startLifetime;
		buyCoinParticles.enableEmission = false;
		vector4 -= vector;
		vector4 += vector2;
		vector4 -= vector3;
		z2 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
		magnitude = vector4.magnitude;
		float startLifetime2 = (magnitude + num2) / giveCoinParticles.startSpeed;
		giveCoinsEffectNode.localRotation = Quaternion.Euler(0f, 0f, z2 + 180f);
		giveCoinsEffectNode.localPosition = vector3;
		giveCoinParticles.startLifetime = startLifetime2;
		giveCoinParticles.enableEmission = false;
	}

	private void Awake()
	{
		Instance = this;
		buyButton.OnClick = BuyItem;
		equipButton.OnClick = EquipItem;
		unequipButton.OnClick = UnequipItem;
		buyProductButton.OnClick = BuyProduct;
		bannerButton.OnClick = OpenBannerURL;
		GUIButton gUIButton = restoreTransactionsButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(RestoreTransactions));
		GUIButton gUIButton2 = downloadButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(DownloadItem));
		ItemManager.OnBuyProductResponse = (ItemManager.OnBuyProductResponseDelegate)Delegate.Combine(ItemManager.OnBuyProductResponse, new ItemManager.OnBuyProductResponseDelegate(BuyProductRespose));
		ItemManager.OnItemSelected = (Action<Item>)Delegate.Combine(ItemManager.OnItemSelected, new Action<Item>(ItemSelected));
		ItemManager.OnItemDownloadFailed = (Action<Item, string>)Delegate.Combine(ItemManager.OnItemDownloadFailed, new Action<Item, string>(ItemDownloadFailed));
		StoreManager instance = StoreManager.Instance;
		instance.OnRestoreTransactionsSucceeded = (Action)Delegate.Combine(instance.OnRestoreTransactionsSucceeded, new Action(RestoreTransactionsSucceeded));
		StoreManager instance2 = StoreManager.Instance;
		instance2.OnRestoreTransactionsFailed = (Action<string>)Delegate.Combine(instance2.OnRestoreTransactionsFailed, new Action<string>(RestoreTransactionsFailed));
		SetupEffects();
		m_stars = new GUIStar[maxItemAchievements];
		for (int i = 0; i < maxItemAchievements; i++)
		{
			m_stars[i] = TransformUtils.Instantiate(starPrefab, starCenterNode);
		}
	}

	private void Start()
	{
		_003CStart_003Ec__AnonStorey1E _003CStart_003Ec__AnonStorey1E = new _003CStart_003Ec__AnonStorey1E();
		_003CStart_003Ec__AnonStorey1E.currentStore = GetTargetStore();
		int num = tabButtons.Count(_003CStart_003Ec__AnonStorey1E._003C_003Em__15);
		m_tabs = new GUIShopTab[num];
		int i = 0;
		int num2 = 0;
		for (; i < tabButtons.Length; i++)
		{
			if ((tabButtons[i].store & _003CStart_003Ec__AnonStorey1E.currentStore) == _003CStart_003Ec__AnonStorey1E.currentStore)
			{
				m_tabs[num2] = UnityEngine.Object.Instantiate(tabButtons[i].tabPrefab, tabContainer.position, tabContainer.rotation) as GUIShopTab;
				m_tabs[num2].transform.parent = tabContainer;
				m_tabs[num2].Setup(tabButtons[i].button, tabButtons[i].itemSetNames);
				tabButtons[i].button.TabIndex = i;
				num2++;
			}
			else
			{
				tabButtons[i].button.gameObject.SetActive(false);
			}
		}
		m_currentTabIndex = 0;
	}

	private void OnEnable()
	{
		if ((bool)SkiGameManager.Instance && SkiGameManager.Instance.ShowShop)
		{
			CurrentTabIndex = m_currentTabIndex;
		}
	}

	private void Update()
	{
		if ((bool)SkiGameManager.Instance && SkiGameManager.Instance.ShowShop)
		{
			m_stateTimer += Time.deltaTime;
			UpdateState();
		}
       

    }
}
