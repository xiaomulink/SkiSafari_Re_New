using System.Diagnostics;

public static class MigrationUtils
{
	public static void MigrateAndroid1to3(Profile persistentProfile, ItemManager itemManager)
	{
		persistentProfile.MigrateLeaderboards();
		if (persistentProfile.GetString("last_update_version") == "1.1")
		{
			persistentProfile.MigrateStartingSkierAndSlope(itemManager);
			persistentProfile.UnmigrateString("current_costume");
			persistentProfile.UnmigrateString("current_gizmo");
			persistentProfile.UnmigrateString("current_powerup");
			persistentProfile.UnmigrateString("LastPlayerName");
			persistentProfile.Save();
		}
	}

	[Conditional("LOCAL_DEBUG")]
	private static void LocalLog(string log)
	{
	}
}
