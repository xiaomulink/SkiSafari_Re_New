using UnityEngine;

public class GUIPageBlip : GUIButton
{
	public GameObject activeSprite;

	public GameObject inactiveSprite;

	public bool Active
	{
		set
		{
			if (value && !activeSprite.activeInHierarchy)
			{
				base.transform.localScale = Vector3.one;
				Go.killAllTweensWithTarget(base.transform);
				Go.to(base.transform, 0.5f, new GoTweenConfig().scale(0.2f, true).setEaseType(GoEaseType.ElasticPunch));
			}
			activeSprite.SetActive(value);
			inactiveSprite.SetActive(!value);
		}
	}

	public int PageIndex { get; set; }

	public override void Click(Vector3 position)
	{
	}
}
