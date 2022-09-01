using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      Item_Qiuqiubi.cs         	
// - CreateTime:    #CreateTime#	
// - Homepage:         #AuthorHomepage#		
// - Email:         #AuthorEmail#		
// - Description:   
//==========================

public class Item_QiuqiubiPack : Item
{
    public int count = 100;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if ((bool)CoinMagnet.Instance)
        {
            Object.Destroy(CoinMagnet.Instance.gameObject);
        }
    }

    public override bool Unlocked
    {
        get
        {
            return false;
        }
        set
        {
        }
    }
}
