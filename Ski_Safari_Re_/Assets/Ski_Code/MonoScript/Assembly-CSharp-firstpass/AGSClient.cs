using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AGSClient : MonoBehaviour
{
	public const string serviceName = "AmazonGameCircle";

	public const AmazonLogging.AmazonLoggingLevel errorLevel = AmazonLogging.AmazonLoggingLevel.Verbose;

	private static bool IsReady;

	private static bool SupportsLeaderboards;

	private static bool SupportsAchievements;

	private static bool SupportsWhispersync;

	private static AmazonJavaWrapper JavaObject;

	private static readonly string PROXY_CLASS_NAME;

	[method: MethodImpl(32)]
	public static event Action ServiceReadyEvent;

	[method: MethodImpl(32)]
	public static event Action<string> ServiceNotReadyEvent;

	static AGSClient()
	{
		PROXY_CLASS_NAME = "com.amazon.ags.api.unity.AmazonGamesClientProxyImpl";
		JavaObject = new AmazonJavaWrapper();
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(PROXY_CLASS_NAME))
		{
			if (androidJavaClass.GetRawClass() == IntPtr.Zero)
			{
				LogGameCircleWarning("No java class " + PROXY_CLASS_NAME + " present, can't use AGSClient");
			}
			else
			{
				JavaObject.setAndroidJavaObject(androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]));
			}
		}
	}

	public static void Init()
	{
		Init(false, false, false);
	}

	public static void Init(bool supportsLeaderboards, bool supportsAchievements, bool supportsWhispersync)
	{
		SupportsLeaderboards = supportsLeaderboards;
		SupportsAchievements = supportsAchievements;
		SupportsWhispersync = supportsWhispersync;
		JavaObject.Call("init", supportsLeaderboards, supportsAchievements, supportsWhispersync);
	}

	public static void SetPopUpEnabled(bool enabled)
	{
		JavaObject.Call("setPopupEnabled", enabled);
	}

	public static void SetPopUpLocation(GameCirclePopupLocation location)
	{
		JavaObject.Call("setPopUpLocation", location.ToString());
	}

	public static void ServiceReady(string empty)
	{
		Log("Client GameCircle - Service is ready");
		AGSProfilesClient.SetupAGSProfilesClient();
		if (SupportsLeaderboards)
		{
			AGSLeaderboardsClient.SetupAGSLeaderboardsClient();
		}
		if (SupportsAchievements)
		{
			AGSAchievementsClient.SetupAGSAchievementsClient();
		}
		if (SupportsWhispersync)
		{
			AGSWhispersyncClient.SetupAGSWhispersyncClient();
		}
		IsReady = true;
		if (AGSClient.ServiceReadyEvent != null)
		{
			AGSClient.ServiceReadyEvent();
		}
	}

	public static bool IsServiceReady()
	{
		return IsReady;
	}

	public static void release()
	{
		JavaObject.Call("release");
	}

	public static void ServiceNotReady(string param)
	{
		IsReady = false;
		if (AGSClient.ServiceNotReadyEvent != null)
		{
			AGSClient.ServiceNotReadyEvent(param);
		}
	}

	public static void ShowGameCircleOverlay()
	{
		JavaObject.Call("showGameCircleOverlay");
	}

	public static void ShowSignInPage()
	{
		JavaObject.Call("showSignInPage");
	}

	public static void LogGameCircleError(string errorMessage)
	{
		AmazonLogging.LogError(AmazonLogging.AmazonLoggingLevel.Verbose, "AmazonGameCircle", errorMessage);
	}

	public static void LogGameCircleWarning(string errorMessage)
	{
		AmazonLogging.LogWarning(AmazonLogging.AmazonLoggingLevel.Verbose, "AmazonGameCircle", errorMessage);
	}

	public static void Log(string message)
	{
		AmazonLogging.Log(AmazonLogging.AmazonLoggingLevel.Verbose, "AmazonGameCircle", message);
	}
}
