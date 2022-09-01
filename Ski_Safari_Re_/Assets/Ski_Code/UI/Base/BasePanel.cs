using System;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class BasePanel : MonoBehaviour
{
	// Token: 0x0600068D RID: 1677 RVA: 0x0002D248 File Offset: 0x0002B448
	public GameObject Init()
	{
		GameObject original = ResManager.LoadPrefab(this.skinPath);
		this.skin = UnityEngine.Object.Instantiate<GameObject>(original);
		return this.skin;
	}
   
    // Token: 0x0600068E RID: 1678 RVA: 0x0002D274 File Offset: 0x0002B474
    public void Close()
	{
        PanelManager.Close(base.GetType().ToString());
	}
    public void FunctionClose()
    {
        PanelManager.FunctionClose();
        try
        {
            if(PanelManager.Functionpanels[PanelManager.Functionpanels.Count].GetComponent<CanvasGroup>())
            PanelManager.Functionpanels[PanelManager.Functionpanels.Count].GetComponent<CanvasGroup>().alpha = 1;
        }
        catch
        {

        }
    }
    //淡出
    public void fadeout()
    {
        
    }
    //隐藏
    public void hide()
    {
        skin.SetActive(false);
        // skin.GetComponent<CanvasGroup>().alpha = 0;
    }
    //显示
    public void show()
    {
        skin.SetActive(true);
        //skin.GetComponent<CanvasGroup>().alpha = 1;
    }
    // Token: 0x0600068F RID: 1679 RVA: 0x0002D288 File Offset: 0x0002B488
    public virtual void OnInit()
	{
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0002D28C File Offset: 0x0002B48C
	public virtual void OnShow(params object[] para)
	{
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0002D290 File Offset: 0x0002B490
	public virtual void OnClose()
	{
	}

	// Token: 0x040007E3 RID: 2019
	public string skinPath;

	// Token: 0x040007E4 RID: 2020
	public GameObject skin;

	// Token: 0x040007E5 RID: 2021
	public PanelManager.Layer layer;

    new public string name;
}
