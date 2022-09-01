using System;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class ResManager : MonoBehaviour
{
    // Token: 0x060006E7 RID: 1767 RVA: 0x0002EC2C File Offset: 0x0002CE2C
    public static GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }

    public static T LoadRes<T>(string path) where T: UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }
    public static T[] LoadResAll<T>(string path) where T: UnityEngine.Object
    {
        return Resources.LoadAll<T>(path);
    }
   
   
    // Token: 0x060006E8 RID: 1768 RVA: 0xA0002EC34 File Offset: 0x0002CE34
    public static GameObject[] LoadPrefabAll(string path)
    {
        return Resources.LoadAll<GameObject>(path);
    }
    public static object LoadAssetBundle(string path, string name)
    { 
        var bundle = AssetBundle.LoadFromFile("Assets/Resources/" + path+".jpg");    
        return (object)(bundle.LoadAsset<UnityEngine.Object> (name));
    }
 
}
