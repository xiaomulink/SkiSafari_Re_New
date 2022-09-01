using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Relink : BasePanel
{
	//UI

	//初始化UI
	public override void OnInit()
	{
		this.skinPath = "Relink";
		this.layer = PanelManager.Layer.Panel;
	}
	
	//UI绑定
	public override void OnShow(params object[] args)
	{
	
	}
	

}
