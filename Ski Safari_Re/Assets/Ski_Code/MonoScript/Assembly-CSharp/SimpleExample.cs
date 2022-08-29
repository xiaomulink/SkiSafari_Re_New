using UnityEngine;

public class SimpleExample : MonoBehaviour
{
	public GameObject target;

	private Vector3 targetPosition;

	private void Start()
	{
		targetPosition = target.transform.position;
	}

	private void Update()
	{
		
		
	
		target.transform.position = Vector3.MoveTowards(target.transform.position, targetPosition, Time.deltaTime * 25f);
	}
}
