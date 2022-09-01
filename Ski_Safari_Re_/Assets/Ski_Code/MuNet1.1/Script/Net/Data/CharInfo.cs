using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class CharInfo : MonoBehaviour
{
	// Token: 0x060004AE RID: 1198 RVA: 0x00023F00 File Offset: 0x00022100
	public CharInfo ch()
	{
		return this;
	}

	// Token: 0x0400063F RID: 1599
	public RuntimeAnimatorController Char_A;
    //主武器
	public RuntimeAnimatorController main;
    //近战武器
    public RuntimeAnimatorController melee;

	// Token: 0x04000640 RID: 1600
	public RuntimeAnimatorController CharFree_A;

	// Token: 0x04000641 RID: 1601
	public string Name;

	// Token: 0x04000642 RID: 1602
	public string Ins;

	// Token: 0x04000643 RID: 1603
	public Sprite Hand;

	// Token: 0x04000644 RID: 1604
	public GameObject Prefab3D;

	// Token: 0x04000645 RID: 1605
	public CharDataInfo charData;

	// Token: 0x04000646 RID: 1606
	public SkillInfo[] skillInfos;

	// Token: 0x04000647 RID: 1607
	public bool isOnThreeD;

	// Token: 0x04000648 RID: 1608
	public bool isOnTwoD = true;

	// Token: 0x04000649 RID: 1609
	public bool isComplete = true;

  /* public static CharDataInfo LoadEnemyData(string path)
    {
       List<string> ls = new List<string>();
        ls = ExcelLoad.LoadExcelRow("EnemyData/" + path, 2, 1);
        //Debug.Log(ls.Count);
        CharDataInfo cd = new CharDataInfo();
        cd.Name = ls[0];
        cd.Race = ls[1];
        cd.Level = int.Parse(ls[2]);
        cd.Hp = int.Parse(ls[3]);
        cd.PsychicPower = int.Parse(ls[4]);
        cd.Power = float.Parse(ls[5]);
        cd.AttackDamage = int.Parse(ls[6]);
        cd.S_Defense = float.Parse(ls[7]);
        cd.Defense = int.Parse(ls[8]);
        cd.Strength = int.Parse(ls[9]);
        cd.attacktime = int.Parse(ls[10]);
        cd.PT_attacktime = int.Parse(ls[11]);
        cd.PTH_attacktime = int.Parse(ls[12]);
        cd.Wait_AT = int.Parse(ls[13]);
        cd.Wait_PT_AT = int.Parse(ls[14]);
        cd.Wait_PTH_AT = int.Parse(ls[15]);
        return cd;
    }*/

    /*public static CharDataInfo LoadData(string path)
    {
        List<string> ls =new List<string>();
        ls = ExcelLoad.LoadExcelRow("CharData/"+path,2,1);
        //Debug.Log(ls.Count);
        CharDataInfo cd = new CharDataInfo();
        cd.Name = ls[0];
        cd.Race = ls[1];
        cd.Level = int.Parse(ls[2]);
        cd.Hp = int.Parse(ls[3]);
        cd.PsychicPower = int.Parse(ls[4]);
        cd.Power = float.Parse(ls[5]);
        cd.AttackDamage = int.Parse(ls[6]);
        cd.S_Defense = float.Parse(ls[7]);
        cd.Defense = int.Parse(ls[8]);
        cd.Strength = int.Parse(ls[9]);
        cd.attacktime = int.Parse(ls[10]);
        cd.PT_attacktime = int.Parse(ls[11]);
        cd.PTH_attacktime = int.Parse(ls[12]);
        cd.Wait_AT = int.Parse(ls[13]);
        cd.Wait_PT_AT = int.Parse(ls[14]);
        cd.Wait_PTH_AT = int.Parse(ls[15]);
        return cd;
    }*/
}
