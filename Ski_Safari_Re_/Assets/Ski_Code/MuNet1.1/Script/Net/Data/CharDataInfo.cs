using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Token: 0x020000E7 RID: 231
[Serializable]
public class CharDataInfo
{
    //名字
    public string Name;
    //种族
    public string Race;
    //等级
    public int Level;
    //血量
	public int Hp;

	//灵力值
	public  int PsychicPower;

	//力量
	public float Power;

    //攻击初始值
    public int AttackDamage;

    //防御值(叠加)
    public float S_Defense;
    //防御值
    public int Defense;

	//力气
	public int Strength;

    //攻击等待时间
    public int Wait_AT = 100;//一段
    public int Wait_PT_AT = 0;//二段
    public int Wait_PTH_AT = 0;//三段
    //攻击时间
    public int attacktime = 0;//一段
    public int PT_attacktime = 0;//二段
    public int PTH_attacktime = 0;//三段

}

