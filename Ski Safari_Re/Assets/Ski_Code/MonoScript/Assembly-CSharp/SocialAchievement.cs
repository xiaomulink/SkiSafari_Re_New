using UnityEngine;

public abstract class SocialAchievement : MonoBehaviour
{
	public string Id { get; set; }

	public abstract void Load();

	public abstract void Unload();
}
