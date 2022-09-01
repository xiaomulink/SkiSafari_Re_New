using System;

public abstract class FileManager_CloudBase : FileManager_Local
{
	public override bool WriteAllBytes(string fileName, byte[] bytes, bool saveToCloud, ref DateTime cloudDate, out bool wasSavedToCloud)
	{
		wasSavedToCloud = false;
		bool flag = false;
		string value = null;
		if (CloudEnabled && saveToCloud)
		{
			try
			{
				if (!IsCloudFileNewer(fileName, cloudDate))
				{
					wasSavedToCloud = WriteAllBytesToCloud(fileName, bytes, ref cloudDate);
				}
			}
			catch (Exception ex)
			{
				flag = true;
				value = ex.Message;
			}
		}
		bool wasSavedToCloud2 = false;
		bool flag2 = base.WriteAllBytes(fileName, bytes, saveToCloud, ref cloudDate, out wasSavedToCloud2);
		if (flag2 && flag)
		{
			try
			{
				AnalyticsManager.Instance.SendEvent("delete_cloud_file", "exception", value);
				PurgeCloudFile(fileName);
				return flag2;
			}
			catch (Exception)
			{
				return flag2;
			}
		}
		return flag2;
	}

	public override byte[] ReadAllBytes(string fileName, bool preferLocal, out DateTime cloudDate, out bool isCloudFile)
	{
		if (preferLocal)
		{
			byte[] array = base.ReadAllBytes(fileName, preferLocal, out cloudDate, out isCloudFile);
			if (array != null)
			{
				return array;
			}
		}
		else
		{
			cloudDate = DateTime.MinValue;
			isCloudFile = false;
		}
		if (CloudEnabled)
		{
			try
			{
				byte[] array2 = ReadAllBytesFromCloud(fileName, out cloudDate);
				if (array2 != null)
				{
					return array2;
				}
			}
			catch (Exception)
			{
			}
		}
		if (!preferLocal)
		{
			byte[] array3 = base.ReadAllBytes(fileName, preferLocal, out cloudDate, out isCloudFile);
			if (array3 != null)
			{
				return array3;
			}
		}
		return null;
	}

	protected abstract bool WriteAllBytesToCloud(string fileName, byte[] bytes, ref DateTime cloudDate);

	protected abstract byte[] ReadAllBytesFromCloud(string fileName, out DateTime cloudDate);

	protected abstract bool IsCloudFileNewer(string fileName, DateTime lastCloudDate);

	protected abstract void PurgeCloudFile(string fileName);
}
