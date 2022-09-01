public class GUIHDGraphicsButton : GUIToggleButton
{
	protected override bool Toggled
	{
		get
		{
			return AssetManager.HDEnabled;
		}
		set
		{
			AssetManager.HDEnabled = !AssetManager.HDEnabled;
		}
	}

	protected override void OnEnable()
	{
		if (AssetManager.CanEnableHD)
		{
			base.OnEnable();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
