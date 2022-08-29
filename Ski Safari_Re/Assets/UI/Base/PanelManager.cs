using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200015C RID: 348
public static class PanelManager
{
    public static bool IsInit()
    {
        if (PanelManager.layers.Count == 2)
        {
            return true;
        } else
        {
            return false;
        }
    }
    
    //清除panel
    public static void PanelClear()
    {
        PanelManager.panels.Clear();
        PanelManager.Functionpanels.Clear();
        root = null;
        canvas = null;
    }

    //清除绑定的canvas
    public static void Clear()
    {
        root =null;
        canvas = null;
        try
        {
            PanelManager.layers.Clear();
            PanelManager.Functionpanels.Clear();
        }
        catch
        {
        }
    }


    // Token: 0x060006C8 RID: 1736 RVA: 0x0002E0E4 File Offset: 0x0002C2E4
    public static void Init()
	{
		root = GameObject.Find("UIRoot").transform;
		canvas = root.Find("Canvas");
		Transform value = canvas.Find("Panel");
		Transform value2 = canvas.Find("Tip");
		try
		{
			PanelManager.layers.Add(PanelManager.Layer.Panel, value);
			PanelManager.layers.Add(PanelManager.Layer.Tip, value2);
		}
		catch
		{

		}
	}
    public enum UIstyle
    {
        Default,
        Fadein,
    }
    public static float FadeDuration = 1f;
    public static float FadeTime;
    public static UIstyle stlyle;

    static float _timer = 1;
    static float _time = 0;

    public static void _uistyle(UIstyle _stlyle,GameObject UI)
    {
        switch (_stlyle)
        {
            case UIstyle.Default:
                break;
            case UIstyle.Fadein:
                //计时器
                {
                    /*
                    if (FadeTime <= 0)// reset
                    {
                        FadeTime = FadeDuration;
                    }
                    else
                    {
                        FadeTime -= Time.deltaTime;
                        //需要做的事

                    }
                    */
                    UnityEngine.Object.Instantiate(ResManager.LoadPrefab("UIAnim/Fadein"), UI.transform);
                }
                break;
        }
    }



    // Token: 0x060006C9 RID: 1737 RVA: 0x0002E16C File Offset: 0x0002C36C
    public static T OpenT<T>(UIstyle Stlyle=UIstyle.Default,params object[] para) where T : BasePanel
	{
       
		string key = typeof(T).ToString();
		if (PanelManager.panels.ContainsKey(key))
		{
			return null;
		}
		BasePanel basePanel = PanelManager.root.gameObject.AddComponent<T>();
		basePanel.OnInit();
		GameObject result = basePanel.Init();
        result.name = key;
		Transform parent = PanelManager.layers[basePanel.layer];
		basePanel.skin.transform.SetParent(parent, false);
		PanelManager.panels.Add(key, basePanel);
		basePanel.OnShow(para);
        _uistyle(Stlyle,result);
        return PanelManager.root.gameObject.GetComponent<T>();
	} 
    // Token: 0x060006C9 RID: 1737 RVA: 0x0002E16C File Offset: 0x0002C36C
    public static GameObject Open<T>(UIstyle Stlyle=UIstyle.Default,params object[] para) where T : BasePanel
	{
		string key = typeof(T).ToString();
		if (PanelManager.panels.ContainsKey(key) && key != "TipaffirmPanel")
		{
			return null;
		}
        if (key == "TipaffirmPanel")
        {
            Close("TipaffirmPanel");
        }
        BasePanel basePanel = PanelManager.root.gameObject.AddComponent<T>();
		basePanel.OnInit();
		GameObject result = basePanel.Init();
        result.name = key;
		Transform parent = PanelManager.layers[basePanel.layer];
		basePanel.skin.transform.SetParent(parent, false);
		PanelManager.panels.Add(key, basePanel);
		basePanel.OnShow(para);
        _uistyle(Stlyle,result);
        return result;
	}

    //以可随意关闭的方式打开penel
    public static GameObject FunctionOpen<T>(UIstyle Stlyle = UIstyle.Default, params object[] para) where T : BasePanel
    {

        string key = typeof(T).ToString();
        foreach (BasePanel ba in Functionpanels)
        {
            if (ba.name==key)
            {
                return null;
            }
        }
        BasePanel basePanel = PanelManager.root.gameObject.AddComponent<T>();
        basePanel.OnInit();
        GameObject result = basePanel.Init();
        basePanel.name = key;
        Transform parent = PanelManager.layers[basePanel.layer];
        basePanel.skin.transform.SetParent(parent, false);
        PanelManager.Functionpanels.Add(basePanel);
        basePanel.OnShow(para);
        _uistyle(Stlyle, result);
        return result;
    }

