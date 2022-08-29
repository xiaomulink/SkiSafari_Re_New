using System;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
	[Serializable]
	public class PlatformDescriptor
	{
		public GameObject[] managers;
	}

	public GameObject[] commonManagers;

	public PlatformDescriptor editor;

	public PlatformDescriptor iOS;

	public PlatformDescriptor googlePlay;

	public PlatformDescriptor amazon;

	public PlatformDescriptor nook;

	public PlatformDescriptor unknown;

	public static PlatformManager Instance { get; private set; }

	private void InitialiseManagers(GameObject[] managers)
	{
		foreach (GameObject gameObject in managers)
		{
			if ((bool)gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.name = gameObject.name;
				gameObject2.transform.parent = base.transform;
			}
		}
	}

	private void SetupPlatform(PlatformDescriptor platform, string platformName)
	{
		InitialiseManagers(platform.managers);
	}

	private void Awake()
	{
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		InitialiseManagers(commonManagers);
		SetupPlatform(googlePlay, "Google Play");
	}
}
