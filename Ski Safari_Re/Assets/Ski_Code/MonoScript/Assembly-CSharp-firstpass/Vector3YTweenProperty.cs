using System;
using UnityEngine;

public class Vector3YTweenProperty : Vector3XTweenProperty
{
	public Vector3YTweenProperty(string propertyName, float endValue, bool isRelative = false)
		: base(propertyName, endValue, isRelative)
	{
	}

	public override void prepareForUse()
	{
		_getter = GoTweenUtils.getterForProperty<Func<Vector3>>(_ownerTween.target, propertyName);
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			_endValue = _getter().y;
		}
		else
		{
			_startValue = _getter().y;
		}
		if (_isRelative && !_ownerTween.isFrom)
		{
			_diffValue = _endValue;
		}
		else
		{
			_diffValue = _endValue - _startValue;
		}
	}

	public override void tick(float totalElapsedTime)
	{
		Vector3 obj = _getter();
		obj.y = _easeFunction(totalElapsedTime, _startValue, _diffValue, _ownerTween.duration);
		_setter(obj);
	}
}
