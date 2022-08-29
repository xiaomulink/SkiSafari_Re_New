using System.Collections.Generic;
using UnityEngine;

public class PooledParticles : MonoBehaviour
{
	private List<ParticleSystem> emitOnStart = new List<ParticleSystem>();

	private List<ParticleSystem> autoStopEmitters = new List<ParticleSystem>();

	private bool despawnOnAutoStop;

    private ParticleSystem[] animators;

    private int updateCount;

	public static void Apply(GameObject rootObj)
	{
		PooledParticles pooledParticles = rootObj.AddComponent<PooledParticles>();
        ParticleSystem[] array = pooledParticles.animators;
		foreach (ParticleSystem particleAnimator in array)
		{
            ParticleSystem component = particleAnimator.GetComponent<ParticleSystem>();
			if (!component)
			{
				continue;
			}
			if (component.enableEmission)
			{
				pooledParticles.emitOnStart.Add(component);
			}
		
		}
	}

	private void Awake()
	{
		animators = GetComponentsInChildren<ParticleSystem>();
	}

	private void OnEnable()
	{
		ProcessAutodestroy();
		foreach (ParticleSystem item in emitOnStart)
		{
			item.enableEmission = true;
		}
		updateCount = 0;
	}

	private void ProcessAutodestroy()
	{
        ParticleSystem[] array = animators;
		foreach (ParticleSystem particleAnimator in array)
		{
			if (particleAnimator.enableEmission && (bool)particleAnimator.GetComponent<ParticleSystem>())
			{
				particleAnimator.enableEmission = false;
				if (!autoStopEmitters.Contains(particleAnimator.GetComponent<ParticleSystem>()))
				{
					autoStopEmitters.Add(particleAnimator.GetComponent<ParticleSystem>());
				}
				if (particleAnimator.transform == base.transform)
				{
					despawnOnAutoStop = true;
				}
			}
		}
	}

	private void Update()
	{
		updateCount++;
		if (updateCount < 3)
		{
			return;
		}
		ProcessAutodestroy();
        try
        {
            foreach (ParticleSystem autoStopEmitter in autoStopEmitters)
            {
                if (autoStopEmitter.particleCount <= 0)
                {
                    if (autoStopEmitter.enableEmission)
                    {
                        autoStopEmitter.enableEmission = false;
                    }
                    if (despawnOnAutoStop && !(autoStopEmitter.transform != base.transform))
                    {
                        Pool.Despawn(base.gameObject);
                    }
                }
            }
        }
        catch
        {

        }
	}

	public void OnDisable()
	{
        ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
        ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem ParticleEmitter in array)
		{
			  ParticleEmitter.Clear();
		}
	}
}
