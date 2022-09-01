using System;
using System.Collections.Generic;
using UnityEngine;

public class GoTween : AbstractGoTween
{
	private float _elapsedDelay;

	private bool _delayComplete;

	private List<AbstractTweenProperty> _tweenPropertyList = new List<AbstractTweenProperty>();

	private GoEaseType _easeType;

	public object target { get; private set; }

	public float delay { get; private set; }

	public bool isFrom { get; private set; }

	public GoEaseType easeType
	{
		get
		{
			return _easeType;
		}
		set
		{
			_easeType = value;
			foreach (AbstractTweenProperty tweenProperty in _tweenPropertyList)
			{
				tweenProperty.setEaseType(value);
			}
		}
	}

	public GoTween(object target, float duration, GoTweenConfig config, Action<AbstractGoTween> onComplete = null)
	{
		base.autoRemoveOnComplete = true;
		this.target = target;
		base.duration = duration;
		id = config.id;
		delay = config.delay;
		base.loopType = config.loopType;
		base.iterations = config.iterations;
		_easeType = config.easeType;
		base.updateType = config.propertyUpdateType;
		isFrom = config.isFrom;
		base.timeScale = config.timeScale;
		_onComplete = config.onCompleteHandler;
		_onStart = config.onStartHandler;
		if (config.isPaused)
		{
			base.state = GoTweenState.Paused;
		}
		if (onComplete != null)
		{
			_onComplete = onComplete;
		}
		for (int i = 0; i < config.tweenProperties.Count; i++)
		{
			AbstractTweenProperty abstractTweenProperty = config.tweenProperties[i];
			if (abstractTweenProperty.isInitialized)
			{
				abstractTweenProperty = abstractTweenProperty.clone();
			}
			addTweenProperty(abstractTweenProperty);
		}
		if (base.iterations < 0)
		{
			base.totalDuration = float.PositiveInfinity;
		}
		else
		{
			base.totalDuration = (float)base.iterations * duration;
		}
	}

	public override bool update(float deltaTime)
	{
		if (Go.validateTargetObjectsEachTick && (target == null || target.Equals(null)))
		{
			Debug.LogWarning("target validation failed. destroying the tween to avoid errors");
			base.autoRemoveOnComplete = true;
			return true;
		}
		if (!_delayComplete && _elapsedDelay < delay)
		{
			if (base.timeScale != 0f)
			{
				_elapsedDelay += deltaTime / base.timeScale;
			}
			if (_elapsedDelay >= delay)
			{
				_delayComplete = true;
			}
			return false;
		}
		base.update(deltaTime);
		float num = ((!_isLoopingBackOnPingPong) ? _elapsedTime : (base.duration - _elapsedTime));
		for (int i = 0; i < _tweenPropertyList.Count; i++)
		{
			_tweenPropertyList[i].tick(num);
		}
		if (base.state == GoTweenState.Complete)
		{
			if (!_didComplete)
			{
				onComplete();
			}
			return true;
		}
		return false;
	}

	public override bool isValid()
	{
		return target != null;
	}

	public void addTweenProperty(AbstractTweenProperty tweenProp)
	{
		if (tweenProp.validateTarget(target))
		{
			if (_tweenPropertyList.Contains(tweenProp))
			{
				Debug.Log("not adding tween property because one already exists: " + tweenProp);
				return;
			}
			_tweenPropertyList.Add(tweenProp);
			tweenProp.init(this);
		}
		else
		{
			Debug.Log("tween failed to validate target: " + tweenProp);
		}
	}

	public override bool removeTweenProperty(AbstractTweenProperty property)
	{
		if (_tweenPropertyList.Contains(property))
		{
			_tweenPropertyList.Remove(property);
			return true;
		}
		return false;
	}

	public override bool containsTweenProperty(AbstractTweenProperty property)
	{
		return _tweenPropertyList.Contains(property);
	}

	public void clearTweenProperties()
	{
		_tweenPropertyList.Clear();
	}

	public override List<AbstractTweenProperty> allTweenProperties()
	{
		return _tweenPropertyList;
	}

	protected override void onStart()
	{
		base.onStart();
		for (int i = 0; i < _tweenPropertyList.Count; i++)
		{
			_tweenPropertyList[i].prepareForUse();
		}
	}

	public override void destroy()
	{
		base.destroy();
		_tweenPropertyList.Clear();
		target = null;
	}

	public override void goTo(float time)
	{
		_delayComplete = true;
		_elapsedDelay = delay;
		base.goTo(time);
	}

	public override void rewind()
	{
		rewind(true);
	}

	public void rewind(bool skipDelay)
	{
		base.state = GoTweenState.Paused;
		_elapsedTime = (_totalElapsedTime = 0f);
		_elapsedDelay = ((!skipDelay) ? 0f : base.duration);
		_delayComplete = skipDelay;
		_isLoopingBackOnPingPong = false;
		_completedIterations = 0;
	}

	public override void complete()
	{
		if (base.iterations >= 0)
		{
			base.complete();
			_delayComplete = true;
		}
	}
}
