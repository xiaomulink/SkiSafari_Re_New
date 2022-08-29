using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FileManager : MonoBehaviour
{
	public class FileInfo
	{
		public string path;

		public DateTime date;
	}

	public static FileManager Instance;

	public static Action<string, DateTime> OnFileUpdated;

	public static Action<string> OnFilePendingDownload;

	public abstract string LocalDocumentsPath { get; set; }

	public abstract bool CloudEnabled { get; }

	public abstract bool HasPendingDownloads { get; }

	public abstract bool WriteAllBytes(string fileName, byte[] bytes, bool saveToCloud, ref DateTime cloudDate, out bool wasSavedToCloud);

	public abstract byte[] ReadAllBytes(string fileName, bool preferLocal, out DateTime cloudDate, out bool isCloudFile);

	public abstract List<FileInfo> GetFileList(string searchPattern);

	public abstract List<FileInfo> GetLocalFileList(string searchPattern);

	public abstract void DeleteAllFiles();

	protected virtual void Awake()
	{
		Instance = this;
	}
}
