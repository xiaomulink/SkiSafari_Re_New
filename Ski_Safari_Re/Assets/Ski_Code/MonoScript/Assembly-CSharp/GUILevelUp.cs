using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUILevelUp : MonoBehaviour
{
	protected class QueuedReward
	{
		public GUIReward guiRewardPrefab;

		public string description;

		public int coins;

		public float descriptionOffset;

		public GUIRewardWindow guiRewardWindow;
	}

	public GUIDropShadowText titleText;

	public GUIDropShadowText levelNameText;

	public Transform badgeNode;

	public Sound badgeSound;

	public GUIRewardWindow rewardWindowPrefab;

	public GUITransitionAnimator transitionAnimator;

	private GoTweenConfig m_tweenConfig = new GoTweenConfig().scale(0.2f, true).setEaseType(GoEaseType.ElasticPunch);

	private IEnumerator Animate()
	{
		Go.to(base.transform, 1.5f, new GoTweenConfig().rotation(new Vector3(0f, 0f, 30f), true).setEaseType(GoEaseType.ElasticPunch));
		LevelManager.LevelDescriptor levelDescriptor = LevelManager.Instance.CurrentLevelDescriptor;
		levelNameText.Text = levelDescriptor.name;
		TransformUtils.Instantiate(levelDescriptor.largeBadge, badgeNode);
		List<QueuedReward> rewards = new List<QueuedReward>();
		int currentLevel = LevelManager.Instance.CurrentLevel;
		ItemSet[] itemSets = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet in itemSets)
		{
			Item[] items = itemSet.items;
			foreach (Item item in items)
			{
				if (item.requiredLevel == currentLevel && (bool)item.guiReward && !item.lockedUntilUpdate && item.CurrentCost == 0)
				{
					string description = string.Empty;
					float descriptionOffset = 0f;
					if (item is Slope)
					{
						description = item.displayName + " unlocked!";
					}
					else if (item is StartingSkier)
					{
						description = item.displayName + " available from cabin!";
					}
					rewards.Add(new QueuedReward
					{
						guiRewardPrefab = item.guiReward,
						description = description,
						descriptionOffset = descriptionOffset
					});
					break;
				}
			}
		}
		LevelManager.ScoreMultiplier[] scoreMultipliers = LevelManager.Instance.scoreMultipliers;
		foreach (LevelManager.ScoreMultiplier scoreMultiplier in scoreMultipliers)
		{
			if (scoreMultiplier.requiredLevel == currentLevel)
			{
				rewards.Add(new QueuedReward
				{
					guiRewardPrefab = scoreMultiplier.guiReward,
					description = "combo limit unlocked!",
					descriptionOffset = 2.5f
				});
				break;
			}
		}
		LevelManager.CoinBonus[] coinBonuses = LevelManager.Instance.coinBonuses;
		foreach (LevelManager.CoinBonus coinBonus in coinBonuses)
		{
			if (coinBonus.requiredLevel == currentLevel)
			{
				rewards.Add(new QueuedReward
				{
					guiRewardPrefab = coinBonus.guiReward,
					description = coinBonus.coins + " bonus coins!",
					coins = coinBonus.coins
				});
				break;
			}
		}
		titleText.gameObject.SetActive(true);
		levelNameText.gameObject.SetActive(false);
		badgeNode.gameObject.SetActive(false);
		yield return new WaitForSeconds(1.5f);
		badgeNode.gameObject.SetActive(true);
		levelNameText.gameObject.SetActive(true);
		SoundManager.Instance.PlaySound(badgeSound);
		Go.to(badgeNode.transform, 0.5f, m_tweenConfig);
		Go.to(levelNameText.transform, 0.5f, m_tweenConfig);
		yield return new WaitForSeconds(0.5f);
		SoundManager.Instance.PlaySound(levelDescriptor.levelUpSound);
		int rewardIndex = 0;
		foreach (QueuedReward reward2 in rewards)
		{
			yield return new WaitForSeconds(1f);
			GUIRewardWindow guiRewardWindow = Object.Instantiate(rewardWindowPrefab);
			guiRewardWindow.Setup(reward2.guiRewardPrefab, reward2.description, reward2.descriptionOffset, rewardIndex++);
			reward2.guiRewardWindow = guiRewardWindow;
			yield return new WaitForSeconds(0.5f);
			SoundManager.Instance.PlaySound(reward2.guiRewardPrefab.sound);
			Go.to(reward2.guiRewardWindow.rewardNode.gameObject.transform, 0.5f, m_tweenConfig);
			Go.to(reward2.guiRewardWindow.descriptionText.gameObject.transform, 0.5f, m_tweenConfig);
			if (reward2.coins > 0)
			{
				int startingCoinCount = GameState.CoinCount;
				float coinTimer = 0f;
				float coinGiveDuration = 1f;
				reward2.guiRewardWindow.coinCountNode.SetActive(true);
				while (coinTimer < coinGiveDuration)
				{
					coinTimer = Mathf.Min(coinGiveDuration, coinTimer + Time.deltaTime);
					int coinCount = Mathf.CeilToInt((float)startingCoinCount + (float)reward2.coins * (coinTimer / coinGiveDuration));
					reward2.guiRewardWindow.coinCountText.Text = coinCount.ToString("N0");
					yield return new WaitForSeconds(0f);
				}
				Go.to(reward2.guiRewardWindow.coinCountNode.gameObject.transform, 0.5f, m_tweenConfig);
				GameState.IncrementCoinCount(reward2.coins);
			}
		}
		yield return new WaitForSeconds(5f);
		transitionAnimator.Hide();
		foreach (QueuedReward reward in rewards)
		{
			reward.guiRewardWindow.transitionAnimator.Hide();
		}
	}

	private void Start()
	{
		StartCoroutine(Animate());
	}
}
