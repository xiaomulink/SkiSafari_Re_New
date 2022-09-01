using System;

public class ByteArray
{
	private const int DEFAULT_SIZE = 1024;

	private int initSize;

	public byte[] bytes;

	public int readIdx;

	public int writeIdx;

	private int capacity;

	public int remain
	{
		get
		{
			return this.capacity - this.writeIdx;
		}
	}

	public int length
	{
		get
		{
			return this.writeIdx - this.readIdx;
		}
	}

	public ByteArray(int size = 1024)
	{
		this.bytes = new byte[size];
		this.capacity = size;
		this.initSize = size;
		this.readIdx = 0;
		this.writeIdx = 0;
	}

	public ByteArray(byte[] defaultBytes)
	{
		this.bytes = defaultBytes;
		this.capacity = defaultBytes.Length;
		this.initSize = defaultBytes.Length;
		this.readIdx = 0;
		this.writeIdx = defaultBytes.Length;
	}

	public void ReSize(int size)
	{
		if (size >= this.length && size >= this.initSize)
		{
			int i;
			for (i = 1; i < size; i *= 2)
			{
			}
			this.capacity = i;
			byte[] destinationArray = new byte[this.capacity];
			Array.Copy(this.bytes, this.readIdx, destinationArray, 0, this.writeIdx - this.readIdx);
			this.bytes = destinationArray;
			this.writeIdx = this.length;
			this.readIdx = 0;
		}
	}

	public int Write(byte[] bs, int offset, int count)
	{
		bool flag = this.remain < count;
		if (flag)
		{
			this.ReSize(this.length + count);
		}
		Array.Copy(bs, offset, this.bytes, this.writeIdx, count);
		this.writeIdx += count;
		return count;
	}

	public int Read(byte[] bs, int offset, int count)
	{
		count = Math.Min(count, this.length);
		Array.Copy(this.bytes, 0, bs, offset, count);
		this.readIdx += count;
		this.CheckAndMoveBytes();
		return count;
	}

	public void CheckAndMoveBytes()
	{
		bool flag = this.length < 8;
		if (flag)
		{
			this.MoveBytes();
		}
	}

	public void MoveBytes()
	{
		Array.Copy(this.bytes, this.readIdx, this.bytes, 0, this.length);
		this.writeIdx = this.length;
		this.readIdx = 0;
	}

	public short ReadInt16()
	{
		bool flag = this.length < 2;
		short result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			short num = BitConverter.ToInt16(this.bytes, this.readIdx);
			this.readIdx += 2;
			this.CheckAndMoveBytes();
			result = num;
		}
		return result;
	}

	public int ReadInt32()
	{
		bool flag = this.length < 4;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			int num = BitConverter.ToInt32(this.bytes, this.readIdx);
			this.readIdx += 4;
			this.CheckAndMoveBytes();
			result = num;
		}
		return result;
	}

	public override string ToString()
	{
		return BitConverter.ToString(this.bytes, this.readIdx, this.length);
	}

	public string Debug()
	{
		return string.Format("readIdx({0}) writeIdx({1}) bytes({2})", this.readIdx, this.writeIdx, BitConverter.ToString(this.bytes, 0, this.capacity));
	}
}
