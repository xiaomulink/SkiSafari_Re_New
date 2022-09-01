using System;

public class MsgUseSkill : MsgBase
{
	public string id;

	public string charname;

	public int Skillnub;

	public MsgUseSkill()
	{
		this.protoName = "MsgUseSkill";
	}
}
