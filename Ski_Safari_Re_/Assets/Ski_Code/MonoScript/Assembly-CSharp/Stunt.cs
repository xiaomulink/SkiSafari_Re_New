using UnityEngine;

public abstract class Stunt : MonoBehaviour
{
	public StuntManager Manager { get; set; }

	protected abstract void OnEnable();

	protected abstract void OnDisable();

	public virtual void OnReset()
	{
	}
}
