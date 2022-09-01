using System.Collections.Generic;
using UnityEngine;

public class GUIShopTab : GUIButton
{
	public GUIShopTabButton tabButton;

	public GameObject newObject;

	public GUIShopItem itemPrefab;

	public GUIShopItemSetTitle itemSetTitlePrefab;

	public float itemSeparation = 6f;

	public float itemSetTitleSeparation = 4f;

	public float itemMoveTime = 0.25f;

	public float itemFadeOutTime = 0.25f;

	public float itemFadeInTime = 0.5f;

	public Vector3 itemNormalScale = new Vector3(0.75f, 0.75f, 0.75f);

	public Vector3 itemFocusedScale = Vector3.one;

	public int visibleItemsLeftRight = 1;

	public float minMoveSpeed = 20f;

	public float manualMoveSpeed = 40f;

	public float flingMinMoveSpeed = 10f;

	private List<ItemSet> m_itemSets = new List<ItemSet>();

	private Item[] m_items;

	private GameObject m_itemContainer;

	public GUIShopItem[] m_guiItems;

	public int m_selectedItemIndex = -1;

	private ItemSet m_selectedItemSet;

	private bool m_showing;

	private bool m_locked;

	private bool m_pressed;

	private bool m_dragging;

	private int m_pressedItemIndex = -1;

	private float m_pos;

	private float m_targetPos;

	private float m_speed;

	private float m_deceleration;

	private float m_lastUpdateTime;

	private GoTweenConfig m_tweenToNormalScale;

	private GoTweenConfig m_tweenToFocusedScale;

	public GameObject ItemContainer
	{
		get
		{
			return m_itemContainer;
		}
	}

	public bool Show
	{
		get
		{
			return m_showing;
		}
		set
		{
			if (value == m_showing)
			{
				return;
			}
			m_showing = value;
			base.gameObject.SetActive(value);
			tabButton.SetToggled(value);
			UpdateState();
			if (value)
			{
				GUIShopItem[] guiItems = m_guiItems;
				foreach (GUIShopItem gUIShopItem in guiItems)
				{
					gUIShopItem.OnTabVisible();
				}
			}
			else
			{
				GUIShopItem[] guiItems2 = m_guiItems;
				foreach (GUIShopItem gUIShopItem2 in guiItems2)
				{
					gUIShopItem2.OnTabHidden();
				}
			}
		}
	}

	public bool Locked
	{
		get
		{
			return m_locked;
		}
		set
		{
			m_locked = value;
			if (m_locked)
			{
				SetTargetItem(m_selectedItemIndex, minMoveSpeed);
				m_pressed = false;
				m_dragging = false;
			}
		}
	}

	public int SelectedItemIndex
	{
		get
		{
			return m_selectedItemIndex;
		}
	}

	public int RelativeSelectedItemIndex
	{
		get
		{
			int num = m_selectedItemIndex;
			foreach (ItemSet itemSet in m_itemSets)
			{
				if (itemSet != SelectedItemSet)
				{
					num -= itemSet.items.Length;
					continue;
				}
				return num;
			}
			return num;
		}
	}

	public Item SelectedItem
	{
		get
		{
			return m_items[m_selectedItemIndex];
		}
	}

	public bool CanMoveLeft
	{
		get
		{
			return m_selectedItemIndex > 0;
		}
	}

	public bool CanMoveRight
	{
		get
		{
			return m_selectedItemIndex < m_items.Length - 1;
		}
	}

	public int ItemCount
	{
		get
		{
			return m_items.Length;
		}
	}

	public ItemSet SelectedItemSet
	{
		get
		{
			return m_selectedItemSet;
		}
	}

	public GUIShopItem SelectedGUIItem
	{
		get
		{
			return m_guiItems[m_selectedItemIndex];
		}
	}

	private void Awake()
	{
		m_tweenToNormalScale = new GoTweenConfig().scale(itemNormalScale);
		m_tweenToFocusedScale = new GoTweenConfig().scale(itemFocusedScale);
	}

	public bool ContainsNonConsumableProduct()
	{
		Item[] items = m_items;
		foreach (Item item in items)
		{
			if (item is Item_NonConsumableProduct)
			{
				return true;
			}
		}
		return false;
	}

	public void EquipItem()
	{
		int num = m_selectedItemIndex;
		foreach (ItemSet itemSet in m_itemSets)
		{
			if (itemSet == SelectedItemSet)
			{
				break;
			}
			num -= itemSet.items.Length;
		}
		SelectedItemSet.SelectItem(num, true);
	}

	public void UnequipItem()
	{
		SelectedItemSet.SelectItem(m_selectedItemSet.defaultIndex, true);
		UpdateState();
	}

	public void MoveLeft()
	{
		SetTargetItem(Mathf.Max(0, m_selectedItemIndex - 1), manualMoveSpeed);
	}

	public void MoveRight()
	{
		SetTargetItem(Mathf.Min(m_items.Length - 1, m_selectedItemIndex + 1), manualMoveSpeed);
	}

	public int GetFirstNewItemIndex()
	{
		for (int i = 0; i < m_items.Length; i++)
		{
			if (m_items[i].IsNew)
			{
				return i;
			}
		}
		return -1;
	}

