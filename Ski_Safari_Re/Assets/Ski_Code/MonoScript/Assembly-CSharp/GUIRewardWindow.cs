using UnityEngine;

public class GUIRewardWindow : MonoBehaviour
{
	public Transform content;

	public Vector3 offsetPerReward = new Vector3(0f, -3f, 0f);

	public Transform rewardNode;

	public GUIDropShadowText descriptionText;

	public GameObject coinCountNode;

	public GUIDropShadowText coinCountText;

	public GUITransitionAnimator transitionAnimator;

	public void Setup(GUIReward rewardPrefab, string description, float descriptionOffset, int rewardIndex)
	{
		content.localPosition += offsetPerReward * rewardIndex;
		TransformUtils.Instantiate(rewardPrefab, rewardNode);
		descriptionText.Text = description;
		descriptionText.transform.localPosition += new Vector3(descriptionOffset, 0f, 0f);
		coinCountNode.SetActive(false);
	}
}
