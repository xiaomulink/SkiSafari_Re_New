using UnityEngine;

public class UUIDUtils
{
	public static string GetUniqueDeviceId()
	{
        try
        {
            //Discarded unreachable code: IL_003d, IL_004f
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");

                using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass(Application.identifier + ".UDIDUtils"))
                {
                    return androidJavaClass2.CallStatic<string>("generateUDID", new object[1] { @static });
                }
            }
        }
        catch {
            return "generateUDID64646494676449";
        }

	}

	private static string GenerateUUID()
	{
		string text = string.Empty;
		for (int i = 0; i < 5; i++)
		{
			text += Random.Range(int.MinValue, int.MaxValue).ToString("X8");
		}
		return text;
	}
}