	public bool TrySnapToItem(Item item)
	{
		for (int i = 0; i < m_items.Length; i++)
		{
			if (item == m_items[i])
			{
				SnapToItemIndex(i);
				return true;
			}
		}
		return false;
	}

	public void SnapToItemIndex(int itemIndex)
	{
		m_selectedItemIndex = itemIndex;
		m_targetPos = (m_pos = CalculateItemPos(itemIndex));
		bool flag = false;
		for (int i = 0; i < m_guiItems.Length; i++)
		{
			m_guiItems[i].transform.localScale = ((i != itemIndex || flag) ? itemNormalScale : itemFocusedScale);
		}
		Vector3 localPosition = m_itemContainer.transform.localPosition;
		localPosition.x = 0f - m_pos;
		m_itemContainer.transform.localPosition = localPosition;
		m_guiItems[m_selectedItemIndex].IsNew = false;
	}

	public void SetTargetItemIndex(int itemIndex)
	{
		SetTargetItem(itemIndex, manualMoveSpeed);
	}

	private void FadeItem(int index, float alpha, float time)
	{
	}

	public void Setup(GUIShopTabButton tabButton, string[] itemSetNames)
	{
		this.tabButton = tabButton;
		int num = 0;
		for (int i = 0; i < itemSetNames.Length; i++)
		{
			ItemSet itemSet = ItemManager.Instance.GetItemSet(itemSetNames[i]);
			if (!(itemSet.name == "gift"))
			{
				m_itemSets.Add(itemSet);
				num += itemSet.items.Length;
			}
		}
		m_selectedItemSet = m_itemSets[0];
		m_items = new Item[num];
		int num2 = 0;
		foreach (ItemSet itemSet2 in m_itemSets)
		{
			Item[] items = itemSet2.items;
			foreach (Item item in items)
			{
				m_items[num2++] = item;
			}
		}
		m_itemContainer = new GameObject("Items");
		m_itemContainer.transform.parent = base.transform;
		m_itemContainer.transform.localPosition = new Vector3(0f - itemSetTitleSeparation, 0f, 0f);
		m_guiItems = new GUIShopItem[m_items.Length];
		Vector3 zero = Vector3.zero;
		num2 = 0;
		foreach (ItemSet itemSet3 in m_itemSets)
		{
			if ((bool)itemSetTitlePrefab)
			{
				GUIShopItemSetTitle gUIShopItemSetTitle = TransformUtils.Instantiate(itemSetTitlePrefab, m_itemContainer.transform);
				gUIShopItemSetTitle.title.Text = itemSet3.displayName;
				gUIShopItemSetTitle.titlePrefix.Text = itemSet3.displayNamePrefix;
				gUIShopItemSetTitle.transform.localPosition = zero;
			}
			zero.x += itemSetTitleSeparation;
			Item[] items2 = itemSet3.items;
			foreach (Item item2 in items2)
			{
				m_guiItems[num2] = TransformUtils.Instantiate(itemPrefab, m_itemContainer.transform);
				m_guiItems[num2].Item = item2;
				m_guiItems[num2].Item_ = item2;
				m_guiItems[num2].transform.localPosition = zero;
				num2++;
				zero.x += itemSeparation;
			}
		}
		SnapToItemIndex(Mathf.Max(0, m_itemSets[0].CurrentIndex));
		base.gameObject.SetActive(false);
		m_showing = false;
	}

	public void UpdateState()
	{
		if (Show)
		{
			int num = 0;
			foreach (ItemSet itemSet in m_itemSets)
			{
				Item currentItem = itemSet.CurrentItem;
				for (int i = 0; i < itemSet.items.Length; i++)
				{
					GUIShopItem gUIShopItem = m_guiItems[num++];
					gUIShopItem.UpdateState(currentItem);
				}
			}
			tabButton.newObject.SetActive(false);
			return;
		}
		bool active = false;
		GUIShopItem[] guiItems = m_guiItems;
		foreach (GUIShopItem gUIShopItem2 in guiItems)
		{
			if (gUIShopItem2.Item.IsNew)
			{
				active = true;
				break;
			}
		}
		tabButton.newObject.SetActive(active);
	}

	public override void Click(Vector3 position)
	{
		m_pressed = true;
		m_lastUpdateTime = Time.realtimeSinceStartup;
		float pos = position.x + m_pos;
		m_pressedItemIndex = FindNearestItem(pos, itemSeparation * 0.5f, 0f);
	}

