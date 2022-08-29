using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      AssetsBandleResManager.cs         	
// - CreateTime:    2022/04/16 12:46:33	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================

public class AssetsBundleResManager : Singleton<AssetsBundleResManager>
{
    public float score;
    void Awake()
    {
        
    }
#if UNITY_EDITOR
    public T GetAssetCache<T>(string name) where T: UnityEngine.Object
    {
        {
            string path ="Assets/AssetsPackage/"+name;
        UnityEngine.Object target =UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        return target as T;
         
        }
    }
#endif

}
