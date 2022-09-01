using UnityEngine;

public class DebugMenu_Licensing : DebugMenu
{
	public override string Name
	{
		get
		{
			return "Licensing";
		}
	}

	public override bool IsAvailable()
	{
		return (bool)LicenseManager.Instance && (!SkiGameManager.Instance || SkiGameManager.Instance.TitleScreenActive) && (!GUITutorials.Instance || !GUITutorials.Instance.AutoShow);
	}

	public override void Draw()
	{
		GUILayout.Label(LicenseManager.Instance.ToString());
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Reset Everything", GUILayout.Height(40f)))
		{
			LicenseManager.Instance.DebugResetEverything();
		}
		if (GUILayout.Button("Force Check", GUILayout.Height(40f)))
		{
			LicenseManager.Instance.DebugForceCheck();
		}
		if (GUILayout.Button("Force Buy", GUILayout.Height(40f)))
		{
			LicenseManager.Instance.DebugForceBuy();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		if (LicenseManager.Instance.IsInvoking("RepeatCheck"))
		{
			if (GUILayout.Button("Cancel Repeat Check", GUILayout.Height(40f)))
			{
				LicenseManager.Instance.CancelInvoke("RepeatCheck");
			}
		}
		else if (GUILayout.Button("Start Repeat Check", GUILayout.Height(40f)))
		{
			LicenseManager.Instance.InvokeRepeating("RepeatCheck", 0f, LicenseManager.Instance.repeatCheckInterval);
		}
	}
}
