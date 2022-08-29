using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      AudioManager.cs         	
// - CreateTime:    2022/05/05 20:53:22	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================

public class AudioManager : MonoBehaviour
{
    
    public static GameObject OpenAudio(string name)
    {
        GameObject go = ResManager.LoadPrefab("Audio_");
        AudioClip audio = ResManager.LoadRes<AudioClip>("Audio/" + name);
        GameObject go_ = Instantiate(go);
        go_.GetComponent<AudioSource>().clip = audio;
        go_.GetComponent<AudioSource>().enabled = false;
        go_.GetComponent<AudioSource>().enabled = true;

        return go_;
    }
}
