using System;
using System.Collections.Generic;
using UnityEngine;

public class Bundle : ScriptableObject, IComparable
{
	public enum AssetType
	{
		Texture = 0,
		AudioClip = 1
	}

	public enum Mode
	{
		Resource = 0,
		AssetBundle = 1,
		Excluded = 2
	}

	[Serializable]
	public class BaseAsset : IComparable
	{
		public string name;

		public int CompareTo(object obj)
		{
			BaseAsset baseAsset = obj as BaseAsset;
			return name.CompareTo(baseAsset.name);
		}
	}

	[Serializable]
	public class TextureAsset : BaseAsset
	{
		public Material[] materials;
	}

	[Serializable]
	public class SoundAsset : BaseAsset
	{
		public Sound[] sounds;
	}

	[Serializable]
	public class LibraryDescriptor
	{
		public List<TextureAsset> textures;

		public List<SoundAsset> sounds;

		private LibraryDescriptor()
		{
		}

		public static LibraryDescriptor Create()
		{
			LibraryDescriptor libraryDescriptor = new LibraryDescriptor();
			libraryDescriptor.textures = new List<TextureAsset>();
			libraryDescriptor.sounds = new List<SoundAsset>();
			return libraryDescriptor;
		}
	}

	[Serializable]
	public class DefinitionDescriptor
	{
		public Mode mode;

		public int version;

		public string assetBundleName;

		public string remoteURL;

		public int size;
	}

	[Serializable]
	public class PlatformDescriptor : IComparable
	{
		public Platform platform;

		public DefinitionDescriptor sd;

		public DefinitionDescriptor hd;

		public int CompareTo(object obj)
		{
			PlatformDescriptor platformDescriptor = obj as PlatformDescriptor;
			return platform.CompareTo(platformDescriptor.platform);
		}
	}

	public bool resident;

	public int version;

	public LibraryDescriptor library;

	public List<PlatformDescriptor> platforms;

	public int CompareTo(object obj)
	{
		Bundle bundle = obj as Bundle;
		return base.name.CompareTo(bundle.name);
	}
}
