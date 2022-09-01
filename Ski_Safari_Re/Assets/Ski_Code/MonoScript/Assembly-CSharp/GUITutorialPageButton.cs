using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GUITutorialPageButton : GUIButton
{
	public bool moveRight = true;

	private Vector3 m_baseScale;

	public override void Click(Vector3 position)
	{
		base.Click(position);
		Go.killAllTweensWithTarget(base.gameObject.transform);
		base.transform.localScale = m_baseScale;
		Go.to(base.gameObject.transform, 0.5f, new GoTweenConfig().scale(0.5f, true).setEaseType(GoEaseType.ElasticPunch));
		if (moveRight)
		{
			GUITutorials.Instance.MoveRight();
		}
		else
		{
			GUITutorials.Instance.MoveLeft();
		}
	}

	private void Awake()
	{
		m_baseScale = base.transform.localScale;
        //base.Awake();
	}
}
