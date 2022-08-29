using UnityEngine;

public static class AppInfo
{
	private static string s_version;

	public static string Version
	{
		get
		{
			if (string.IsNullOrEmpty(s_version))
			{
                /*
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						using (AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", new object[0]))
						{
							int num = 0;
							using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getPackageInfo", new object[2]
							{
								androidJavaObject.Call<string>("getPackageName", new object[0]),
								num
							}))
							{
								s_version = androidJavaObject3.Get<string>("versionName");
							}
						}
					}
				}*/
			}
			return "1.5.3";
		}
	}

	public static string PlatformName
	{
		get
		{
			return "Android";
		}
	}
}
