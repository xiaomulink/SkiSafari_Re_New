using System.Collections.Generic;
using UnityEngine;

public class GUIShopItem : MonoBehaviour
{
	public GameObject iconObject;

	public GameObject newObject;

	public GameObject costObject;

	public GUIDropShadowText costText;

	public GameObject purchaseCostObject;

	public GUIDropShadowText purchaseCostText;

	public GameObject downloadableObject;

	public GameObject lockedObject;

	public Renderer cracksSprite;

	public GameObject unlockedObject;

	public GameObject equippedObject;

	public Vector3 unlockedObjectEquippedScale = Vector3.one;

	public Vector3 unlockedObjectNormalScale = new Vector3(0.5f, 0.5f, 0.5f);

	public Renderer sprite;

	public Material normalSpriteMaterial;

	public Material cutoffSpriteMaterial;

	public Transform starCenterNode;

	public GUIStar starPrefab;

	public float starSeparation = 1.25f;

	private Item m_item;

	private GUIStar[] m_stars;

	private float m_cutoffRatio;
    
	public Item Item
	{
		get
		{
			return m_item;
		}
		set
		{
			m_item = value;
			m_cutoffRatio = 0f;
		}
	}
    public Item Item_;


    public bool IsNew
	{
		get
		{
			return m_item.IsNew;
		}
		set
		{
			m_item.IsNew = value;
			newObject.SetActive(value);
		}
	}

	public float CutoffRatio
	{
		set
		{
			if (value == m_cutoffRatio)
			{
				return;
			}
			if (value == 0f)
			{
				AssetManager.UnloadTexture(sprite);
				sprite.material = normalSpriteMaterial;
				sprite.material.mainTexture = AssetManager.LoadAsset<Texture>(m_item.iconTextureName);
			}
			else
			{
				if (m_cutoffRatio == 0f)
				{
					AssetManager.UnloadTexture(sprite);
					sprite.material = cutoffSpriteMaterial;
					sprite.material.mainTexture = AssetManager.LoadAsset<Texture>(m_item.iconTextureName + "_cutoff");
				}
				sprite.material.SetFloat("_Cutoff", value);
			}
			m_cutoffRatio = value;
		}
	}

	public void OnTabVisible()
	{
		sprite.material.mainTexture = AssetManager.LoadAsset<Texture>(m_item.iconTextureName);
	}

	public void OnTabHidden()
	{
		AssetManager.UnloadTexture(sprite);
	}

	public void UpdateState(Item equippedItem)
	{
		newObject.SetActive(m_item.IsNew);
		Item_Product item_Product = m_item as Item_Product;
        Item_QiuqiubiPack item_Qiuqiubi = m_item as Item_QiuqiubiPack;
        if ((bool)item_Qiuqiubi)
        {
            costObject.SetActive(true);
            purchaseCostObject.SetActive(false);
            cracksSprite.material.SetFloat("_Cutoff", 1f);
            costText.Text = m_item.CurrentCost.ToString("N0");
        }else
        if ((bool)item_Product)
        {
            costObject.SetActive(false);
            if (item_Product.Purchasable)
            {
                purchaseCostObject.SetActive(true);
                purchaseCostText.Text = StoreManager.Instance.GetProductFormattedCost(item_Product);
            }
            else
            {
                purchaseCostObject.SetActive(false);
            }
            cracksSprite.material.SetFloat("_Cutoff", 1f);
        }
        else if (m_item.Unlocked)
        {
            costObject.SetActive(false);
            purchaseCostObject.SetActive(false);
        }
		else if (m_item.Unlocked)
		{
			costObject.SetActive(false);
			purchaseCostObject.SetActive(false);
		}
		else
		{
			if (m_item.CurrentCost == 0)
			{
				costObject.SetActive(false);
			}
			else
			{
				costObject.SetActive(true);
				costText.Text = m_item.CurrentCost.ToString("N0");
			}
			purchaseCostObject.SetActive(false);
			cracksSprite.material.SetFloat("_Cutoff", 1f);
		}
		starCenterNode.gameObject.SetActive(false);
		Item_ChallengeStar item_ChallengeStar = m_item as Item_ChallengeStar;
		if ((bool)item_ChallengeStar)
		{
			List<Achievement> activeAchievements = AchievementManager.Instance.ActiveAchievements;
			if (item_ChallengeStar.index < activeAchievements.Count)
			{
				m_item.description = "\"" + activeAchievements[item_ChallengeStar.index].description + "\"";
				m_item.descriptionTextScale = activeAchievements[item_ChallengeStar.index].descriptionTextScale;
			}
		}
		if (m_item == equippedItem || ((bool)item_Product && item_Product is Item_NonConsumableProduct && item_Product.Unlocked))
		{
			equippedObject.SetActive(true);
			lockedObject.SetActive(false);
			unlockedObject.SetActive(true);
			unlockedObject.transform.localScale = unlockedObjectEquippedScale;
			downloadableObject.SetActive(false);
			return;
		}
		equippedObject.SetActive(false);
		downloadableObject.SetActive(!m_item.IsCached);
		if (m_item.Unlocked)
		{
			lockedObject.SetActive(false);
			unlockedObject.SetActive(true);
			unlockedObject.transform.localScale = unlockedObjectNormalScale;
		}
		else
		{
			unlockedObject.SetActive(false);
			lockedObject.SetActive(true);
			lockedObject.transform.localScale = Vector3.one;
			lockedObject.transform.localRotation = Quaternion.identity;
		}
	}
}
