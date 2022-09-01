using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager_Local : FileManager
{
	private string m_localDocumentsPath;

	private string m_backupDocumentsPath;

	private bool m_backupEnabled;

	public override string LocalDocumentsPath
	{
		get
		{
			return m_localDocumentsPath;
		}
		set
		{
			m_localDocumentsPath = value;
			if (!Directory.Exists(m_localDocumentsPath))
			{
				Directory.CreateDirectory(m_localDocumentsPath);
			}
		}
	}

	protected string BackupDocumentsPath
	{
		get
		{
			return m_backupDocumentsPath;
		}
		set
		{
			m_backupDocumentsPath = value;
			m_backupEnabled = !string.IsNullOrEmpty(value);
			if (m_backupEnabled && !Directory.Exists(m_backupDocumentsPath))
			{
				Directory.CreateDirectory(m_backupDocumentsPath);
			}
		}
	}

	protected bool BackupEnabled
	{
		get
		{
			return m_backupEnabled;
		}
	}

	public override bool CloudEnabled
	{
		get
		{
			return false;
		}
	}

	public override bool HasPendingDownloads
	{
		get
		{
			return false;
		}
	}

	public override bool WriteAllBytes(string fileName, byte[] bytes, bool saveToCloud, ref DateTime cloudDate, out bool wasSavedToCloud)
	{
		wasSavedToCloud = false;
		if (m_backupEnabled)
		{
			string path = Path.Combine(m_backupDocumentsPath, fileName);
			try
			{
				File.WriteAllBytes(path, bytes);
			}
			catch (Exception)
			{
			}
		}
		string path2 = Path.Combine(m_localDocumentsPath, fileName);
		bool result = false;
		try
		{
			File.WriteAllBytes(path2, bytes);
			result = true;
			return result;
		}
		catch (Exception ex2)
		{
			AnalyticsManager.Instance.SendEvent("local_save_fail", "exception", ex2.Message);
			return result;
		}
	}

	public override byte[] ReadAllBytes(string fileName, bool preferLocal, out DateTime cloudDate, out bool isCloudFile)
	{
		cloudDate = DateTime.MinValue;
		isCloudFile = false;
		string path = Path.Combine(m_localDocumentsPath, fileName);
		try
		{
			if (File.Exists(path))
			{
				return File.ReadAllBytes(path);
			}
		}
		catch (Exception)
		{
		}
		if (m_backupEnabled)
		{
			string path2 = Path.Combine(m_backupDocumentsPath, fileName);
			try
			{
				if (File.Exists(path2))
				{
					return File.ReadAllBytes(path2);
				}
			}
			catch (Exception)
			{
			}
		}
		return null;
	}

	public override List<FileInfo> GetFileList(string searchPattern)
	{
		//Discarded unreachable code: IL_0043
		try
		{
			string[] files = Directory.GetFiles(m_localDocumentsPath, searchPattern);
			if (files != null)
			{
				return GetFileInfoList(files);
			}
		}
		catch (Exception)
		{
		}
		try
		{
			return GetFileInfoList(Directory.GetFiles(m_backupDocumentsPath, searchPattern));
		}
		catch (Exception)
		{
		}
		return null;
	}

	public override List<FileInfo> GetLocalFileList(string searchPattern)
	{
		try
		{
			string[] files = Directory.GetFiles(m_localDocumentsPath, searchPattern);
			if (files != null)
			{
				return GetFileInfoList(files);
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public override void DeleteAllFiles()
	{
		try
		{
			DeleteFiles(m_localDocumentsPath);
			if (m_backupEnabled)
			{
				DeleteFiles(m_backupDocumentsPath);
			}
		}
		catch (Exception)
		{
		}
	}

	protected List<FileInfo> GetFileInfoList(string[] filePaths)
	{
		List<FileInfo> list = new List<FileInfo>(filePaths.Length);
		foreach (string path in filePaths)
		{
			FileInfo fileInfo = new FileInfo();
			fileInfo.path = path;
			fileInfo.date = File.GetLastWriteTimeUtc(path);
			list.Add(fileInfo);
		}
		return list;
	}

	protected void DeleteFiles(string path)
	{
		string[] files = Directory.GetFiles(path);
		string[] array = files;
		foreach (string path2 in array)
		{
			File.Delete(Path.Combine(path, path2));
		}
	}

	protected void RestoreBackupFiles()
	{
		if (!m_backupEnabled)
		{
			return;
		}
		try
		{
			string[] files = Directory.GetFiles(m_backupDocumentsPath);
			string[] array = files;
			foreach (string path in array)
			{
				string fileName = Path.GetFileName(path);
				string path2 = Path.Combine(m_localDocumentsPath, fileName);
				if (!File.Exists(path2))
				{
					byte[] bytes = File.ReadAllBytes(path);
					File.WriteAllBytes(path2, bytes);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	protected override void Awake()
	{
		base.Awake();
		LocalDocumentsPath = Application.persistentDataPath;
	}
}
