using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      GUICustomHomeButton.cs         	
// - CreateTime:    #CreateTime#	
// - Homepage:         #AuthorHomepage#		
// - Email:         #AuthorEmail#		
// - Description:   
//==========================

public class GUICustomHomeButton : GUIButton
{
    public override void Click(Vector3 position)
    {
        base.Click(position);
        Deactivate();
        if (SkiGameManager.Instance.ShowCustom)
        {
            SkiGameManager.Instance.ShowCustom = false;
        }
        else
        {
            SkiGameManager.Instance.Restart(SkiGameManager.RestartMode.Title);
        }
    }
}
