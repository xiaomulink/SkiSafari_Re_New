using System;
using System.Text;
using System.Web.Script.Serialization;

public class MsgBase
{
	public string protoName = "null";

	private static JavaScriptSerializer Js = new JavaScriptSerializer();

	public static byte[] Encode(MsgBase msgBase)
	{
		string s = MsgBase.Js.Serialize(msgBase);
		return Encoding.UTF8.GetBytes(s);
	}

	public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        try
        {
            string @string = Encoding.UTF8.GetString(bytes, offset, count);
            try
            {
                return (MsgBase)MsgBase.Js.Deserialize(@string, Type.GetType(protoName));
            }
            catch
            {
                ReadLog._ReadLog("ErrorDecode:" + @string);
                return null;
            }
        }
        catch
        {
            return null;
        }
    }

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

	public static string DecodeName(byte[] bytes, int offset, out int count)
	{
		count = 0;
		bool flag = offset + 2 > bytes.Length;
		string result;
		if (flag)
		{
			ReadLog._ReadLog("没有大于2字节", null, true);
			result = "null";
		}
		else
		{
			short num = (short)((int)bytes[offset + 1] << 8 | (int)bytes[offset]);
			bool flag2 = offset + 2 + (int)num > bytes.Length;
			if (flag2)
			{
				ReadLog._ReadLog(Encoding.UTF8.GetString(bytes, 0, bytes.Length), null, true);
				ReadLog._ReadLog("长度不够", null, true);
				result = "Notenough";
			}
			else
			{
				count = (int)(2 + num);
				string @string = Encoding.UTF8.GetString(bytes, offset + 2, (int)num);
				result = @string;
			}
		}
		return result;
	}
    public static string PingDecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        bool flag = offset + 2 > bytes.Length;
        string result;
        if (flag)
        {
            ReadLog._ReadLog("没有大于2字节", null, true);
            result = "null";
        }
        else
        {
            short num = (short)((int)bytes[offset + 1] << 8 | (int)bytes[offset]);
            bool flag2 = offset + 2 + (int)num > bytes.Length;
            if (flag2)
            {
                ReadLog._ReadLog(Encoding.UTF8.GetString(bytes, 0, bytes.Length), null, true);
                ReadLog._ReadLog("ping长度不够", null, true);
                result = "Notenough";
            }
            else
            {
                count = (int)(2 + num);
                string @string = Encoding.UTF8.GetString(bytes, offset + 2, (int)num);
                result = @string;
            }
        }
        return result;
    }
}
