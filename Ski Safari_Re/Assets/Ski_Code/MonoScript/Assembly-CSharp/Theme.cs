using System;
using System.Collections.Generic;
using UnityEngine;

public class Theme : ScriptableObject
{
	[Serializable]
	public class TextureConfig : IComparable
	{
		public string textureName;

		public Material[] materials;

		public int CompareTo(object obj)
		{
			TextureConfig textureConfig = obj as TextureConfig;
			return textureName.CompareTo(textureConfig.textureName);
		}
	}

	[Serializable]
	public class ColourConfig : IComparable
	{
		public Material material;

		public Color colour;

		public int CompareTo(object obj)
		{
			ColourConfig colourConfig = obj as ColourConfig;
			if (colourConfig == null || !colourConfig.material)
			{
				return 1;
			}
			if (!material)
			{
				return -1;
			}
			return material.name.CompareTo(colourConfig.material.name);
		}
	}

	[Serializable]
	public class SoundConfig : IComparable
	{
		public string audioClipName;

		public Sound[] sounds;

		public int CompareTo(object obj)
		{
			SoundConfig soundConfig = obj as SoundConfig;
			return audioClipName.CompareTo(soundConfig.audioClipName);
		}
	}

	public List<Bundle> bundles;

	public List<TextureConfig> textures;

	public List<ColourConfig> colours;

	public List<SoundConfig> sounds;
}
