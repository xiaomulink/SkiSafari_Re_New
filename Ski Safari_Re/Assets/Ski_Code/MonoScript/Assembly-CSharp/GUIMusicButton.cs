public class GUIMusicButton : GUIToggleButton
{
	protected override bool Toggled
	{
		get
		{
			return SoundManager.Instance.MusicEnabled;
		}
		set
		{
			SoundManager.Instance.MusicEnabled = value;
		}
	}
}
