using UnityEngine;

public class GUIProfileSummary : GUIButton
{
	public TextMesh titleText;

	public Material selectedTitleMaterial;

	public Material unselectedTitleMaterial;

	public GameObject selectedSprite;

	public GameObject unselectedSprite;

	public TextMesh rankText;

	public Transform badgeNode;

	public float badgeScale = 0.5f;

	public TextMesh coinCountText;

	public TextMesh shopItemsText;

	public Transform background;

	private Profile m_profile;

	public Profile Profile
	{
		get
		{
			return m_profile;
		}
		set
		{
			m_profile = value;
			coinCountText.text = m_profile.Coins.ToString("N0");
			int @int = m_profile.GetInt("current_level");
			LevelManager.LevelDescriptor levelDescriptor = LevelManager.Instance.levelDescriptors[Mathf.Clamp(@int - 1, 0, LevelManager.Instance.levelDescriptors.Length - 1)];
			rankText.text = string.Format("Rank {0}", @int);
			GameObject prefab = ((!levelDescriptor.smallBadge) ? levelDescriptor.largeBadge : levelDescriptor.smallBadge);
			GameObject gameObject = TransformUtils.Instantiate(prefab, badgeNode);
			gameObject.transform.localScale *= badgeScale;
			shopItemsText.text = string.Format("{0} shop items", m_profile.GetInt("unlocked_item_count"));
		}
	}

	public void SetSelected(bool selected)
	{
		titleText.GetComponent<Renderer>().material = ((!selected) ? unselectedTitleMaterial : selectedTitleMaterial);
		selectedSprite.SetActive(selected);
		unselectedSprite.SetActive(!selected);
		Vector3 localPosition = background.transform.localPosition;
		localPosition.z = (selected ? 1 : (-1));
		background.transform.localPosition = localPosition;
	}
}
