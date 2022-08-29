using System;
using UnityEngine;

[Serializable]
public class SpriteAnimation
{
	public string name;

	public Rect rect = new Rect(0f, 0f, 32f, 32f);

	public int frameCount = 4;

	public int initialFrame;

	public float fps = 4f;

	public bool looping = true;

	public bool hold = true;

	public bool synchronise;

	public Sound sound;

	public SpriteNodeTrack[] nodeTracks = new SpriteNodeTrack[0];

	public Mesh[] bakedMeshes = new Mesh[0];
}