    public static T FindUISprict<T>()
    {
        return root.GetComponent<T>();
    }

    public static float clocetime = 0.1f;
    // Token: 0x060006CA RID: 1738 RVA: 0x0002E1F0 File Offset: 0x0002C3F0
    public static async void FunctionClose(int position=1)
    {
        if(clocetime!=0)
        {
            return;
        }
       // try
        {
            BasePanel basePanel = PanelManager.Functionpanels[Functionpanels.Count - position];
            Debug.LogError(basePanel.name);
            Type type = Type.GetType(basePanel.name);
            object obj = type.Assembly.CreateInstance(basePanel.name);
            type.GetMethod("OnClose").Invoke(obj, null);
            basePanel.OnClose();
            if (basePanel == null)
            {
                return;
            }
            await Task.Delay(50);


            UnityEngine.Object.Destroy(basePanel);
            UnityEngine.Object.Destroy(basePanel.skin);
            try
            {
                PanelManager.Functionpanels.Remove(Functionpanels[Functionpanels.Count - position]);
                PanelManager.Functionpanels[Functionpanels.Count - position].skin.SetActive(true);
            }
            catch { }
           

        }
        //catch
        {
      
        }
    }
    // Token: 0x060006CA RID: 1738 RVA: 0x0002E1F0 File Offset: 0x0002C3F0
    public static void FunctionClose(string name)
    {
        if (name == null)
        {
            return;
        }
        try
        {
            foreach (BasePanel basePanel in Functionpanels)
            {
                
                if (basePanel.name == name)
                {
                    basePanel.OnClose();
                    PanelManager.Functionpanels.Remove(basePanel);  
                    UnityEngine.Object.Destroy(basePanel.skin);
                    UnityEngine.Object.Destroy(basePanel);
                }
                Debug.LogError("Null");
            }
        }
        catch
        {
            if (name != "LevelPanel")
                Debug.LogError("ErrorPanel:" + name);
        }
    }
    // Token: 0x060006CA RID: 1738 RVA: 0x0002E1F0 File Offset: 0x0002C3F0
    public static void Close(string name)
	{
        if(name==null)
        {
            return;
        }
        try
        {
            BasePanel basePanel = PanelManager.panels[name];
            if (basePanel == null)
            {
                return;
            }
            basePanel.OnClose();
            PanelManager.panels.Remove(name);
            UnityEngine.Object.Destroy(basePanel.skin);
            UnityEngine.Object.Destroy(basePanel);
        }catch
        {
            if(name!= "LevelPanel")
                Debug.LogError("ErrorPanel:" + name);
        }
	}
    
    public static GameObject Find(string name)
    {
        try
        {
            BasePanel basePanel = PanelManager.panels[name];
            if (basePanel == null)
            {
                Debug.LogError("Null");

                return null;
            }
            Debug.LogError("return");

            return basePanel.skin;
        }catch(Exception ex)
        {
            Debug.LogError("_Null\n"+ex);

            return null;
        }
    } public static GameObject FunctionFind(string name)
    {
        try
        {
            foreach (BasePanel basepanel in Functionpanels)
            {
                
                if (basepanel.name == name)
                {
                    return basepanel.skin;
                }
                return null;

            }

            return null;

        }
        catch (Exception ex)
        {

            return null;
        }
    }

	// Token: 0x0400080B RID: 2059
	private static Dictionary<PanelManager.Layer, Transform> layers = new Dictionary<PanelManager.Layer, Transform>();

	// Token: 0x0400080C RID: 2060
	public static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
	public static List<BasePanel> Functionpanels = new List<BasePanel>();

	// Token: 0x0400080D RID: 2061
	public static Transform root;

	// Token: 0x0400080E RID: 2062
	public static Transform canvas;

	// Token: 0x0200015D RID: 349
	public enum Layer
	{
		// Token: 0x04000810 RID: 2064
		Panel,
		// Token: 0x04000811 RID: 2065
		Tip
	}
}
