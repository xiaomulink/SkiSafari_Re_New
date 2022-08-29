public class GUISFXButton : GUIToggleButton
{
	protected override bool Toggled
	{
		get
		{
			return SoundManager.Instance.SFXEnabled;
		}
		set
		{
			SoundManager.Instance.SFXEnabled = value;
		}
	}
}
