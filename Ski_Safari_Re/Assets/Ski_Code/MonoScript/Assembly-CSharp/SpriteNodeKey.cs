using System;
using UnityEngine;

[Serializable]
public class SpriteNodeKey
{
	public Vector3 pos;

	public Quaternion rot;

	public SpriteNodeKey()
	{
	}

	public SpriteNodeKey(Transform node)
	{
		pos = node.localPosition;
		rot = node.localRotation;
	}
}
