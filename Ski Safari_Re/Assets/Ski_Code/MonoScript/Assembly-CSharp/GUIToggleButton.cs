using UnityEngine;

public abstract class GUIToggleButton : GUIButton
{
	public GameObject activeSprite;

	public GameObject inactiveSprite;

	public Renderer text;

	public Renderer subtext;

	public Material activeTextMaterial;

	public Material inactiveTextMaterial;

	protected abstract bool Toggled { get; set; }

	public void UpdateToggled()
	{
		bool toggled = Toggled;
		activeSprite.SetActive(toggled);
		inactiveSprite.SetActive(!toggled);
		if ((bool)text)
		{
			text.material = ((!toggled) ? inactiveTextMaterial : activeTextMaterial);
		}
		if ((bool)subtext)
		{
			subtext.material = ((!toggled) ? inactiveTextMaterial : activeTextMaterial);
		}
	}

	public override void Click(Vector3 position)
	{
		bool toggled = Toggled;
		Toggled = !toggled;
		bool toggled2 = Toggled;
		if (toggled2 != toggled)
		{
			base.Click(position);
			UpdateToggled();
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		UpdateToggled();
	}
}
