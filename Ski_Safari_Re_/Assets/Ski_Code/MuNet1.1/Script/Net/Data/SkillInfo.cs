using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


//==========================
// - ScriptName:      SkillInfo.cs         	
// - CreateTime:    2021/04/23 22:49:48	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================

[Serializable]
public class SkillInfo : MonoBehaviour 
{ 
    // Token: 0x060005ED RID: 1517 RVA: 0x0002A650 File Offset: 0x00028850
    public static async void UseSkill(SkillInfo skill)
    {

    }

    // Token: 0x04000753 RID: 1875
    public Sprite Skills_image;

    // Token: 0x04000754 RID: 1876
    public Sprite Skills_Hand;

    // Token: 0x04000756 RID: 1878
    public string Skills_name;

    // Token: 0x04000757 RID: 1879
    public string Skills_ins;

    // Token: 0x04000758 RID: 1880
    public float _Skills_time;

    // Token: 0x04000759 RID: 1881
    public float skillcd;

    // Token: 0x0400075A RID: 1882
    public CharDataInfo _skillData;
}
