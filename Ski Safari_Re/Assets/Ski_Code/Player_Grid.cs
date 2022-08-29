using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Grid : MonoBehaviour {
    public AudioSource Source;

    public AudioClip[] Clips;

	// Use this for initialization
	void Start () {
        int i = Random.Range(0, Clips.Length);
        Source.clip = Clips[i];
        Source.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
