using System;

// Token: 0x02000153 RID: 339
public class ByteArray
{
	// Token: 0x17000078 RID: 120
	// (get) Token: 0x06000693 RID: 1683 RVA: 0x0002D29C File Offset: 0x0002B49C
	public int remain
	{
		get
		{
			return this.capacity - this.writeIdx;
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000694 RID: 1684 RVA: 0x0002D2AC File Offset: 0x0002B4AC
	public int length
	{
		get
		{
			return this.writeIdx - this.readIdx;
		}
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0002D2BC File Offset: 0x0002B4BC
	public ByteArray(int size = 1024)
	{
		this.bytes = new byte[size];
		this.capacity = size;
		this.initSize = size;
		this.readIdx = 0;
		this.writeIdx = 0;
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x0002D2EC File Offset: 0x0002B4EC
	public ByteArray(byte[] defaultBytes)
	{
		this.bytes = defaultBytes;
		this.capacity = defaultBytes.Length;
		this.initSize = defaultBytes.Length;
		this.readIdx = 0;
		this.writeIdx = defaultBytes.Length;
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0002D320 File Offset: 0x0002B520
	public void ReSize(int size)
	{
		if (size < this.length)
		{
			return;
		}
		if (size < this.initSize)
		{
			return;
		}
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

	// Token: 0x06000698 RID: 1688 RVA: 0x0002D39C File Offset: 0x0002B59C
	public int Write(byte[] bs, int offset, int count)
	{
		if (this.remain < count)
		{
			this.ReSize(this.length + count);
		}
		Array.Copy(bs, offset, this.bytes, this.writeIdx, count);
		this.writeIdx += count;
		return count;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0002D3D8 File Offset: 0x0002B5D8
	public int Read(byte[] bs, int offset, int count)
	{
		count = Math.Min(count, this.length);
		Array.Copy(this.bytes, 0, bs, offset, count);
		this.readIdx += count;
		this.CheckAndMoveBytes();
		return count;
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0002D40C File Offset: 0x0002B60C
	public void CheckAndMoveBytes()
	{
		if (this.length < 8)
		{
			this.MoveBytes();
		}
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x0002D420 File Offset: 0x0002B620
	public void MoveBytes()
	{
		Array.Copy(this.bytes, this.readIdx, this.bytes, 0, this.length);
		this.writeIdx = this.length;
		this.readIdx = 0;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x0002D454 File Offset: 0x0002B654
	public short ReadInt16()
	{
		if (this.length < 2)
		{
			return 0;
		}
		short result = BitConverter.ToInt16(this.bytes, this.readIdx);
		this.readIdx += 2;
		this.CheckAndMoveBytes();
		return result;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x0002D488 File Offset: 0x0002B688
	public int ReadInt32()
	{
		if (this.length < 4)
		{
			return 0;
		}
		int result = BitConverter.ToInt32(this.bytes, this.readIdx);
		this.readIdx += 4;
		this.CheckAndMoveBytes();
		return result;
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0002D4BC File Offset: 0x0002B6BC
	public override string ToString()
	{
		return BitConverter.ToString(this.bytes, this.readIdx, this.length);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x0002D4D8 File Offset: 0x0002B6D8
	public string Debug()
	{
		return string.Format("readIdx({0}) writeIdx({1}) bytes({2})", this.readIdx, this.writeIdx, BitConverter.ToString(this.bytes, 0, this.capacity));
	}

	// Token: 0x040007E6 RID: 2022
	private const int DEFAULT_SIZE = 1024;

	// Token: 0x040007E7 RID: 2023
	private int initSize;

	// Token: 0x040007E8 RID: 2024
	public byte[] bytes;

	// Token: 0x040007E9 RID: 2025
	public int readIdx;

	// Token: 0x040007EA RID: 2026
	public int writeIdx;

	// Token: 0x040007EB RID: 2027
	private int capacity;
}
