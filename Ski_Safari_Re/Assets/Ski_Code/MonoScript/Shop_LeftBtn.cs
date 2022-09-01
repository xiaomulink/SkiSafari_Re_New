using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      Shop_LeftBtn.cs         	
// - CreateTime:    #CreateTime#	
// - Homepage:         #AuthorHomepage#		
// - Email:         #AuthorEmail#		
// - Description:   
//==========================

public class Shop_LeftBtn : GUIButton
{
    public override void Click(Vector3 position)
    {
        transform.parent.GetComponent<GUIShopTab>().SetTargetItemIndex(transform.parent.GetComponent<GUIShopTab>().m_selectedItemIndex - 1);
    }
}
