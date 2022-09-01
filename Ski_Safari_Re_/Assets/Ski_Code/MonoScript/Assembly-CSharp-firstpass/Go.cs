using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Go : MonoBehaviour
{
	public static GoEaseType defaultEaseType = GoEaseType.Linear;

	public static GoLoopType defaultLoopType = GoLoopType.RestartFromBeginning;

	public static GoUpdateType defaultUpdateType = GoUpdateType.Update;

	public static GoDuplicatePropertyRuleType duplicatePropertyRule = GoDuplicatePropertyRuleType.None;

	public static GoLogLevel logLevel = GoLogLevel.Warn;

	public static bool validateTargetObjectsEachTick = true;

	private static List<AbstractGoTween> _tweens = new List<AbstractGoTween>();

	private bool _timeScaleIndependentUpdateIsRunning;

	private static Go _instance = null;

	public static Go instance
	{
		get
		{
			return _instance;
		}
	}

	private void handleUpdateOfType(GoUpdateType updateType, float deltaTime)
	{
		for (int num = _tweens.Count - 1; num >= 0; num--)
		{
			AbstractGoTween abstractGoTween = _tweens[num];
			if (abstractGoTween.updateType == updateType && abstractGoTween.state == GoTweenState.Running && abstractGoTween.update(deltaTime * abstractGoTween.timeScale) && (abstractGoTween.state == GoTweenState.Destroyed || abstractGoTween.autoRemoveOnComplete))
			{
				removeTween(abstractGoTween);
				abstractGoTween.destroy();
			}
		}
	}

	private void Update()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.Update, Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.LateUpdate, Time.deltaTime);
		}
	}

	private void FixedUpdate()
	{
		if (_tweens.Count != 0)
		{
			handleUpdateOfType(GoUpdateType.FixedUpdate, Time.deltaTime);
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		_instance = this;
	}

	private IEnumerator timeScaleIndependentUpdate()
	{
		_timeScaleIndependentUpdateIsRunning = true;
		DateTime time = DateTime.Now;
		while (_tweens.Count > 0)
		{
			DateTime now = DateTime.Now;
			float elapsed = (float)(now - time).TotalSeconds;
			time = now;
			handleUpdateOfType(GoUpdateType.TimeScaleIndependentUpdate, elapsed);
			yield return null;
		}
		_timeScaleIndependentUpdateIsRunning = false;
	}

	private static bool handleDuplicatePropertiesInTween(GoTween tween)
	{
		List<GoTween> list = tweensWithTarget(tween.target);
		List<AbstractTweenProperty> list2 = tween.allTweenProperties();
		foreach (GoTween item in list)
		{
			foreach (AbstractTweenProperty item2 in list2)
			{
				if (item.containsTweenProperty(item2))
				{
					if (duplicatePropertyRule == GoDuplicatePropertyRuleType.DontAddCurrentProperty)
					{
						return true;
					}
					if (duplicatePropertyRule == GoDuplicatePropertyRuleType.RemoveRunningProperty)
					{
						item.removeTweenProperty(item2);
					}
					return false;
				}
			}
		}
		return false;
	}

	[Conditional("UNITY_EDITOR")]
	private static void log(object format, params object[] paramList)
	{
		if (format is string)
		{
			UnityEngine.Debug.Log(string.Format(format as string, paramList));
		}
		else
		{

            UnityEngine.Debug.Log(format);
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void warn(object format, params object[] paramList)
	{
		if (logLevel != 0 && logLevel != GoLogLevel.Info)
		{
			if (format is string)
			{

                UnityEngine.Debug.LogWarning(string.Format(format as string, paramList));
			}
			else
			{

                UnityEngine.Debug.LogWarning(format);
			}
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void error(object format, params object[] paramList)
	{
		if (logLevel != 0 && logLevel != GoLogLevel.Info && logLevel != GoLogLevel.Warn)
		{
			if (format is string)
			{

                UnityEngine.Debug.LogError(string.Format(format as string, paramList));
			}
			else
			{

                UnityEngine.Debug.LogError(format);
			}
		}
	}

	public static GoTween to(object target, float duration, GoTweenConfig config)
	{
		GoTween goTween = new GoTween(target, duration, config);
		addTween(goTween);
		return goTween;
	}

	public static GoTween from(object target, float duration, GoTweenConfig config)
	{
		config.setIsFrom();
		GoTween goTween = new GoTween(target, duration, config);
		addTween(goTween);
		return goTween;
	}

	public static void addTween(AbstractGoTween tween)
	{
		if (!tween.isValid())
		{

            UnityEngine.Debug.Log("Tween is not valid: " + tween);
			return;
		}
		if (_tweens.Contains(tween))
		{

            UnityEngine.Debug.LogError("Tween already exists: " + tween);
			return;
		}
		if (duplicatePropertyRule != 0 && tween is GoTween)
		{
			if (handleDuplicatePropertiesInTween(tween as GoTween))
			{

                UnityEngine.Debug.LogError("Tween is a duplicate: " + tween);
				return;
			}
			if (!tween.isValid())
			{

                UnityEngine.Debug.LogError("Tween has been made invalid: " + tween);
				return;
			}
		}
		_tweens.Add(tween);
		if (!instance.enabled)
		{
			_instance.enabled = true;
		}
		if (tween is GoTween && ((GoTween)tween).isFrom && tween.state != GoTweenState.Paused)
		{
			tween.update(0f);
		}
		if (!_instance._timeScaleIndependentUpdateIsRunning && tween.updateType == GoUpdateType.TimeScaleIndependentUpdate)
		{
			_instance.StartCoroutine(_instance.timeScaleIndependentUpdate());
		}
	}

	public static bool removeTween(AbstractGoTween tween)
	{
		if (_tweens.Contains(tween))
		{
			_tweens.Remove(tween);
			if (_tweens.Count == 0)
			{
				instance.enabled = false;
			}
			return true;
		}
		return false;
	}

	public static List<AbstractGoTween> tweensWithId(int id)
	{
		List<AbstractGoTween> list = null;
		foreach (AbstractGoTween tween in _tweens)
		{
			if (tween.id == id)
			{
				if (list == null)
				{
					list = new List<AbstractGoTween>();
				}
				list.Add(tween);
			}
		}
		return list;
	}

	public static List<GoTween> tweensWithTarget(object target, bool traverseCollections = false)
	{
		List<GoTween> list = new List<GoTween>();
		foreach (AbstractGoTween tween in _tweens)
		{
			GoTween goTween = tween as GoTween;
			if (goTween != null && goTween.target == target)
			{
				list.Add(goTween);
			}
			if (!traverseCollections || goTween != null)
			{
				continue;
			}
			AbstractGoTweenCollection abstractGoTweenCollection = tween as AbstractGoTweenCollection;
			if (abstractGoTweenCollection != null)
			{
				List<GoTween> list2 = abstractGoTweenCollection.tweensWithTarget(target);
				if (list2.Count > 0)
				{
					list.AddRange(list2);
				}
			}
		}
		return list;
	}

	public static void killAllTweensWithTarget(object target)
	{
		foreach (GoTween item in tweensWithTarget(target, true))
		{
			item.destroy();
		}
	}
}
