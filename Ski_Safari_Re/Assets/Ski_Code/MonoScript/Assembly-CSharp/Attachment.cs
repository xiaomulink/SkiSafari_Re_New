using UnityEngine;

public class Attachment : MonoBehaviour
{
	public AttachmentGroup attachmentGroup;

	public string attachmentCategory;

	public GameObject detachedPrefab;

	public Player[] leaderPrefabs;

	public AnimatedSprite sprite;

	public float autoDetachDelay = -1f;

	public bool alwaysOverwrite;

	public Sound attachSound;

	public Sound igniteSound;

	public Sound detachSound;

	public GameObject detachEffectPrefab;

	public Sound followSound;

	public bool isPlayer;

	public void Detach(Transform transform)
	{
		GameObject gameObject = null;
		if ((bool)detachedPrefab)
		{
			gameObject = Pool.Spawn(detachedPrefab, transform.position, transform.rotation);
		}
		if (isPlayer)
		{
			Player component = gameObject.GetComponent<Player>();
			PlayerManager.ReplacePlayer(this, component);
		}
	}
}
