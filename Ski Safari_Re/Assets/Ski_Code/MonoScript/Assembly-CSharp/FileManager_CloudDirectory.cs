using System;
using System.Collections.Generic;
using System.IO;

public abstract class FileManager_CloudDirectory : FileManager_CloudBase
{
	private string m_cloudDocumentsPath;

	protected string CloudDocumentsPath
	{
		get
		{
			return m_cloudDocumentsPath;
		}
		set
		{
			m_cloudDocumentsPath = value;
		}
	}

	public override List<FileInfo> GetFileList(string searchPattern)
	{
		//Discarded unreachable code: IL_0023
		if (CloudEnabled)
		{
			try
			{
				return GetFileInfoList(Directory.GetFiles(m_cloudDocumentsPath, searchPattern));
			}
			catch (Exception)
			{
			}
		}
		return base.GetFileList(searchPattern);
	}

	public override void DeleteAllFiles()
	{
		base.DeleteAllFiles();
		try
		{
			DeleteFiles(m_cloudDocumentsPath);
		}
		catch (Exception)
		{
		}
	}

	protected override bool WriteAllBytesToCloud(string fileName, byte[] bytes, ref DateTime cloudDate)
	{
		string path = Path.Combine(m_cloudDocumentsPath, fileName);
		File.WriteAllBytes(path, bytes);
		cloudDate = File.GetLastWriteTimeUtc(path);
		return true;
	}

	protected override byte[] ReadAllBytesFromCloud(string fileName, out DateTime cloudDate)
	{
		string path = Path.Combine(m_cloudDocumentsPath, fileName);
		if (File.Exists(path))
		{
			cloudDate = File.GetLastWriteTimeUtc(path);
			return File.ReadAllBytes(path);
		}
		cloudDate = DateTime.MinValue;
		return null;
	}

	protected override bool IsCloudFileNewer(string fileName, DateTime lastCloudDate)
	{
		string path = Path.Combine(m_cloudDocumentsPath, fileName);
		if (!File.Exists(path))
		{
			return false;
		}
		DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
		return lastWriteTimeUtc > lastCloudDate;
	}

	protected override void PurgeCloudFile(string fileName)
	{
		string path = Path.Combine(m_cloudDocumentsPath, fileName);
		File.Delete(path);
	}
}
