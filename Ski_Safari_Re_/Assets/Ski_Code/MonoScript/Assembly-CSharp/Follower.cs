using UnityEngine;

public class Follower : MonoBehaviour
{
	public string followerCategory;

	public AnimatedSprite sprite;

	public Player leaderPrefab;

	public GameObject detachedPrefab;

	public float followDistance = 2f;

	public Transform chainVisualNode;

	public AttachNode[] attachNodes;
}
