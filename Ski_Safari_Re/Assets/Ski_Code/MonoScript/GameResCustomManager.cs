using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResCustomManager : MonoBehaviour {
    public string Player_Default_Skin = "player_skier_default";

    public string Player_GetupAudio_Skin = "Down";
    public string Player_BacksomersaultAudio_Skin = "hulue";

    public AudioClip FullAudio;
    public AudioClip GetupAudio;
    public AudioClip BacksomersaultAudio;
    public Texture2D _PlayerTexture;

    public Item_Skin Player_Default_Texture;

    public MeshRenderer[] mesh;
    public AudioSource[] source;
    public UnityEngine.Sprite m_PlayerSprie;

    public GameObject[] Players;
    public static GameResCustomManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
         Players = GameObject.FindGameObjectsWithTag("Player");
        if(Players.Length>2)
        {
            Destroy(Players[1]);
        }
        if (SkiGameManager.Instance.custom.Texture_1)
        {
            mesh = GameObject.FindObjectsOfType<MeshRenderer>();
            if (_PlayerTexture != null)
            {
                foreach (MeshRenderer renderer in mesh)
                {
                    if (renderer.material.mainTexture != null)
                        if (renderer.material.mainTexture.name == Player_Default_Skin)
                            renderer.material.mainTexture = _PlayerTexture;
                }
            }
        }
        if (SkiGameManager.Instance.custom.Audio_1)
        {
            source = GameObject.FindObjectsOfType<AudioSource>();
            if (FullAudio != null)
            {
                SkiGameManager.Instance.PlayerHit1.clip = FullAudio;
                SkiGameManager.Instance.PlayerHit2.clip = FullAudio;
            }
        }
        if (SkiGameManager.Instance.custom.Audio_2)
        {
            source = GameObject.FindObjectsOfType<AudioSource>();
            if (GetupAudio != null)
            {
                foreach (AudioSource source in source)
                {
                    if (source.clip != null)
                        if (source.clip.name == Player_GetupAudio_Skin)
                            source.clip = GetupAudio;
                }
            }
        }
        if (SkiGameManager.Instance.custom.Audio_3)
        {
            source = GameObject.FindObjectsOfType<AudioSource>();
            if (BacksomersaultAudio != null)
            {
                foreach (AudioSource source in source)
                {
                    if (source.clip != null)
                        if (source.clip.name == Player_BacksomersaultAudio_Skin)
                            source.clip = BacksomersaultAudio;
                }
            }
        }
       
    }
}
