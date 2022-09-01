public class GUIPauseButton : GUIToggleButton
{
	protected override bool Toggled
	{
		get
		{
			return SkiGameManager.Instance.Paused;
		}
		set
		{
            if (!SkiGameManager.Instance.isOnline)
            {
                SkiGameManager.Instance.Paused = value;
            }
            else
            {
                if (PanelManager.Find("PausePanel"))
                {
                    PanelManager.Close("PausePanel");
                }
                else
                {
                    PanelManager.Open<PausePanel>();
                }
            }
		}
	}
}
