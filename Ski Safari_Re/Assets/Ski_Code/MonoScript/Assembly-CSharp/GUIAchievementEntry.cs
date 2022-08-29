using UnityEngine;

public class GUIAchievementEntry : MonoBehaviour
{
	public GUIDropShadowText nameText;

	public Vector3 nameTextOffset = new Vector3(0.7f, 0f, 0f);

	public Vector3 nameTextIconOffset = new Vector3(1.5f, 0f, 0f);

	public GameObject starSprite;

	public GameObject newSprite;

	public Renderer icon;

	private Achievement m_achievement;

	private string m_achievementName;

	public string AchievementName
	{
		get
		{
			return m_achievementName;
		}
	}

	public Achievement Achievement
	{
		get
		{
			return m_achievement;
		}
		set
		{
			m_achievement = value;
			m_achievementName = value.name;
			if ((bool)m_achievement.requiredItem)
			{
				nameText.transform.localPosition = nameTextIconOffset;
				AssetManager.UpdateTexture(icon, m_achievement.requiredItem.iconTextureName);
				icon.enabled = true;
			}
			else
			{
				AssetManager.UnloadTexture(icon);
				nameText.transform.localPosition = nameTextOffset;
				icon.enabled = false;
			}
		}
	}

	private void OnDestroy()
	{
		AssetManager.UnloadTexture(icon);
	}
}
