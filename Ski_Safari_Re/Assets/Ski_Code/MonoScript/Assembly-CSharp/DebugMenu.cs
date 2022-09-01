using System;

public abstract class DebugMenu : IDisposable
{
	public abstract string Name { get; }

	public bool Active { get; set; }

	public virtual bool IsAvailable()
	{
		return (bool)SkiGameManager.Instance && SkiGameManager.Instance.TitleScreenActive && (!GUITutorials.Instance || !GUITutorials.Instance.AutoShow);
	}

	public abstract void Draw();

	public virtual void Update()
	{
	}

	public virtual void Dispose()
	{
	}
}
