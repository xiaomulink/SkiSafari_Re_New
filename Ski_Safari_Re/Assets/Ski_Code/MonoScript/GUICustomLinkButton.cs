using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      GUICustomLinkButton.cs         	
// - CreateTime:    #CreateTime#	
// - Homepage:         #AuthorHomepage#		
// - Email:         #AuthorEmail#		
// - Description:   
//==========================

public class GUICustomLinkButton : GUIButton
{
    public string url = "https://space.bilibili.com/297969683";

    public override void Click(Vector3 position)
    {
        base.Click(position);
        Application.OpenURL(url);
    }
}
