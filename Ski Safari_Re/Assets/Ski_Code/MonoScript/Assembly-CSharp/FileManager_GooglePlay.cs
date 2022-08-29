using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

public class FileManager_GooglePlay : FileManager_CloudBase
{
	private class MemoryFileInfo
	{
		public string name;

		public DateTime timestamp;

		public byte[] data;
	}

	private enum State
	{
		Idle = 0,
		Loading = 1
	}

	internal static class FindFilesPatternToRegex
	{
		private static Regex HasQuestionMarkRegEx = new Regex("\\?");

		private static Regex IlegalCharactersRegex = new Regex("[\\/:<>|\"]");

		private static Regex CatchExtentionRegex = new Regex("^\\s*.+\\.([^\\.]+)\\s*$");

		private static string NonDotCharacters = "[^.]*";

		public static Regex Convert(string pattern)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException();
			}
			pattern = pattern.Trim();
			if (pattern.Length == 0)
			{
				throw new ArgumentException("Pattern is empty.");
			}
			if (IlegalCharactersRegex.IsMatch(pattern))
			{
				throw new ArgumentException("Patterns contains ilegal characters.");
			}
			bool flag = CatchExtentionRegex.IsMatch(pattern);
			bool flag2 = false;
			if (HasQuestionMarkRegEx.IsMatch(pattern))
			{
				flag2 = true;
			}
			else if (flag)
			{
				flag2 = CatchExtentionRegex.Match(pattern).Groups[1].Length != 3;
			}
			string input = Regex.Escape(pattern);
			input = "^" + Regex.Replace(input, "\\\\\\*", ".*");
			input = Regex.Replace(input, "\\\\\\?", ".");
			if (!flag2 && flag)
			{
				input += NonDotCharacters;
			}
			input += "$";
			return new Regex(input, RegexOptions.IgnoreCase);
		}
	}

	private const string SnapshotName = "0";

	private Dictionary<string, MemoryFileInfo> m_files = new Dictionary<string, MemoryFileInfo>();

	private State m_state;

	private bool m_loadRequested;

	private bool m_saveRequested;

	private bool m_hasLoadedFromCloud;

	public override bool CloudEnabled
	{
		get
		{
			return true;
		}
	}

	public override bool HasPendingDownloads
	{
		get
		{
			return false;
		}
	}

	protected override bool WriteAllBytesToCloud(string fileName, byte[] bytes, ref DateTime cloudDate)
	{
		cloudDate = DateTime.UtcNow;
		MemoryFileInfo value;
		if (m_files.TryGetValue(fileName, out value))
		{
			value.timestamp = cloudDate;
			value.data = bytes.Clone() as byte[];
		}
		else
		{
			m_files[fileName] = new MemoryFileInfo
			{
				name = fileName,
				timestamp = cloudDate,
				data = (bytes.Clone() as byte[])
			};
		}
		m_saveRequested = true;
		return true;
	}

	protected override byte[] ReadAllBytesFromCloud(string fileName, out DateTime cloudDate)
	{
		MemoryFileInfo value;
		if (m_files.TryGetValue(fileName, out value))
		{
			cloudDate = value.timestamp;
			return value.data.Clone() as byte[];
		}
		cloudDate = DateTime.MinValue;
		return null;
	}

	public override List<FileInfo> GetFileList(string searchPattern)
	{
		Regex regex = FindFilesPatternToRegex.Convert(searchPattern);
		List<FileInfo> list = new List<FileInfo>();
		foreach (KeyValuePair<string, MemoryFileInfo> file in m_files)
		{
			if (regex.IsMatch(file.Key))
			{
				FileInfo fileInfo = new FileInfo();
				fileInfo.path = file.Key;
				fileInfo.date = file.Value.timestamp;
				list.Add(fileInfo);
			}
		}
		if (list.Count == 0)
		{
		}
		return list;
	}

	public override void DeleteAllFiles()
	{
		base.DeleteAllFiles();
		m_files.Clear();
		m_hasLoadedFromCloud = false;
	}

	public void DebugClearCloudCache()
	{
		m_files.Clear();
		m_hasLoadedFromCloud = true;
	}

	protected override bool IsCloudFileNewer(string fileName, DateTime fileTimestamp)
	{
		if (!m_hasLoadedFromCloud)
		{
			return true;
		}
		MemoryFileInfo value;
		if (!m_files.TryGetValue(fileName, out value))
		{
			return false;
		}
		return value.timestamp > fileTimestamp;
	}

	protected override void PurgeCloudFile(string fileName)
	{
		m_files.Remove(fileName);
	}

	private void DeserialiseFiles(byte[] data)
	{
		string @string = Encoding.ASCII.GetString(data);
		try
		{
			ArrayList arrayList = MiniJSON.jsonDecode(@string) as ArrayList;
			foreach (Hashtable item in arrayList)
			{
				string text = (string)item["name"];
				DateTime dateTime = DateTime.FromFileTimeUtc(long.Parse((string)item["timestamp"]));
				byte[] data2 = Convert.FromBase64String((string)item["data"]);
				MemoryFileInfo value;
				if (m_files.TryGetValue(text, out value))
				{
					if (dateTime > value.timestamp)
					{
						value.timestamp = dateTime;
						value.data = data2;
					}
				}
				else
				{
					MemoryFileInfo memoryFileInfo = new MemoryFileInfo();
					memoryFileInfo.name = text;
					memoryFileInfo.timestamp = dateTime;
					memoryFileInfo.data = data2;
					value = memoryFileInfo;
					m_files[text] = value;
				}
				if (FileManager.OnFileUpdated != null)
				{
					FileManager.OnFileUpdated(text, dateTime);
				}
			}
			DateTime utcNow = DateTime.UtcNow;
			foreach (MemoryFileInfo value2 in m_files.Values)
			{
				if (value2.timestamp == DateTime.MinValue)
				{
					value2.timestamp = utcNow;
					if (FileManager.OnFileUpdated != null)
					{
						FileManager.OnFileUpdated(value2.name, utcNow);
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private byte[] SerialiseFiles()
	{
		ArrayList arrayList = new ArrayList(m_files.Count);
		foreach (KeyValuePair<string, MemoryFileInfo> file in m_files)
		{
			Hashtable hashtable = new Hashtable(3);
			hashtable["name"] = file.Value.name;
			hashtable["timestamp"] = file.Value.timestamp.ToFileTimeUtc().ToString();
			hashtable["data"] = Convert.ToBase64String(file.Value.data);
			arrayList.Add(hashtable);
		}
		string s = MiniJSON.jsonEncode(arrayList);
		return Encoding.ASCII.GetBytes(s);
	}

	private void PopulateFilesFromLocalDocuments()
	{
		string[] files = Directory.GetFiles(LocalDocumentsPath, "*");
		if (files == null)
		{
			return;
		}
		string[] array = files;
		foreach (string path in array)
		{
			string fileName = Path.GetFileName(path);
			if (!m_files.ContainsKey(fileName))
			{
				try
				{
					MemoryFileInfo memoryFileInfo = new MemoryFileInfo();
					memoryFileInfo.name = fileName;
					memoryFileInfo.data = File.ReadAllBytes(path);
					memoryFileInfo.timestamp = DateTime.MinValue;
					m_files[fileName] = memoryFileInfo;
				}
				catch (Exception)
				{
				}
			}
		}
	}

	private void OnAuthenticationSucceeded(string playerId)
	{
		m_loadRequested = true;
		m_state = State.Idle;
	}

	

	private void OnLoadSnapshotFailed(string reason)
	{
		m_state = State.Idle;
		if (reason == "2002")
		{
			m_hasLoadedFromCloud = true;
		}
	}

	private void OnSaveSnapshotSucceeded()
	{
	}

	private void OnSaveSnapshotFailed(string reason)
	{
	}

	protected override void Awake()
	{
		base.Awake();
		
            Debug.Log(Application.identifier);

        LocalDocumentsPath = Application.persistentDataPath;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
            AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass(Application.identifier + ".PathUtils"))
			{
				string text2 = (LocalDocumentsPath = androidJavaClass2.CallStatic<string>("getDataPath", new object[2] { @static, "data" }));
				base.BackupDocumentsPath = Application.persistentDataPath;
				RestoreBackupFiles();
			}
		}
		PopulateFilesFromLocalDocuments();
	}

	private void Update()
	{
		
	}
}
