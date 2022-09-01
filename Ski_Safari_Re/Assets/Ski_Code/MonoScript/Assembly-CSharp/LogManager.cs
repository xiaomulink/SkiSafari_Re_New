using System;
using System.Collections.Generic;
using UnityEngine;

public class LogManager
{
	private static int m_maxLogCount;

	private static List<string> s_logs;

	private static DateTime s_lastLogDate;

	public static List<string> Logs
	{
		get
		{
			return s_logs;
		}
	}

	public static DateTime LastLogDate
	{
		get
		{
			return s_lastLogDate;
		}
	}

	static LogManager()
	{
		m_maxLogCount = 1000;
		s_logs = new List<string>();
		Application.RegisterLogCallback(OnLog);
	}

	public static void Initialise()
	{
	}

	private static void OnLog(string log, string stackTrace, LogType type)
	{
		if (type != LogType.Log || DebugManager.DebugEnabled)
		{
			s_logs.Add(log);
			s_lastLogDate = DateTime.Now;
			while (s_logs.Count > m_maxLogCount)
			{
				s_logs.RemoveAt(0);
			}
		}
	}
}
