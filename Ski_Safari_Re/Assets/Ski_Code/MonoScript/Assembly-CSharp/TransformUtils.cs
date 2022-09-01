using UnityEngine;

public class TransformUtils
{
	public static T Instantiate<T>(T prefab, Transform parent, bool linkParent, bool pooled) where T : Component
	{
		object obj = null;
		if (prefab == obj)
		{
			return (T)default(T);
		}
        Vector3 position=new Vector3();
        try
        {
            position = parent.transform.TransformPoint(prefab.transform.localPosition);
        }
        catch
        {
            Debug.LogError(prefab.gameObject.name);
        }
        Quaternion rotation = parent.transform.rotation * prefab.transform.localRotation;
		if (pooled)
		{
			GameObject gameObject = Pool.Spawn(prefab.gameObject, position, rotation);
			T component = gameObject.GetComponent<T>();
			if (linkParent)
			{
				component.transform.parent = parent;
			}
			return component;
		}
		GameObject gameObject2 = Object.Instantiate(prefab.gameObject, position, rotation) as GameObject;
		T component2 = gameObject2.GetComponent<T>();
		if (linkParent)
		{
			component2.transform.parent = parent;
		}
		return component2;
	}

	public static T Instantiate<T>(T prefab, Transform parent, bool linkParent) where T : Component
	{
		return Instantiate(prefab, parent, linkParent, false);
	}

	public static T Instantiate<T>(T prefab, Transform parent) where T : Component
	{
        /*if (prefab == FlamePowerup)
        {
            GameObject go = Instantiate(prefab.gameObject, GameObject.Find("GamePool").transform);
            T t = go.GetComponent<T>();
            go.SetActive(false);
        }*/
		return Instantiate(prefab, parent, true, false);
	}

	public static GameObject Instantiate(GameObject prefab, Transform parent, bool linkParent, bool pooled)
	{
		if (!prefab)
		{
			return null;
		}
		Vector3 position = parent.transform.TransformPoint(prefab.transform.localPosition);
		Quaternion rotation = parent.transform.rotation * prefab.transform.localRotation;
		GameObject gameObject = ((!pooled) ? Object.Instantiate(prefab, position, rotation) : Pool.Spawn(prefab, position, rotation)) as GameObject;
		if (linkParent)
		{
			gameObject.transform.parent = parent;
		}
		return gameObject;
	}

	public static GameObject Instantiate(GameObject prefab, Transform parent, bool linkParent)
	{
		return Instantiate(prefab, parent, linkParent, false);
	}

	public static GameObject Instantiate(GameObject prefab, Transform parent)
	{
		return Instantiate(prefab, parent, true, false);
	}

	public static string GetPath(Transform transform)
	{
		string text = transform.name;
		Transform parent = transform.parent;
		while ((bool)parent)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	public static Transform GetRoot(Transform transform)
	{
		if ((bool)transform.parent)
		{
			return GetRoot(transform.parent);
		}
		return transform;
	}
}
