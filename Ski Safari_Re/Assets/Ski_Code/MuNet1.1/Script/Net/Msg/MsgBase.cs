using System;
using System.Text;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class MsgBase
{
	// Token: 0x060006AA RID: 1706 RVA: 0x0002D6C8 File Offset: 0x0002B8C8
	public static byte[] Encode(MsgBase msgBase)
	{
		string s = JsonUtility.ToJson(msgBase);
		return Encoding.UTF8.GetBytes(s);
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x0002D6E8 File Offset: 0x0002B8E8
	public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
	{
		string @string = Encoding.UTF8.GetString(bytes, offset, count);
		if (protoName != "MsgSyncPlayer")
		{
			Debug.Log(@string);
		}
		return (MsgBase)JsonUtility.FromJson(@string, Type.GetType(protoName));
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x0002D728 File Offset: 0x0002B928
	public static byte[] EncodeName(MsgBase msgBase)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(msgBase.protoName);
		short num = (short)bytes.Length;
		byte[] array = new byte[(int)(2 + num)];
		array[0] = (byte)(num % 256);
		array[1] = (byte)(num / 256);
		Array.Copy(bytes, 0, array, 2, (int)num);
		return array;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0002D774 File Offset: 0x0002B974
	public static string DecodeName(byte[] bytes, int offset, out int count)
	{
		count = 0;
		if (offset + 2 > bytes.Length)
		{
			return "";
		}
		short num = (short)((int)bytes[offset + 1] << 8 | (int)bytes[offset]);
		if (offset + 2 + (int)num > bytes.Length)
		{
			return "";
		}
		count = (int)(2 + num);
		return Encoding.UTF8.GetString(bytes, offset + 2, (int)num);
	}

	// Token: 0x040007F6 RID: 2038
	public string protoName = "null";
}
