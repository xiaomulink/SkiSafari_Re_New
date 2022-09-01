using System;
using System.IO;

public class CSVReader : IDisposable
{
	private static char[] separators = new char[1] { ',' };

	private static char[] subSeparators = new char[1] { '+' };

	private static char[] trimCharacters = new char[2] { ' ', '"' };

	private StreamReader m_file;

	private string[] m_tokens;

	private int m_lineIndex = -1;

	private int m_tokenIndex;

	private bool m_silent;

	private bool m_disposed;

	public CSVReader(string fileName, bool silent = false)
	{
		m_silent = silent;
		try
		{
			m_file = new StreamReader(fileName);
			m_file.ReadLine();
		}
		catch (Exception)
		{
			if (m_silent)
			{
			}
		}
	}

	~CSVReader()
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

	public bool NextRow()
	{
		if (m_file == null)
		{
			return false;
		}
		string text = m_file.ReadLine();
		if (text == null)
		{
			return false;
		}
		m_tokens = text.Split(separators);
		m_tokenIndex = 0;
		m_lineIndex++;
		return true;
	}

	private string NextToken()
	{
		return m_tokens[m_tokenIndex++].Trim(trimCharacters);
	}

	public int ReadInt()
	{
		if (m_tokenIndex < m_tokens.Length)
		{
			string s = NextToken();
			int result = 0;
			if (int.TryParse(s, out result) || !m_silent)
			{
			}
			return result;
		}
		if (!m_silent)
		{
		}
		return 0;
	}

	public float ReadFloat()
	{
		if (m_tokenIndex < m_tokens.Length)
		{
			string s = NextToken();
			float result = 0f;
			if (float.TryParse(s, out result) || !m_silent)
			{
			}
			return result;
		}
		if (!m_silent)
		{
		}
		return 0f;
	}

	public string ReadString()
	{
		if (m_tokenIndex < m_tokens.Length)
		{
			return NextToken();
		}
		if (!m_silent)
		{
		}
		return string.Empty;
	}

	public string[] ReadStringArray()
	{
		if (m_tokenIndex < m_tokens.Length)
		{
			string[] array = m_tokens[m_tokenIndex++].Trim(trimCharacters).Split(subSeparators);
			if (array.Length == 1 && string.IsNullOrEmpty(array[0]))
			{
				return new string[0];
			}
			return array;
		}
		if (!m_silent)
		{
		}
		return new string[0];
	}

	public T ReadEnum<T>(T defaultValue)
	{
		//Discarded unreachable code: IL_002e
		string value = ReadString();
		if (!string.IsNullOrEmpty(value))
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value, true);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}
		return defaultValue;
	}
}