	public override void Drag(Vector3 positionDelta)
	{
		m_dragging = true;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!m_locked)
		{
			float num = realtimeSinceStartup - m_lastUpdateTime;
			if (num > float.Epsilon)
			{
				m_speed = (0f - positionDelta.x) / num;
				m_pos -= positionDelta.x;
			}
			else
			{
				m_speed = 0f;
			}
		}
		m_lastUpdateTime = realtimeSinceStartup;
	}

	public override void Release()
	{
		if (m_locked)
		{
			return;
		}
		m_pressed = false;
		m_lastUpdateTime = Time.realtimeSinceStartup;
		if (m_dragging)
		{
			float num = 0.25f;
			int targetItemIndex = m_selectedItemIndex;
			if (!Mathf.Approximately(m_speed, 0f))
			{
				float num2 = m_speed * m_speed / (2f * (m_speed / num));
				targetItemIndex = FindNearestItem(m_pos + num2, itemSeparation * 0.5f, itemSetTitleSeparation * 0.5f);
				if (m_speed > flingMinMoveSpeed)
				{
					int a = FindNearestItem(m_pos, 0f, 0f);
					targetItemIndex = Mathf.Max(a, targetItemIndex);
				}
				else if (m_speed < 0f - flingMinMoveSpeed)
				{
					int a2 = FindNearestItem(m_pos, itemSeparation, itemSetTitleSeparation);
					targetItemIndex = Mathf.Min(a2, targetItemIndex);
				}
				targetItemIndex = Mathf.Clamp(targetItemIndex, 0, m_items.Length - 1);
			}
			SetTargetItem(targetItemIndex, minMoveSpeed);
			m_dragging = false;
		}
		else
		{
			SetTargetItem(m_pressedItemIndex, manualMoveSpeed);
		}
	}

	private void SetTargetItem(int targetItemIndex, float minSpeed)
	{
		m_targetPos = CalculateItemPos(targetItemIndex);
		float num = m_targetPos - m_pos;
		if (num > 0f)
		{
			m_speed = Mathf.Max(m_speed, minSpeed);
		}
		else if (num < 0f)
		{
			m_speed = Mathf.Min(m_speed, 0f - minSpeed);
		}
		else
		{
			m_speed = 0f;
		}
	}

	private int FindNearestItem(float pos, float threshold, float setThreshold)
	{
		int num = 0;
		foreach (ItemSet itemSet in m_itemSets)
		{
			pos -= itemSetTitleSeparation;
			for (int i = 0; i < itemSet.items.Length - 1; i++)
			{
				if (pos < threshold)
				{
					return num;
				}
				pos -= itemSeparation;
				num++;
			}
			if (pos < threshold + setThreshold)
			{
				return num;
			}
			pos -= itemSeparation;
			num++;
		}
		return Mathf.Min(m_items.Length - 1, num);
	}

	private float CalculateItemPos(int itemIndex)
	{
		float num = 0f;
		foreach (ItemSet itemSet in m_itemSets)
		{
			num += itemSetTitleSeparation;
			for (int i = 0; i < itemSet.items.Length; i++)
			{
				if (itemIndex <= 0)
				{
					return num;
				}
				itemIndex--;
				num += itemSeparation;
			}
		}
		return num;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		m_targetPos = (m_pos = CalculateItemPos(m_selectedItemIndex));
	}

	private void Update()
	{
		if (!m_pressed)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num = realtimeSinceStartup - m_lastUpdateTime;
			float num2 = m_targetPos - m_pos;
			if (!Mathf.Approximately(num2, 0f))
			{
				m_deceleration = (0f - m_speed * m_speed) / (2f * num2);
				m_speed += m_deceleration * num;
				if (m_deceleration > 0f)
				{
					m_speed = Mathf.Min(0f, m_speed);
				}
				else
				{
					m_speed = Mathf.Max(0f, m_speed);
				}
				m_pos += m_speed * num;
			}
			else
			{
				m_speed = 0f;
			}
			m_lastUpdateTime = realtimeSinceStartup;
		}
        try
        {
            m_pos = Mathf.Clamp(m_pos, itemSetTitleSeparation, itemSetTitleSeparation * (float)m_itemSets.Count + itemSeparation * (float)(m_items.Length - 1));
        }
        catch
        {

        }
		int num3 = FindNearestItem(m_pos, itemSeparation * 0.5f, itemSetTitleSeparation * 0.5f);
		if (num3 != m_selectedItemIndex)
		{
			bool flag = false;
			if (m_selectedItemIndex != -1)
			{
				m_guiItems[m_selectedItemIndex].IsNew = false;
			}
			if (m_selectedItemIndex != -1)
			{
				Go.killAllTweensWithTarget(m_guiItems[m_selectedItemIndex].transform);
				Go.to(m_guiItems[m_selectedItemIndex].transform, itemMoveTime, m_tweenToNormalScale);
			}
			m_selectedItemIndex = num3;
			Go.killAllTweensWithTarget(m_guiItems[m_selectedItemIndex].transform);
			Go.to(m_guiItems[m_selectedItemIndex].transform, itemMoveTime, m_tweenToFocusedScale);
			int num4 = m_selectedItemIndex;
			for (int i = 0; i < m_itemSets.Count; i++)
			{
				int num5 = m_itemSets[i].items.Length;
				if (num4 < num5)
				{
					m_selectedItemSet = m_itemSets[i];
					break;
				}
				num4 -= num5;
			}
			GUIShop.Instance.UpdateItemDetails();
		}
		Vector3 localPosition = m_itemContainer.transform.localPosition;
		localPosition.x = 0f - m_pos;
		m_itemContainer.transform.localPosition = localPosition;
	}
}
