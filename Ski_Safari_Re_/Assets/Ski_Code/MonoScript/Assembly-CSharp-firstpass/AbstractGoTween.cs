using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGoTween
{
	public int id;

	protected bool _didStart;

	protected bool _didComplete;

	protected float _elapsedTime;

	protected float _totalElapsedTime;

	protected bool _isLoopingBackOnPingPong;

	protected int _completedIterations;

	protected Action<AbstractGoTween> _onStart;

	protected Action<AbstractGoTween> _onComplete;

	public GoTweenState state { get; protected set; }

	public float duration { get; protected set; }

	public float totalDuration { get; protected set; }

	public float timeScale { get; set; }

	public GoUpdateType updateType { get; protected set; }

	public GoLoopType loopType { get; protected set; }

	public int iterations { get; protected set; }

	public bool autoRemoveOnComplete { get; set; }

	public bool isReversed { get; protected set; }

	public float totalElapsedTime
	{
		get
		{
			return _totalElapsedTime;
		}
	}

	public bool isLoopoingBackOnPingPong
	{
		get
		{
			return _isLoopingBackOnPingPong;
		}
	}

	public int completedIterations
	{
		get
		{
			return _completedIterations;
		}
	}

	public void setOnStartHandler(Action<AbstractGoTween> onStart)
	{
		_onStart = onStart;
	}

	public void setOnCompleteHandler(Action<AbstractGoTween> onComplete)
	{
		_onComplete = onComplete;
	}

	protected virtual void onStart()
	{
		_didStart = true;
		if (_onStart != null)
		{
			_onStart(this);
		}
	}

	protected virtual void onComplete()
	{
		_didComplete = true;
		if (_onComplete != null)
		{
			_onComplete(this);
		}
	}

	public virtual bool update(float deltaTime)
	{
		if (!_didStart)
		{
			onStart();
		}
		if (isReversed)
		{
			_totalElapsedTime -= deltaTime;
		}
		else
		{
			_totalElapsedTime += deltaTime;
		}
		_totalElapsedTime = Mathf.Clamp(_totalElapsedTime, 0f, totalDuration);
		_completedIterations = (int)Mathf.Floor(_totalElapsedTime / duration);
		_isLoopingBackOnPingPong = false;
		if (loopType == GoLoopType.PingPong)
		{
			if (iterations < 0 && _completedIterations % 2 != 0)
			{
				_isLoopingBackOnPingPong = true;
			}
			else if (iterations > 0)
			{
				if (completedIterations >= iterations && _completedIterations % 2 == 0)
				{
					_isLoopingBackOnPingPong = true;
				}
				else if (completedIterations < iterations && _completedIterations % 2 != 0)
				{
					_isLoopingBackOnPingPong = true;
				}
			}
		}
		if (iterations > 0 && _completedIterations >= iterations)
		{
			_elapsedTime = duration;
			if (!isReversed)
			{
				state = GoTweenState.Complete;
			}
		}
		else if (_totalElapsedTime < duration)
		{
			_elapsedTime = _totalElapsedTime;
		}
		else
		{
			_elapsedTime = _totalElapsedTime % duration;
		}
		if (isReversed && _totalElapsedTime <= 0f)
		{
			state = GoTweenState.Complete;
		}
		return false;
	}

	public abstract bool isValid();

	public abstract bool removeTweenProperty(AbstractTweenProperty property);

	public abstract bool containsTweenProperty(AbstractTweenProperty property);

	public abstract List<AbstractTweenProperty> allTweenProperties();

	public virtual void destroy()
	{
		state = GoTweenState.Destroyed;
		Go.removeTween(this);
	}

	public void pause()
	{
		state = GoTweenState.Paused;
	}

	public void play()
	{
		state = GoTweenState.Running;
	}

	public void playForward()
	{
		isReversed = false;
		state = GoTweenState.Running;
	}

	public void playBackwards()
	{
		isReversed = true;
		state = GoTweenState.Running;
	}

	public abstract void rewind();

	public void restart(bool skipDelay = true)
	{
		_didComplete = false;
		rewind();
		state = GoTweenState.Running;
	}

	public void reverse()
	{
		isReversed = !isReversed;
	}

	public virtual void complete()
	{
		if (iterations >= 0)
		{
			_elapsedTime = ((!isReversed) ? duration : 0f);
			_totalElapsedTime = ((!isReversed) ? totalDuration : 0f);
			_completedIterations = ((!isReversed) ? iterations : 0);
			state = GoTweenState.Running;
		}
	}

	public virtual void goTo(float time)
	{
		time = Mathf.Clamp(time, 0f, totalDuration);
		_totalElapsedTime = time;
		_elapsedTime = _totalElapsedTime;
		if (iterations > 0 || iterations < 0)
		{
			_completedIterations = (int)Mathf.Floor(_totalElapsedTime / duration);
			if (loopType == GoLoopType.PingPong)
			{
				_isLoopingBackOnPingPong = _completedIterations % 2 != 0;
			}
			if (iterations < 0 || (iterations > 0 && _completedIterations < iterations + 1))
			{
				_elapsedTime = _totalElapsedTime % duration;
			}
		}
		update(0f);
	}

	public void goToAndPlay(float time)
	{
		state = GoTweenState.Running;
		goTo(time);
	}

	public IEnumerator waitForCompletion()
	{
		while (state != GoTweenState.Complete && state != GoTweenState.Destroyed)
		{
			yield return null;
		}
	}
}
