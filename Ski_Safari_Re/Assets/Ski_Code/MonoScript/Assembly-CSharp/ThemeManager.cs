using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
	[Serializable]
	public class SkinMaterial
	{
		public string name;

		public Material material;

		public string defaultTextureName;
	}

	[CompilerGenerated]
	private sealed class _003CLoadTheme_003Ec__AnonStorey1A
	{
		internal Theme.TextureConfig textureConfig;

		internal bool _003C_003Em__E(Theme.TextureConfig t)
		{
			return t.textureName == textureConfig.textureName;
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadTheme_003Ec__AnonStorey1B
	{
		internal Theme.SoundConfig soundConfig;

		internal bool _003C_003Em__F(Theme.SoundConfig s)
		{
			return s.audioClipName == soundConfig.audioClipName;
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadTheme_003Ec__AnonStorey1C
	{
		internal Theme.TextureConfig textureConfig;

		internal bool _003C_003Em__10(Theme.TextureConfig t)
		{
			return t.textureName == textureConfig.textureName;
		}
	}

	[CompilerGenerated]
	private sealed class _003CLoadTheme_003Ec__AnonStorey1D
	{
		internal Theme.SoundConfig soundConfig;

		internal bool _003C_003Em__11(Theme.SoundConfig s)
		{
			return s.audioClipName == soundConfig.audioClipName;
		}
	}

	public static ThemeManager Instance;

	public SkinMaterial[] skinMaterials;

	private Theme m_currentTheme;

	private SkinMaterial FindSkinMaterial(string materialName)
	{
		SkinMaterial[] array = skinMaterials;
		foreach (SkinMaterial skinMaterial in array)
		{
			if (skinMaterial.name == materialName)
			{
				return skinMaterial;
			}
		}
		return null;
	}

	public void SetSkinTexture(string materialName, string textureName)
	{
		SkinMaterial skinMaterial = FindSkinMaterial(materialName);
		AssetManager.UpdateTexture(skinMaterial.material, textureName);
	}

	public void LoadTheme(Theme theme)
	{
		if ((bool)m_currentTheme)
		{
			_003CLoadTheme_003Ec__AnonStorey1A _003CLoadTheme_003Ec__AnonStorey1A = new _003CLoadTheme_003Ec__AnonStorey1A();
			foreach (Theme.TextureConfig texture in m_currentTheme.textures)
			{
				_003CLoadTheme_003Ec__AnonStorey1A.textureConfig = texture;
				Material[] materials = _003CLoadTheme_003Ec__AnonStorey1A.textureConfig.materials;
				foreach (Material material in materials)
				{
					material.mainTexture = null;
				}
				if (theme.textures.Find(_003CLoadTheme_003Ec__AnonStorey1A._003C_003Em__E) == null)
				{
					AssetManager.DecreaseReferenceCount(_003CLoadTheme_003Ec__AnonStorey1A.textureConfig.textureName, _003CLoadTheme_003Ec__AnonStorey1A.textureConfig.materials.Length);
				}
			}
			_003CLoadTheme_003Ec__AnonStorey1B _003CLoadTheme_003Ec__AnonStorey1B = new _003CLoadTheme_003Ec__AnonStorey1B();
			foreach (Theme.SoundConfig sound3 in m_currentTheme.sounds)
			{
				_003CLoadTheme_003Ec__AnonStorey1B.soundConfig = sound3;
				Sound[] sounds = _003CLoadTheme_003Ec__AnonStorey1B.soundConfig.sounds;
				foreach (Sound sound in sounds)
				{
					sound.clip = null;
				}
				if (theme.sounds.Find(_003CLoadTheme_003Ec__AnonStorey1B._003C_003Em__F) == null)
				{
					AssetManager.DecreaseReferenceCount(_003CLoadTheme_003Ec__AnonStorey1B.soundConfig.audioClipName, _003CLoadTheme_003Ec__AnonStorey1B.soundConfig.sounds.Length);
				}
			}
		}
		foreach (Theme.TextureConfig texture2 in theme.textures)
		{
			Material[] materials2 = texture2.materials;
			foreach (Material material2 in materials2)
			{
				material2.mainTexture = AssetManager.LoadAsset<Texture>(texture2.textureName);
			}
		}
		foreach (Theme.SoundConfig sound4 in theme.sounds)
		{
			Sound[] sounds2 = sound4.sounds;
			foreach (Sound sound2 in sounds2)
			{
				sound2.clip = AssetManager.LoadAsset<AudioClip>(sound4.audioClipName);
			}
		}
		foreach (Theme.ColourConfig colour in theme.colours)
		{
			colour.material.SetColor("_TintColor", colour.colour);
		}
		if ((bool)m_currentTheme)
		{
			_003CLoadTheme_003Ec__AnonStorey1C _003CLoadTheme_003Ec__AnonStorey1C = new _003CLoadTheme_003Ec__AnonStorey1C();
			foreach (Theme.TextureConfig texture3 in m_currentTheme.textures)
			{
				_003CLoadTheme_003Ec__AnonStorey1C.textureConfig = texture3;
				if (theme.textures.Find(_003CLoadTheme_003Ec__AnonStorey1C._003C_003Em__10) != null)
				{
					AssetManager.DecreaseReferenceCount(_003CLoadTheme_003Ec__AnonStorey1C.textureConfig.textureName, _003CLoadTheme_003Ec__AnonStorey1C.textureConfig.materials.Length);
				}
			}
			_003CLoadTheme_003Ec__AnonStorey1D _003CLoadTheme_003Ec__AnonStorey1D = new _003CLoadTheme_003Ec__AnonStorey1D();
			foreach (Theme.SoundConfig sound5 in m_currentTheme.sounds)
			{
				_003CLoadTheme_003Ec__AnonStorey1D.soundConfig = sound5;
				if (theme.sounds.Find(_003CLoadTheme_003Ec__AnonStorey1D._003C_003Em__11) != null)
				{
					AssetManager.DecreaseReferenceCount(_003CLoadTheme_003Ec__AnonStorey1D.soundConfig.audioClipName, _003CLoadTheme_003Ec__AnonStorey1D.soundConfig.sounds.Length);
				}
			}
		}
		m_currentTheme = theme;
	}

	private void Awake()
	{
		Instance = this;
	}
}
