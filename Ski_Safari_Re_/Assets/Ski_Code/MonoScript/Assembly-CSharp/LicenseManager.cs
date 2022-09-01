using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class LicenseManager : MonoBehaviour
{
	private enum LicenseState
	{
		Licenced = 0,
		NotLicenced = 1,
		Owned = 2,
		Retry = 3
	}

	private const string m_PublicKey_Modulus_Base64 = "AOrMPoIx19Rx4Tyy6PtqPHAmy5zEg3I2zWmqukEYPjFH3aUE7rV4OfASR6KITOdAGJFTSp2Ls5e1a15iRYPeTLed/HEEo/xrDlDrL/9yRoNI0NmW2GOzNMdLC5e7evm+pzIW/woS14HxDGbDbNO+Y53tYj4yEcgTdVoK0fJEhHgkW+E1Oa8oAkzmqxl9+rsLZ0l5nlPAort4GZ0AkrR5Sb+FZNgCZo1mgUzE9eKvFULINlRFTce7pjxgc59TyOkIN/qdAiUs2XrX/8l7GrVIbRmfQQ0VCmXnlwwQEy819sesO2CCKSL8h+2XLTppY6PJaafw80kkzYL4HHogJ/ZLPhc=";

	private const string m_PublicKey_Exponent_Base64 = "AQAB";

	private const string lastResponseKey = "resp";

	private const string licenseValidToKey = "valid";

	private const string licenseGraceKey = "grace";

	public static LicenseManager Instance;

	public float checkTimeout = 5f;

	public float repeatCheckInterval = 120f;

	private static string deviceID;

	private static DateTime epoch;

	private RSAParameters m_PublicKey = default(RSAParameters);

	private AndroidJavaObject m_Activity;

	private AndroidJavaObject m_LVLCheck;

	private int m_responseCode;

	private string m_message;

	private string m_signature;

	private bool m_art;

	private string m_packageName;

	private int m_nonce;

	private LicenseState? m_lastResponse;

	private DateTime? m_licenseValidTo;

	private DateTime? m_licenseGrace;

	public bool IsRetrying
	{
		get
		{
			return m_lastResponse.HasValue && m_lastResponse.Value == LicenseState.Retry;
		}
	}

	private LicenseState? LastResponse
	{
		get
		{
			if (!m_lastResponse.HasValue)
			{
				string deviceSpecificKey = GetDeviceSpecificKey("resp");
				if (PlayerPrefs.HasKey(deviceSpecificKey))
				{
					m_lastResponse = (LicenseState)PlayerPrefs.GetInt(deviceSpecificKey);
				}
			}
			return m_lastResponse;
		}
		set
		{
			m_lastResponse = value;
			if (m_lastResponse.HasValue)
			{
				PlayerPrefs.SetInt(GetDeviceSpecificKey("resp"), (int)m_lastResponse.Value);
			}
			else
			{
				PlayerPrefs.DeleteKey(GetDeviceSpecificKey("resp"));
			}
		}
	}

	private DateTime? LicenseValidTo
	{
		get
		{
			if (!m_licenseValidTo.HasValue && ExistsDateTime("valid"))
			{
				m_licenseValidTo = LoadDataTime("valid");
			}
			return m_licenseValidTo;
		}
		set
		{
			m_licenseValidTo = value;
			if (m_licenseValidTo.HasValue)
			{
				SaveDateTime("valid", m_licenseValidTo.Value);
			}
			else
			{
				ClearDateTime("valid");
			}
		}
	}

	private DateTime? LicenseGrace
	{
		get
		{
			if (!m_licenseGrace.HasValue && ExistsDateTime("grace"))
			{
				m_licenseGrace = LoadDataTime("grace");
			}
			return m_licenseGrace;
		}
		set
		{
			m_licenseGrace = value;
			if (m_licenseGrace.HasValue)
			{
				SaveDateTime("grace", m_licenseGrace.Value);
			}
			else
			{
				ClearDateTime("grace");
			}
		}
	}

	public bool HasFinishedOrTimedOut { get; private set; }

	public bool AllowAccess()
	{
		if (m_art)
		{
			return true;
		}
		DateTime utcNow = DateTime.UtcNow;
		if (LastResponse.HasValue && LastResponse.Value == LicenseState.Owned)
		{
			return true;
		}
		if (LastResponse.HasValue && LastResponse.Value == LicenseState.Licenced)
		{
			return true;
		}
		if (LastResponse.HasValue && LastResponse.Value == LicenseState.Retry && LicenseGrace.HasValue && utcNow < LicenseGrace.Value)
		{
			return true;
		}
		return false;
	}

	public void DebugResetEverything()
	{
		LastResponse = null;
		LicenseValidTo = null;
		LicenseGrace = null;
	}

	public void DebugForceCheck()
	{
		if (m_LVLCheck == null)
		{
			StartCheck();
		}
	}

	public void DebugForceBuy()
	{
		LicenseValidTo = DateTime.UtcNow.AddYears(100);
		FinishCheck(LicenseState.Owned);
	}

	public void DebugForceUnlicensed()
	{
		LastResponse = null;
	}

	private void StartCheck()
	{
		if (m_Activity != null && m_Activity.GetRawObject() != IntPtr.Zero)
		{
			m_LVLCheck = new AndroidJavaObject("com.unity3d.plugin.lvl.ServiceBinder", m_Activity);
			if (m_LVLCheck != null && m_LVLCheck.GetRawObject() != IntPtr.Zero)
			{
				m_nonce = new System.Random().Next();
				m_LVLCheck.Call("create", m_nonce, new AndroidJavaRunnable(ReceiveResponse));
				StartCoroutine(WaitForResponse());
				return;
			}
		}
		FinishCheck(LicenseState.NotLicenced);
	}

	private void RepeatCheck()
	{
		if (ShouldCheck())
		{
			StartCheck();
		}
		else if (LastResponse.HasValue && LastResponse.Value == LicenseState.Owned)
		{
			CancelInvoke("RepeatCheck");
		}
	}

	private IEnumerator WaitForResponse()
	{
		DateTime startTime = DateTime.Now;
		while (m_LVLCheck != null && (DateTime.Now - startTime).TotalSeconds < (double)checkTimeout)
		{
			yield return 0;
		}
		if (m_LVLCheck == null)
		{
			ProcessCheck();
			yield break;
		}
		DateTime maxGrace = DateTime.UtcNow.AddDays(1.0);
		if (LicenseGrace.HasValue)
		{
			DateTime? licenseGrace = LicenseGrace;
			if (!licenseGrace.HasValue || !(licenseGrace.Value > maxGrace))
			{
				goto IL_011f;
			}
		}
		LicenseGrace = maxGrace;
		goto IL_011f;
		IL_011f:
		FinishCheck(LicenseState.Retry);
	}

	private void ReceiveResponse()
	{
		m_responseCode = m_LVLCheck.Get<int>("_arg0");
		m_message = m_LVLCheck.Get<string>("_arg1");
		m_signature = m_LVLCheck.Get<string>("_arg2");
		m_LVLCheck = null;
	}

	private void ProcessCheck()
	{
		//Discarded unreachable code: IL_01ab
		string text = null;
		if (m_responseCode == 0 || m_responseCode == 1 || m_responseCode == 2)
		{
			if (string.IsNullOrEmpty(m_message) || string.IsNullOrEmpty(m_signature))
			{
				FinishCheck(LicenseState.NotLicenced);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(m_message);
			byte[] rgbSignature = Convert.FromBase64String(m_signature);
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.ImportParameters(m_PublicKey);
			SHA1Managed sHA1Managed = new SHA1Managed();
			if (!rSACryptoServiceProvider.VerifyHash(sHA1Managed.ComputeHash(bytes), CryptoConfig.MapNameToOID("SHA1"), rgbSignature))
			{
				FinishCheck(LicenseState.NotLicenced);
				return;
			}
			int num = m_message.IndexOf(':');
			string text2;
			if (num == -1)
			{
				text2 = m_message;
				text = string.Empty;
			}
			else
			{
				text2 = m_message.Substring(0, num);
				text = ((num < m_message.Length) ? m_message.Substring(num + 1) : string.Empty);
			}
			string[] array = text2.Split('|');
			try
			{
				if (m_responseCode != Convert.ToInt32(array[0]))
				{
					FinishCheck(LicenseState.NotLicenced);
					return;
				}
				if (m_nonce != Convert.ToInt32(array[1]))
				{
					FinishCheck(LicenseState.NotLicenced);
					return;
				}
				if (m_packageName != array[2])
				{
					FinishCheck(LicenseState.NotLicenced);
					return;
				}
			}
			catch (Exception)
			{
				FinishCheck(LicenseState.NotLicenced);
				return;
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (!string.IsNullOrEmpty(text))
		{
			DecodeExtras(text, dictionary);
		}
		switch (m_responseCode)
		{
		case 0:
		case 2:
		{
			if (dictionary.ContainsKey("VT"))
			{
				LicenseValidTo = FromMilisecondsSinceEpoch(Convert.ToInt64(dictionary["VT"]));
			}
			else
			{
				LicenseValidTo = DateTime.UtcNow.AddHours(1.0);
			}
			if (dictionary.ContainsKey("GT"))
			{
				LicenseGrace = FromMilisecondsSinceEpoch(Convert.ToInt64(dictionary["GT"]));
			}
			else
			{
				LicenseGrace = DateTime.UtcNow.AddHours(1.0);
			}
			DateTime? licenseValidTo = LicenseValidTo;
			if (licenseValidTo.HasValue && licenseValidTo.Value > DateTime.UtcNow.AddHours(12.0))
			{
				FinishCheck(LicenseState.Owned);
			}
			else
			{
				FinishCheck(LicenseState.Licenced);
			}
			break;
		}
		case 1:
			FinishCheck(LicenseState.NotLicenced);
			break;
		case 4:
		case 5:
		case 257:
		{
			DateTime dateTime = DateTime.UtcNow.AddDays(1.0);
			if (LicenseGrace.HasValue)
			{
				DateTime? licenseGrace = LicenseGrace;
				if (!licenseGrace.HasValue || !(licenseGrace.Value > dateTime))
				{
					goto IL_038a;
				}
			}
			LicenseGrace = dateTime;
			goto IL_038a;
		}
		default:
			{
				FinishCheck(LicenseState.NotLicenced);
				break;
			}
			IL_038a:
			FinishCheck(LicenseState.Retry);
			break;
		}
	}

	private void FinishCheck(LicenseState state)
	{
		if (!LastResponse.HasValue || LastResponse.Value != state)
		{
			switch (state)
			{
			case LicenseState.Licenced:
			case LicenseState.Owned:
				AnalyticsManager.Instance.SendEvent("Licensed");
				break;
			case LicenseState.NotLicenced:
				AnalyticsManager.Instance.SendEvent("UnLicensed");
				break;
			}
		}
		if (state == LicenseState.Retry)
		{
			if (!LastResponse.HasValue || (LastResponse.Value != LicenseState.Owned && LastResponse.Value != 0))
			{
				LastResponse = state;
			}
		}
		else
		{
			LastResponse = state;
		}
		if (state == LicenseState.Owned)
		{
			CancelInvoke("RepeatCheck");
		}
		HasFinishedOrTimedOut = true;
		m_nonce = 0;
	}

	[Conditional("LICENSE_DEBUG")]
	private static void LocalLog(string log)
	{
	}

	private static long ToMilisecondsSinceEpoch(DateTime dateTime)
	{
		return (dateTime.ToUniversalTime() - epoch).Ticks / 10000;
	}

	private static DateTime FromMilisecondsSinceEpoch(long milis)
	{
		return epoch.AddTicks(milis * 10000);
	}

	private static string GetDeviceSpecificKey(string key)
	{
		return (deviceID + key).GetHashCode().ToString();
	}

	private static void SaveDateTime(string key, DateTime dateTime)
	{
		PlayerPrefs.SetString(GetDeviceSpecificKey(key), ToMilisecondsSinceEpoch(dateTime).ToString());
	}

	private static bool ExistsDateTime(string key)
	{
		return PlayerPrefs.HasKey(GetDeviceSpecificKey(key));
	}

	private static void ClearDateTime(string key)
	{
		PlayerPrefs.DeleteKey(GetDeviceSpecificKey(key));
	}

	private static DateTime LoadDataTime(string key)
	{
		long result = 0L;
		long.TryParse(PlayerPrefs.GetString(GetDeviceSpecificKey(key)), out result);
		return FromMilisecondsSinceEpoch(result);
	}

	internal static void DecodeExtras(string query, Dictionary<string, string> result)
	{
		if (query.Length == 0)
		{
			return;
		}
		int length = query.Length;
		int num = 0;
		bool flag = true;
		while (num <= length)
		{
			int num2 = -1;
			int num3 = -1;
			for (int i = num; i < length; i++)
			{
				if (num2 == -1 && query[i] == '=')
				{
					num2 = i + 1;
				}
				else if (query[i] == '&')
				{
					num3 = i;
					break;
				}
			}
			if (flag)
			{
				flag = false;
				if (query[num] == '?')
				{
					num++;
				}
			}
			string key;
			if (num2 == -1)
			{
				key = null;
				num2 = num;
			}
			else
			{
				key = WWW.UnEscapeURL(query.Substring(num, num2 - num - 1));
			}
			if (num3 < 0)
			{
				num = -1;
				num3 = query.Length;
			}
			else
			{
				num = num3 + 1;
			}
			string value = WWW.UnEscapeURL(query.Substring(num2, num3 - num2));
			result.Add(key, value);
			if (num == -1)
			{
				break;
			}
		}
	}

	private bool ShouldCheck()
	{
		if (m_art)
		{
			return false;
		}
		if (!LastResponse.HasValue || !LicenseValidTo.HasValue || LastResponse.Value != LicenseState.Owned)
		{
			return m_LVLCheck == null;
		}
		return false;
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Instance = this;
		deviceID = UUIDUtils.GetUniqueDeviceId();
		epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}

	private IEnumerator Start()
	{
		//m_PublicKey.Modulus = Convert.FromBase64String("AOrMPoIx19Rx4Tyy6PtqPHAmy5zEg3I2zWmqukEYPjFH3aUE7rV4OfASR6KITOdAGJFTSp2Ls5e1a15iRYPeTLed/HEEo/xrDlDrL/9yRoNI0NmW2GOzNMdLC5e7evm+pzIW/woS14HxDGbDbNO+Y53tYj4yEcgTdVoK0fJEhHgkW+E1Oa8oAkzmqxl9+rsLZ0l5nlPAort4GZ0AkrR5Sb+FZNgCZo1mgUzE9eKvFULINlRFTce7pjxgc59TyOkIN/qdAiUs2XrX/8l7GrVIbRmfQQ0VCmXnlwwQEy819sesO2CCKSL8h+2XLTppY6PJaafw80kkzYL4HHogJ/ZLPhc=");
		//m_PublicKey.Exponent = Convert.FromBase64String("AQAB");
		//new SHA1CryptoServiceProvider();
        try
        {
          //  m_Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        catch { }
		//m_packageName = m_Activity.Call<string>("getPackageName", new object[0]);
		/*AndroidJavaClass systemProperties = new AndroidJavaClass("android.os.SystemProperties");
		string runtime = systemProperties.CallStatic<string>("get", new object[2] { "persist.sys.dalvik.vm.lib", "libdvm.so" });
		if (runtime == "libart.so")
		{
			m_art = true;
		}
		if (ShouldCheck())
		{
			StartCheck();
			InvokeRepeating("RepeatCheck", repeatCheckInterval, repeatCheckInterval);
		}
		else
		{
			HasFinishedOrTimedOut = true;
		}*/
		yield return null;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public override string ToString()
	{
		return "License Status\nLast response code: " + ((!LastResponse.HasValue) ? "Not received a response" : LastResponse.Value.ToString()) + "\nValid to: " + ((!LicenseValidTo.HasValue) ? "Not received" : LicenseValidTo.Value.ToLocalTime().ToString()) + "\nGrace to: " + ((!LicenseGrace.HasValue) ? "Not received" : LicenseGrace.Value.ToLocalTime().ToString()) + "\nAllow Access: " + AllowAccess();
	}
}
