using System;
using System.IO;

public class CSVWriter : IDisposable
{
	private static char separator = ',';

	private static char subSeparator = '+';

	private StreamWriter m_file;

	private bool m_startOfLine = true;

	private bool m_disposed;

	public CSVWriter(string fileName, params string[] columns)
	{
		try
		{
			m_file = new StreamWriter(fileName);
			foreach (string val in columns)
			{
				WriteString(val);
			}
			EndRow();
		}
		catch (Exception)
		{
		}
	}

	~CSVWriter()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!m_disposed)
		{
			if (disposing && m_file != null)
			{
				m_file.Dispose();
			}
			m_disposed = true;
		}
	}

	public void EndRow()
	{
		if (m_file != null)
		{
			m_file.Write('\n');
			m_startOfLine = true;
		}
	}

	public void WriteInt(int val)
	{
		if (!m_startOfLine)
		{
			m_file.Write(separator);
		}
		m_startOfLine = false;
		m_file.Write(val);
	}

	public void WriteFloat(float val)
	{
		if (!m_startOfLine)
		{
			m_file.Write(separator);
		}
		m_startOfLine = false;
		m_file.Write(val);
	}

	public void WriteString(string val)
	{
		if (!m_startOfLine)
		{
			m_file.Write(separator);
		}
		m_startOfLine = false;
		m_file.Write(val);
	}

	public void WriteStringArray(string[] vals)
	{
		if (!m_startOfLine)
		{
			m_file.Write(separator);
		}
		m_startOfLine = false;
		bool flag = true;
		foreach (string value in vals)
		{
			if (!flag)
			{
				m_file.Write(subSeparator);
			}
			flag = false;
			m_file.Write(value);
		}
	}
}
